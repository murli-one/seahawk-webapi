using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seahawk_WebAPI.Contracts.VesselDetails;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/vessel-details")]
[Authorize]
public class VesselDetailsApiController : ControllerBase
{
    private readonly IVesselDetailService _service;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public VesselDetailsApiController(
        IVesselDetailService service,
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _service = service;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    // GET: /api/vessel-details?page=1&pageSize=14&filterVesselName=test
    [HttpGet]
    public async Task<ActionResult<VesselDetailListResponse>> GetVessels(
        [FromQuery] VesselDetailListRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User is not logged in." });

        var currentRole = currentUser.Role;

        var query = _unitOfWork.VesselDetail.Query()
            .Include(v => v.Company)
            .AsNoTracking();

        if (currentRole == Role.SystemAdmin)
        {
            // SystemAdmin can see all vessels.
        }
        else if (currentRole == Role.ManagementUser)
        {
            var companyUser = _unitOfWork.CompanyUser.GetCompanyUserByUserId(currentUser.Id);

            if (companyUser == null)
            {
                return Ok(new VesselDetailListResponse
                {
                    CurrentUser = currentUser.UserName,
                    CurrentUserEmail = currentUser.Email,
                    CurrentUserRole = currentRole.ToString(),
                    PageNumber = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    FilterVesselName = request.FilterVesselName,
                    FilterIMONumber = request.FilterIMONumber,
                    FilterOwner = request.FilterOwner,
                    FilterCallSign = request.FilterCallSign,
                    FilterCompanyName = request.FilterCompanyName,
                    Items = new List<VesselDetailDto>()
                });
            }

            query = query.Where(v => v.CompanyId == companyUser.CompanyId);
        }
        else
        {
            var assigned = await _service.GetVesselsForUserAsync(currentUser.Id, 0, currentRole);
            var allowedIds = assigned?.Select(x => x.Id).Distinct().ToList() ?? new List<int>();

            if (!allowedIds.Any())
            {
                return Ok(new VesselDetailListResponse
                {
                    CurrentUser = currentUser.UserName,
                    CurrentUserEmail = currentUser.Email,
                    CurrentUserRole = currentRole.ToString(),
                    PageNumber = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = 0,
                    FilterVesselName = request.FilterVesselName,
                    FilterIMONumber = request.FilterIMONumber,
                    FilterOwner = request.FilterOwner,
                    FilterCallSign = request.FilterCallSign,
                    FilterCompanyName = request.FilterCompanyName,
                    Items = new List<VesselDetailDto>()
                });
            }

            query = query.Where(v => allowedIds.Contains(v.Id));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterVesselName))
        {
            var value = request.FilterVesselName.Trim();
            query = query.Where(v => v.VesselName != null && v.VesselName.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterIMONumber))
        {
            var value = request.FilterIMONumber.Trim();
            query = query.Where(v => v.IMONumber != null && v.IMONumber.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterOwner))
        {
            var value = request.FilterOwner.Trim();
            query = query.Where(v => v.Owner != null && v.Owner.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterCallSign))
        {
            var value = request.FilterCallSign.Trim();
            query = query.Where(v => v.CallSign != null && v.CallSign.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterCompanyName))
        {
            var value = request.FilterCompanyName.Trim();
            query = query.Where(v =>
                v.Company != null &&
                v.Company.CompanyName != null &&
                v.Company.CompanyName.Contains(value));
        }

        var totalCount = await query.CountAsync();

        var vessels = await query
            .OrderByDescending(v => v.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return Ok(new VesselDetailListResponse
        {
            CurrentUser = currentUser.UserName,
            CurrentUserEmail = currentUser.Email,
            CurrentUserRole = currentRole.ToString(),

            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,

            FilterVesselName = request.FilterVesselName,
            FilterIMONumber = request.FilterIMONumber,
            FilterOwner = request.FilterOwner,
            FilterCallSign = request.FilterCallSign,
            FilterCompanyName = request.FilterCompanyName,

            Items = vessels.Select(ToDto).ToList()
        });
    }

    // GET: /api/vessel-details/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VesselDetailDto>> GetVessel(int id)
    {
        var vessel = await _service.GetByIdAsync(id);

        if (vessel == null)
            return NotFound(new { message = "Vessel not found." });

        return Ok(ToDto(vessel));
    }

    // GET: /api/vessel-details/create-options
    [HttpGet("create-options")]
    public async Task<ActionResult<VesselDetailOptionsResponse>> GetCreateOptions()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User is not logged in." });

        var response = new VesselDetailOptionsResponse
        {
            CurrentUserRole = currentUser.Role.ToString()
        };

        if (currentUser.Role == Role.SystemAdmin)
        {
            var companies = await _service.GetCompanies();

            response.Companies = companies
                .Select(c => new SelectOptionDto
                {
                    Value = c.Id.ToString(),
                    Text = c.CompanyName ?? "",
                    Selected = false
                })
                .ToList();
        }
        else if (currentUser.Role == Role.ManagementUser)
        {
            var companyUser = _unitOfWork.CompanyUser.GetCompanyUserByUserId(currentUser.Id);

            if (companyUser != null)
            {
                var company = await _unitOfWork.Companies.GetByIdAsync(companyUser.CompanyId);

                if (company != null)
                {
                    response.Companies = new List<SelectOptionDto>
                    {
                        new SelectOptionDto
                        {
                            Value = company.Id.ToString(),
                            Text = company.CompanyName ?? "",
                            Selected = true
                        }
                    };
                }
            }
        }
        else
        {
            return Forbid();
        }

        return Ok(response);
    }

    // POST: /api/vessel-details
    [HttpPost]
    public async Task<ActionResult<VesselDetailDto>> CreateVessel(
        [FromBody] VesselDetailCreateUpdateRequest request)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User is not logged in." });

        if (currentUser.Role == Role.VesselUser)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new VesselDetailApiResponse
            {
                Success = false,
                Message = "You are not allowed to create vessels."
            });
        }

        var vessel = new VesselDetail();
        ApplyFullUpdate(vessel, request);

        if (currentUser.Role == Role.ManagementUser)
        {
            var companyUser = _unitOfWork.CompanyUser.GetCompanyUserByUserId(currentUser.Id);

            if (companyUser != null)
            {
                vessel.CompanyId = companyUser.CompanyId;
            }
        }

        await _service.AddAsync(vessel);

        return CreatedAtAction(nameof(GetVessel), new { id = vessel.Id }, ToDto(vessel));
    }

    // PUT: /api/vessel-details/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<VesselDetailApiResponse>> UpdateVessel(
        int id,
        [FromBody] VesselDetailCreateUpdateRequest request)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User is not logged in." });

        var role = currentUser.Role;

        var existingVessel = await _service.GetByIdAsync(id);

        if (existingVessel == null)
            return NotFound(new VesselDetailApiResponse
            {
                Success = false,
                Message = "Vessel not found."
            });

        if (role == Role.VesselUser)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new VesselDetailApiResponse
            {
                Success = false,
                Message = "You are not allowed to edit vessels."
            });
        }

        if (role == Role.ManagementUser)
        {
            var companyUser = _unitOfWork.CompanyUser.GetCompanyUserByUserId(currentUser.Id);

            if (companyUser == null || existingVessel.CompanyId != companyUser.CompanyId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new VesselDetailApiResponse
                {
                    Success = false,
                    Message = "You cannot edit vessels from another company."
                });
            }

            ApplyManagementUserUpdate(existingVessel, request);
        }
        else if (role == Role.SystemAdmin)
        {
            if (request.CompanyId.HasValue && request.CompanyId != existingVessel.CompanyId)
            {
                var newCompanyDetails = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId.Value);
                existingVessel.Company = newCompanyDetails;
            }

            ApplyFullUpdate(existingVessel, request);
        }

        await _service.UpdateAsync(existingVessel);

        return Ok(new VesselDetailApiResponse
        {
            Success = true,
            Message = "Vessel updated successfully."
        });
    }

    // DELETE: /api/vessel-details/{id}
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<VesselDetailApiResponse>> DeleteVessel(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User is not logged in." });

        if (currentUser.Role == Role.VesselUser)
            return Forbid();

        var vessel = await _service.GetByIdAsync(id);

        if (vessel == null)
            return NotFound(new VesselDetailApiResponse
            {
                Success = false,
                Message = "Vessel not found."
            });

        if (currentUser.Role == Role.ManagementUser)
        {
            var companyUser = _unitOfWork.CompanyUser.GetCompanyUserByUserId(currentUser.Id);

            if (companyUser == null || vessel.CompanyId != companyUser.CompanyId)
                return Forbid();
        }

        await _service.DeleteAsync(id);

        return Ok(new VesselDetailApiResponse
        {
            Success = true,
            Message = "Vessel deleted successfully."
        });
    }

    // GET: /api/vessel-details/{id}/edit-options
    [HttpGet("{id:int}/edit-options")]
    public async Task<ActionResult<VesselDetailOptionsResponse>> GetEditOptions(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User is not logged in." });

        var vessel = await _service.GetByIdAsync(id);

        if (vessel == null)
            return NotFound(new { message = "Vessel not found." });

        if (currentUser.Role == Role.VesselUser)
            return Forbid();

        if (currentUser.Role == Role.ManagementUser)
        {
            var companyUser = _unitOfWork.CompanyUser.GetCompanyUserByUserId(currentUser.Id);

            if (companyUser == null || vessel.CompanyId != companyUser.CompanyId)
                return Forbid();
        }

        var response = new VesselDetailOptionsResponse
        {
            CurrentUserRole = currentUser.Role.ToString()
        };

        if (currentUser.Role == Role.SystemAdmin)
        {
            var companies = await _service.GetCompanies();

            response.Companies = companies
                .Select(c => new SelectOptionDto
                {
                    Value = c.Id.ToString(),
                    Text = c.CompanyName ?? "",
                    Selected = vessel.CompanyId == c.Id
                })
                .ToList();
        }

        return Ok(response);
    }

    // GET: /api/vessel-details/export-xml
    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportXml(
        [FromQuery] string? filterVesselName,
        [FromQuery] string? filterIMONumber,
        [FromQuery] string? filterOwner,
        [FromQuery] string? filterCallSign)
    {
        var all = await _service.GetAllAsync();
        IEnumerable<VesselDetail> vessels = all;

        if (!string.IsNullOrWhiteSpace(filterVesselName))
            vessels = vessels.Where(v =>
                !string.IsNullOrEmpty(v.VesselName) &&
                v.VesselName.Contains(filterVesselName.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(filterIMONumber))
            vessels = vessels.Where(v =>
                !string.IsNullOrEmpty(v.IMONumber) &&
                v.IMONumber.Contains(filterIMONumber.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(filterOwner))
            vessels = vessels.Where(v =>
                !string.IsNullOrEmpty(v.Owner) &&
                v.Owner.Contains(filterOwner.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(filterCallSign))
            vessels = vessels.Where(v =>
                !string.IsNullOrEmpty(v.CallSign) &&
                v.CallSign.Contains(filterCallSign.Trim(), StringComparison.OrdinalIgnoreCase));

        var xml = new XDocument(
            new XElement("Vessels",
                vessels.Select(v =>
                    new XElement("Vessel",
                        new XElement("Id", v.Id),
                        new XElement("VesselName", v.VesselName),
                        new XElement("IMONumber", v.IMONumber),
                        new XElement("BillTo", v.BillTo),
                        new XElement("Built", v.Built),
                        new XElement("ByEMail", v.ByEMail),
                        new XElement("ByFax", v.ByFax),
                        new XElement("ByTelex", v.ByTelex),
                        new XElement("CallSign", v.CallSign),
                        new XElement("Charterer", v.Charterer),
                        new XElement("Class", v.Class),
                        new XElement("Comments", v.Comments),
                        new XElement("CommissionType", v.CommissionType),
                        new XElement("DOGrade", v.DOGrade),
                        new XElement("DOReportType", v.DOReportType),
                        new XElement("Diesel", v.Diesel),
                        new XElement("Draft", v.Draft),
                        new XElement("Dwt", v.Dwt),
                        new XElement("Email", v.Email),
                        new XElement("ExVesselName", v.ExVesselName),
                        new XElement("FOReportType", v.FOReportType),
                        new XElement("FaxArea", v.FaxArea),
                        new XElement("FaxCountry", v.FaxCountry),
                        new XElement("FaxNumber", v.FaxNumber),
                        new XElement("FuelSystem", v.FuelSystem),
                        new XElement("GO", v.GO),
                        new XElement("GOGrade", v.GOGrade),
                        new XElement("GeneratorType", v.GeneratorType),
                        new XElement("HFO", v.HFO),
                        new XElement("HFOGrade", v.HFOGrade),
                        new XElement("IFO", v.IFO),
                        new XElement("IFOGrade", v.IFOGrade),
                        new XElement("Owner", v.Owner),
                        new XElement("Propulsion", v.Propulsion),
                        new XElement("Purifier", v.Purifier),
                        new XElement("Registry", v.Registry),
                        new XElement("TlxNumber", v.TlxNumber),
                        new XElement("Type", v.Type)
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(bytes, "application/xml", "Vessels.xml");
    }

    // POST: /api/vessel-details/export-excel
    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel([FromBody] VesselDetailListRequest request)
    {
        var all = await _service.GetAllAsync();
        IEnumerable<VesselDetail> vessels = all;

        if (!string.IsNullOrWhiteSpace(request.FilterVesselName))
            vessels = vessels.Where(v =>
                !string.IsNullOrEmpty(v.VesselName) &&
                v.VesselName.Contains(request.FilterVesselName.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.FilterIMONumber))
            vessels = vessels.Where(v =>
                !string.IsNullOrEmpty(v.IMONumber) &&
                v.IMONumber.Contains(request.FilterIMONumber.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.FilterOwner))
            vessels = vessels.Where(v =>
                !string.IsNullOrEmpty(v.Owner) &&
                v.Owner.Contains(request.FilterOwner.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.FilterCallSign))
            vessels = vessels.Where(v =>
                !string.IsNullOrEmpty(v.CallSign) &&
                v.CallSign.Contains(request.FilterCallSign.Trim(), StringComparison.OrdinalIgnoreCase));

        using var stream = new MemoryStream();

        using (var spreadsheet = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = spreadsheet.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            var sheetData = new SheetData();
            worksheetPart.Worksheet.AppendChild(sheetData);

            var sheets = spreadsheet.WorkbookPart!.Workbook.AppendChild(new Sheets());

            sheets.Append(new Sheet
            {
                Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Vessels"
            });

            var headers = new[]
            {
                "Vessel Name", "Email", "IMO Number", "Owner", "Call Sign",
                "IFO", "HFO", "Diesel", "Charterer", "Built", "Class", "Type",
                "DWT", "Draft", "Registry", "Fax Number", "Tlx Number",
                "Propulsion", "Generator Type", "Purifier", "Filter",
                "Fuel System", "GO", "HFO Grade", "IFO Grade", "DO Grade",
                "GO Grade", "By Fax", "By Email", "By Telex", "FO Report Type",
                "Fax Country", "Fax Area", "DO Report Type", "Commission Type",
                "Bill To", "Comments", "Ex Vessel Name"
            };

            var headerRow = new Row();

            foreach (var header in headers)
            {
                headerRow.AppendChild(new Cell
                {
                    CellValue = new CellValue(header),
                    DataType = CellValues.String
                });
            }

            sheetData.AppendChild(headerRow);

            int[] maxLengths = new int[headers.Length];

            for (int i = 0; i < headers.Length; i++)
                maxLengths[i] = headers[i].Length;

            foreach (var v in vessels)
            {
                var row = new Row();

                string[] values =
                {
                    v.VesselName ?? "",
                    v.Email ?? "",
                    v.IMONumber ?? "",
                    v.Owner ?? "",
                    v.CallSign ?? "",
                    v.IFO ?? "",
                    v.HFO ?? "",
                    v.Diesel ?? "",
                    v.Charterer ?? "",
                    v.Built?.ToString() ?? "",
                    v.Class ?? "",
                    v.Type ?? "",
                    v.Dwt?.ToString() ?? "",
                    v.Draft?.ToString() ?? "",
                    v.Registry ?? "",
                    v.FaxNumber ?? "",
                    v.TlxNumber ?? "",
                    v.Propulsion ?? "",
                    v.GeneratorType ?? "",
                    v.Purifier ?? "",
                    v.Filter ?? "",
                    v.FuelSystem ?? "",
                    v.GO ?? "",
                    v.HFOGrade ?? "",
                    v.IFOGrade ?? "",
                    v.DOGrade ?? "",
                    v.GOGrade ?? "",
                    v.ByFax ?? "",
                    v.ByEMail ?? "",
                    v.ByTelex ?? "",
                    v.FOReportType ?? "",
                    v.FaxCountry ?? "",
                    v.FaxArea ?? "",
                    v.DOReportType ?? "",
                    v.CommissionType ?? "",
                    v.BillTo ?? "",
                    v.Comments ?? "",
                    v.ExVesselName ?? ""
                };

                for (int i = 0; i < values.Length; i++)
                {
                    row.AppendChild(new Cell
                    {
                        CellValue = new CellValue(values[i]),
                        DataType = CellValues.String
                    });

                    maxLengths[i] = Math.Max(maxLengths[i], values[i].Length);
                }

                sheetData.AppendChild(row);
            }

            var columns = new Columns();

            for (uint i = 1; i <= headers.Length; i++)
            {
                double width = maxLengths[i - 1] + 2;

                columns.Append(new Column
                {
                    Min = i,
                    Max = i,
                    Width = width,
                    CustomWidth = true
                });
            }

            worksheetPart.Worksheet.InsertAt(columns, 0);
            workbookPart.Workbook.Save();
        }

        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Vessels.xlsx");
    }

    private static VesselDetailDto ToDto(VesselDetail vessel)
    {
        return new VesselDetailDto
        {
            Id = vessel.Id,
            VesselName = vessel.VesselName,
            IMONumber = vessel.IMONumber,
            BillTo = vessel.BillTo,
            Built = vessel.Built,
            ByEMail = vessel.ByEMail,
            ByFax = vessel.ByFax,
            ByTelex = vessel.ByTelex,
            CallSign = vessel.CallSign,
            Charterer = vessel.Charterer,
            Class = vessel.Class,
            Comments = vessel.Comments,
            CommissionType = vessel.CommissionType,
            DOGrade = vessel.DOGrade,
            DOReportType = vessel.DOReportType,
            Diesel = vessel.Diesel,
            Draft = vessel.Draft,
            Dwt = vessel.Dwt,
            Email = vessel.Email,
            ExVesselName = vessel.ExVesselName,
            FOReportType = vessel.FOReportType,
            FaxArea = vessel.FaxArea,
            FaxCountry = vessel.FaxCountry,
            FaxNumber = vessel.FaxNumber,
            Filter = vessel.Filter,
            FuelSystem = vessel.FuelSystem,
            GO = vessel.GO,
            GOGrade = vessel.GOGrade,
            GeneratorType = vessel.GeneratorType,
            HFO = vessel.HFO,
            HFOGrade = vessel.HFOGrade,
            IFO = vessel.IFO,
            IFOGrade = vessel.IFOGrade,
            Owner = vessel.Owner,
            Propulsion = vessel.Propulsion,
            Purifier = vessel.Purifier,
            Registry = vessel.Registry,
            TlxNumber = vessel.TlxNumber,
            Type = vessel.Type,
            CompanyId = vessel.CompanyId,
            CompanyName = vessel.Company?.CompanyName
        };
    }

    private static void ApplyManagementUserUpdate(
        VesselDetail vessel,
        VesselDetailCreateUpdateRequest request)
    {
        if (request.ByEMail != null)
            vessel.ByEMail = request.ByEMail;

        if (request.Email != null)
            vessel.Email = request.Email;

        if (request.ByFax != null)
            vessel.ByFax = request.ByFax;

        if (request.FaxArea != null)
            vessel.FaxArea = request.FaxArea;

        if (request.FaxCountry != null)
            vessel.FaxCountry = request.FaxCountry;

        if (request.FaxNumber != null)
            vessel.FaxNumber = request.FaxNumber;

        if (request.ByTelex != null)
            vessel.ByTelex = request.ByTelex;

        if (request.TlxNumber != null)
            vessel.TlxNumber = request.TlxNumber;

        if (request.Comments != null)
            vessel.Comments = request.Comments;

        if (request.Filter != null)
            vessel.Filter = request.Filter;

        if (request.FuelSystem != null)
            vessel.FuelSystem = request.FuelSystem;

        if (request.GeneratorType != null)
            vessel.GeneratorType = request.GeneratorType;

        if (request.Purifier != null)
            vessel.Purifier = request.Purifier;
    }

    private static void ApplyFullUpdate(
        VesselDetail vessel,
        VesselDetailCreateUpdateRequest request)
    {
        vessel.VesselName = request.VesselName;
        vessel.IMONumber = request.IMONumber;
        vessel.BillTo = request.BillTo;
        vessel.Built = request.Built;
        vessel.ByEMail = request.ByEMail;
        vessel.ByFax = request.ByFax;
        vessel.ByTelex = request.ByTelex;
        vessel.CallSign = request.CallSign;
        vessel.Charterer = request.Charterer;
        vessel.Class = request.Class;
        vessel.Comments = request.Comments;
        vessel.CommissionType = request.CommissionType;
        vessel.DOGrade = request.DOGrade;
        vessel.DOReportType = request.DOReportType;
        vessel.Diesel = request.Diesel;
        vessel.Draft = request.Draft;
        vessel.Dwt = request.Dwt;
        vessel.Email = request.Email;
        vessel.ExVesselName = request.ExVesselName;
        vessel.FOReportType = request.FOReportType;
        vessel.FaxArea = request.FaxArea;
        vessel.FaxCountry = request.FaxCountry;
        vessel.FaxNumber = request.FaxNumber;
        vessel.Filter = request.Filter;
        vessel.FuelSystem = request.FuelSystem;
        vessel.GO = request.GO;
        vessel.GOGrade = request.GOGrade;
        vessel.GeneratorType = request.GeneratorType;
        vessel.HFO = request.HFO;
        vessel.HFOGrade = request.HFOGrade;
        vessel.IFO = request.IFO;
        vessel.IFOGrade = request.IFOGrade;
        vessel.Owner = request.Owner;
        vessel.Propulsion = request.Propulsion;
        vessel.Purifier = request.Purifier;
        vessel.Registry = request.Registry;
        vessel.TlxNumber = request.TlxNumber;
        vessel.Type = request.Type;
        vessel.CompanyId = request.CompanyId;
    }
}