using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seahawk_WebAPI.Contracts.Companies;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/companies")]
[Authorize]
public class CompaniesApiController : ControllerBase
{
    private readonly ICompanyService _service;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public CompaniesApiController(
        ICompanyService service,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<CompanyPagedResponse>> GetCompanies([FromQuery] CompanyQueryRequest request)
    {
        request = NormalizeQuery(request);

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        var baseQueryResult = await BuildRoleBasedCompanyQueryAsync(currentUser);

        if (baseQueryResult.ForbiddenMessage != null)
            return Forbid();

        var baseQuery = baseQueryResult.Query;

        if (baseQuery == null)
        {
            return Ok(new CompanyPagedResponse
            {
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = 0,
                TotalPages = 0,
                CurrentUser = currentUser.UserName,
                CurrentUserEmail = currentUser.Email,
                CurrentUserRole = currentUser.Role.ToString(),
                FilterCompanyName = request.FilterCompanyName,
                FilterCity = request.FilterCity,
                FilterCountry = request.FilterCountry,
                Items = new List<CompanyDto>()
            });
        }

        baseQuery = ApplyFilters(baseQuery, request);

        var totalCount = await baseQuery
            .Select(c => c.Id)
            .Distinct()
            .CountAsync();

        var pageIds = await baseQuery
            .Select(c => c.Id)
            .Distinct()
            .OrderBy(id => id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var companies = await _unitOfWork.Companies.Query()
            .AsNoTracking()
            .Include(x => x.VesselDetailList)
            .Where(c => pageIds.Contains(c.Id))
            .OrderBy(c => c.Id)
            .ToListAsync();

        return Ok(new CompanyPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),

            CurrentUser = currentUser.UserName,
            CurrentUserEmail = currentUser.Email,
            CurrentUserRole = currentUser.Role.ToString(),

            FilterCompanyName = request.FilterCompanyName,
            FilterCity = request.FilterCity,
            FilterCountry = request.FilterCountry,

            Items = companies.Select(ToDto).ToList()
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid company id." });

        var company = await _unitOfWork.Companies.Query()
            .AsNoTracking()
            .Include(x => x.VesselDetailList)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (company == null)
            return NotFound(new { message = "Company not found." });

        return Ok(ToDto(company));
    }

    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<CompanyDetailsResponse>> GetCompanyDetails(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid company id." });

        var company = await _unitOfWork.Companies.Query()
            .AsNoTracking()
            .Include(x => x.VesselDetailList)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (company == null)
            return NotFound(new { message = "Company not found." });

        var availableVessels = await _unitOfWork.VesselDetail
            .GetAllAsync(v => v.CompanyId == null || v.CompanyId == 0);

        return Ok(new CompanyDetailsResponse
        {
            Company = ToDto(company),
            AvailableVessels = availableVessels
                .OrderBy(v => v.VesselName)
                .Select(ToVesselDto)
                .ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CompanyUpsertRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.CompanyName))
            return BadRequest(new { message = "Company name is required." });

        var company = new Company
        {
            CompanyName = request.CompanyName?.Trim(),
            BillingAddress = request.BillingAddress?.Trim(),
            City = request.City?.Trim(),
            StateOrProvince = request.StateOrProvince?.Trim(),
            PostalCode = request.PostalCode?.Trim(),
            Country = request.Country?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            FaxNumber = request.FaxNumber?.Trim(),
            EmailAddress = request.EmailAddress?.Trim(),
            Notes = request.Notes?.Trim(),
            ShipOwner = request.ShipOwner?.Trim(),
            FuelSupplier = request.FuelSupplier?.Trim(),
            ContractLab = request.ContractLab?.Trim(),
            Charterer = request.Charterer?.Trim(),
            InvoiceType = request.InvoiceType?.Trim(),
            CompanyKey = request.CompanyKey?.Trim(),
            FaxCountry = request.FaxCountry?.Trim(),
            FaxArea = request.FaxArea?.Trim(),
            ClientRef = request.ClientRef?.Trim()
        };

        await _service.AddAsync(company);

        return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, ToDto(company));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, [FromBody] CompanyUpsertRequest request)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid company id." });

        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.CompanyName))
            return BadRequest(new { message = "Company name is required." });

        var company = await _service.GetByIdAsync(id);

        if (company == null)
            return NotFound(new { message = "Company not found." });

        company.CompanyName = request.CompanyName?.Trim();
        company.BillingAddress = request.BillingAddress?.Trim();
        company.City = request.City?.Trim();
        company.StateOrProvince = request.StateOrProvince?.Trim();
        company.PostalCode = request.PostalCode?.Trim();
        company.Country = request.Country?.Trim();
        company.PhoneNumber = request.PhoneNumber?.Trim();
        company.FaxNumber = request.FaxNumber?.Trim();
        company.EmailAddress = request.EmailAddress?.Trim();
        company.Notes = request.Notes?.Trim();
        company.ShipOwner = request.ShipOwner?.Trim();
        company.FuelSupplier = request.FuelSupplier?.Trim();
        company.ContractLab = request.ContractLab?.Trim();
        company.Charterer = request.Charterer?.Trim();
        company.InvoiceType = request.InvoiceType?.Trim();
        company.CompanyKey = request.CompanyKey?.Trim();
        company.FaxCountry = request.FaxCountry?.Trim();
        company.FaxArea = request.FaxArea?.Trim();
        company.ClientRef = request.ClientRef?.Trim();

        await _service.UpdateAsync(company);

        return Ok(ToDto(company));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCompanyById(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid company id." });

        var company = await _service.GetByIdAsync(id);

        if (company == null)
            return NotFound(new { message = "Company not found." });

        await _service.DeleteAsync(id);

        return NoContent();
    }

    [HttpDelete("by-name/{companyName}")]
    public async Task<IActionResult> DeleteCompanyByName(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return BadRequest(new { message = "Company name is required." });

        var company = await _service.GetByNameAsync(companyName);

        if (company == null)
            return NotFound(new { message = "Company not found." });

        await _service.DeleteCompanyAsync(companyName);

        return NoContent();
    }

    [HttpPost("{companyId:int}/assign-vessels")]
    public async Task<ActionResult<CompanyMessageResponse>> AssignVessels(
        int companyId,
        [FromBody] AssignVesselsRequest request)
    {
        if (companyId <= 0)
            return BadRequest(new { message = "Invalid company id." });

        if (request == null || request.SelectedVesselIds == null || !request.SelectedVesselIds.Any())
            return BadRequest(new { message = "Please select at least one vessel." });

        var company = await _service.GetByIdAsync(companyId);
        if (company == null)
            return NotFound(new { message = "Company not found." });

        int successCount = 0;
        int skippedCount = 0;

        foreach (var vesselId in request.SelectedVesselIds.Distinct())
        {
            var vessel = await _unitOfWork.VesselDetail
                .GetAsync(v => v.Id == vesselId, tracked: true);

            if (vessel == null)
            {
                skippedCount++;
                continue;
            }

            if (vessel.CompanyId == null || vessel.CompanyId == 0)
            {
                vessel.CompanyId = companyId;
                successCount++;
            }
            else
            {
                skippedCount++;
            }
        }

        if (successCount > 0)
            await _unitOfWork.SaveAsync();

        return Ok(new CompanyMessageResponse
        {
            SuccessCount = successCount,
            SkippedCount = skippedCount,
            Message = successCount > 0
                ? $"{successCount} vessel(s) assigned successfully."
                : "No vessels were assigned. Selected vessel(s) may already be assigned."
        });
    }

    [HttpPost("{companyId:int}/remove-vessel/{vesselId:int}")]
    public async Task<ActionResult<CompanyMessageResponse>> RemoveVessel(int companyId, int vesselId)
    {
        if (companyId <= 0 || vesselId <= 0)
            return BadRequest(new { message = "Invalid company or vessel id." });

        var vessel = await _unitOfWork.VesselDetail
            .GetAsync(v => v.Id == vesselId && v.CompanyId == companyId, tracked: true);

        if (vessel == null)
            return NotFound(new { message = "Assigned vessel not found for this company." });

        vessel.CompanyId = null;

        await _unitOfWork.SaveAsync();

        return Ok(new CompanyMessageResponse
        {
            SuccessCount = 1,
            SkippedCount = 0,
            Message = "Vessel removed from company successfully."
        });
    }

    [HttpGet("available-vessels")]
    public async Task<ActionResult<List<CompanyVesselDto>>> GetAvailableVessels()
    {
        var availableVessels = await _unitOfWork.VesselDetail
            .GetAllAsync(v => v.CompanyId == null || v.CompanyId == 0);

        return Ok(availableVessels
            .OrderBy(v => v.VesselName)
            .Select(ToVesselDto)
            .ToList());
    }

    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportXml()
    {
        var companies = await _service.GetAllAsync();

        var xml = new XDocument(
            new XElement("Companies",
                companies.Select(c =>
                    new XElement("Company",
                        new XElement("Id", c.Id),
                        new XElement("CompanyName", c.CompanyName),
                        new XElement("BillingAddress", c.BillingAddress),
                        new XElement("City", c.City),
                        new XElement("StateOrProvince", c.StateOrProvince),
                        new XElement("PostalCode", c.PostalCode),
                        new XElement("Country", c.Country),
                        new XElement("PhoneNumber", c.PhoneNumber),
                        new XElement("FaxNumber", c.FaxNumber),
                        new XElement("EmailAddress", c.EmailAddress),
                        new XElement("Notes", c.Notes),
                        new XElement("ShipOwner", c.ShipOwner),
                        new XElement("FuelSupplier", c.FuelSupplier),
                        new XElement("ContractLab", c.ContractLab),
                        new XElement("Charterer", c.Charterer),
                        new XElement("InvoiceType", c.InvoiceType),
                        new XElement("CompanyKey", c.CompanyKey),
                        new XElement("FaxCountry", c.FaxCountry),
                        new XElement("FaxArea", c.FaxArea),
                        new XElement("ClientRef", c.ClientRef)
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(
            bytes,
            "application/xml",
            $"Companies_{DateTime.UtcNow:yyyyMMddHHmmss}.xml");
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var companies = await _service.GetAllAsync();

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
                Name = "Companies"
            });

            var headers = new[]
            {
                "Company Name",
                "Billing Address",
                "City",
                "State/Province",
                "Postal Code",
                "Country",
                "Phone Number",
                "Fax Number",
                "Email Address",
                "Notes",
                "Ship Owner",
                "Fuel Supplier",
                "Contract Lab",
                "Charterer",
                "Invoice Type",
                "Company Key",
                "Fax Country",
                "Fax Area",
                "Client Ref"
            };

            var maxLengths = new int[headers.Length];

            var headerRow = new Row();

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.AppendChild(CreateTextCell(headers[i]));
                maxLengths[i] = headers[i].Length;
            }

            sheetData.AppendChild(headerRow);

            foreach (var c in companies)
            {
                var values = new[]
                {
                    c.CompanyName ?? "",
                    c.BillingAddress ?? "",
                    c.City ?? "",
                    c.StateOrProvince ?? "",
                    c.PostalCode ?? "",
                    c.Country ?? "",
                    c.PhoneNumber ?? "",
                    c.FaxNumber ?? "",
                    c.EmailAddress ?? "",
                    c.Notes ?? "",
                    c.ShipOwner ?? "",
                    c.FuelSupplier ?? "",
                    c.ContractLab ?? "",
                    c.Charterer ?? "",
                    c.InvoiceType ?? "",
                    c.CompanyKey ?? "",
                    c.FaxCountry ?? "",
                    c.FaxArea ?? "",
                    c.ClientRef ?? ""
                };

                var row = new Row();

                for (int i = 0; i < values.Length; i++)
                {
                    row.AppendChild(CreateTextCell(values[i]));
                    maxLengths[i] = Math.Max(maxLengths[i], values[i].Length);
                }

                sheetData.AppendChild(row);
            }

            var columns = new Columns();

            for (uint i = 1; i <= headers.Length; i++)
            {
                columns.Append(new Column
                {
                    Min = i,
                    Max = i,
                    Width = Math.Min(maxLengths[i - 1] + 2, 80),
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
            $"Companies_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    private async Task<(IQueryable<Company>? Query, string? ForbiddenMessage)> BuildRoleBasedCompanyQueryAsync(
        ApplicationUser currentUser)
    {
        var currentRole = currentUser.Role;

        IQueryable<Company> baseQuery = _unitOfWork.Companies.Query()
            .AsNoTracking();

        if (currentRole == Role.SystemAdmin)
        {
            return (baseQuery, null);
        }

        if (currentRole == Role.ManagementUser)
        {
            var mappings = await _unitOfWork.CompanyUser
                .GetAllAsync(cu => cu.UserId == currentUser.Id);

            var companyIds = mappings?
                .Select(x => x.CompanyId)
                .Distinct()
                .ToList() ?? new List<int>();

            if (!companyIds.Any())
                return (null, null);

            baseQuery = baseQuery.Where(c => companyIds.Contains(c.Id));

            return (baseQuery, null);
        }

        var userData = await _unitOfWork.VesselUser
            .GetAsync(x => x.UserId == currentUser.Id);

        var vesselDetailId = userData?.VesselDetailId ?? 0;

        var vesselDetail = await _unitOfWork.VesselDetail
            .GetCompanyByVesselDetailIdAsync(vesselDetailId);

        var companyId = vesselDetail?.CompanyId;

        if (companyId == null || companyId == 0)
            return (null, null);

        baseQuery = baseQuery.Where(c => c.Id == companyId.Value);

        return (baseQuery, null);
    }

    private static IQueryable<Company> ApplyFilters(
        IQueryable<Company> query,
        CompanyQueryRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.FilterCompanyName))
        {
            var name = request.FilterCompanyName.Trim();
            query = query.Where(c => c.CompanyName != null && c.CompanyName.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterCity))
        {
            var city = request.FilterCity.Trim();
            query = query.Where(c => c.City != null && c.City.Contains(city));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterCountry))
        {
            var country = request.FilterCountry.Trim();
            query = query.Where(c => c.Country != null && c.Country.Contains(country));
        }

        return query;
    }

    private static CompanyQueryRequest NormalizeQuery(CompanyQueryRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 13;

        request.FilterCompanyName = request.FilterCompanyName?.Trim();
        request.FilterCity = request.FilterCity?.Trim();
        request.FilterCountry = request.FilterCountry?.Trim();

        return request;
    }

    private static CompanyDto ToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            CompanyName = company.CompanyName,
            BillingAddress = company.BillingAddress,
            City = company.City,
            StateOrProvince = company.StateOrProvince,
            PostalCode = company.PostalCode,
            Country = company.Country,
            PhoneNumber = company.PhoneNumber,
            FaxNumber = company.FaxNumber,
            EmailAddress = company.EmailAddress,
            Notes = company.Notes,
            ShipOwner = company.ShipOwner,
            FuelSupplier = company.FuelSupplier,
            ContractLab = company.ContractLab,
            Charterer = company.Charterer,
            InvoiceType = company.InvoiceType,
            CompanyKey = company.CompanyKey,
            FaxCountry = company.FaxCountry,
            FaxArea = company.FaxArea,
            ClientRef = company.ClientRef,
            AssignedVessels = company.VesselDetailList?
                .OrderBy(v => v.VesselName)
                .Select(ToVesselDto)
                .ToList() ?? new List<CompanyVesselDto>()
        };
    }

    private static CompanyVesselDto ToVesselDto(VesselDetail vessel)
    {
        return new CompanyVesselDto
        {
            Id = vessel.Id,
            VesselName = vessel.VesselName,
            IMONumber = vessel.IMONumber,
            Owner = vessel.Owner,
            Email = vessel.Email,
            CompanyId = vessel.CompanyId
        };
    }

    private static Cell CreateTextCell(string value)
    {
        return new Cell
        {
            CellValue = new CellValue(value ?? string.Empty),
            DataType = CellValues.String
        };
    }
}