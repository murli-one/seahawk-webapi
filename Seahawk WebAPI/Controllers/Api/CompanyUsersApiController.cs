using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.CompanyUsers;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;
using SeaHawkServices.Domain.StoredProcedures;
namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/company-users")]
[Authorize]
public class CompanyUsersApiController : ControllerBase
{
    private readonly ICompanyUserService _companyUserService;
    private readonly ICompanyService _companyService;
    private readonly IApplicationUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CompanyUsersApiController(
        ICompanyUserService companyUserService,
        ICompanyService companyService,
        IApplicationUserService userService,
        UserManager<ApplicationUser> userManager)
    {
        _companyUserService = companyUserService;
        _companyService = companyService;
        _userService = userService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<CompanyUserPagedResponse>> GetCompanyUsers(
        [FromQuery] CompanyUserQueryRequest request)
    {
        request = NormalizeQuery(request);

        var userName = User.Identity?.Name;
        var userDetails = await _userService.GetUserByUserNameAsync(userName);

        var allMappings = (List<CompanyUser>)await _companyUserService.GetUsersByCompanyAsync();
        var query = ApplyFilters(allMappings.AsQueryable(), request);

        var totalCount = query.Count();

        var pagedData = query
            .OrderBy(x => x.Company != null ? x.Company.CompanyName : "")
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var companies = (await _companyService.GetAllAsync())
            .OrderBy(x => x.CompanyName)
            .Select(x => new CompanyOptionDto
            {
                Id = x.Id,
                CompanyName = x.CompanyName
            })
            .ToList();

        var applicationUsers = (await _userService.GetAllUsersInsteadOfYours(userName)).ToList();

        var companyUserIds = (await _companyUserService.GetAllAsync())
            .ToList()
            .Select(x => x.UserId)
            .ToHashSet();

        var availableUsers = applicationUsers
            .Where(u => !companyUserIds.Contains(u.Id) && u.IsApprove == true)
            .OrderBy(u => u.UserName)
            .Select(ToUserOptionDto)
            .ToList();

        return Ok(new CompanyUserPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),

            SelectedCompanyId = request.CompanyId,
            FilterUserName = request.FilterUserName,
            FilterCompany = request.FilterCompany,
            FilterVesselName = request.FilterVesselName,

            SelectedUser = userDetails?.Name ?? "",
            SelectedUserEmail = userDetails?.Email ?? "",
            SelectedUserRole = userDetails?.Role.ToString() ?? "",

            Companies = companies,
            AvailableUsers = availableUsers,
            Items = pagedData.Select(ToDto).ToList()
        });
    }

    // GET: /api/company-users/unassigned-users?search=test&page=1&pageSize=15
    [HttpGet("unassigned-users")]
    public async Task<ActionResult<UnassignedCompanyUsersResponse>> GetUnassignedCompanyUsers(
        [FromQuery] string? search = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 15;

        var currentUserName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(currentUserName))
            return Unauthorized(new { message = "Current user not found." });

        var currentUser = await _userService.GetUserByUserNameAsync(currentUserName);

        var applicationUsers = (await _userService.GetAllUsersInsteadOfYours(currentUserName)).ToList();

        var assignedUserIds = (await _companyUserService.GetAllAsync())
            .Select(x => x.UserId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet();

        var query = applicationUsers
            .Where(u =>
                !assignedUserIds.Contains(u.Id) &&
                u.IsApprove == true)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();

            query = query.Where(u =>
                (!string.IsNullOrEmpty(u.UserName) &&
                 u.UserName.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(u.Name) &&
                 u.Name.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(u.Email) &&
                 u.Email.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(u.PhoneNumber) &&
                 u.PhoneNumber.Contains(value, StringComparison.OrdinalIgnoreCase)));
        }

        var totalCount = query.Count();

        var users = query
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToUserOptionDto)
            .ToList();

        return Ok(new UnassignedCompanyUsersResponse
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = pageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / pageSize),

            Search = search,

            SelectedUser = currentUser?.Name ?? "",
            SelectedUserEmail = currentUser?.Email ?? "",
            SelectedUserRole = currentUser?.Role.ToString() ?? "",

            Items = users
        });
    }

    [HttpPost("assign")]
    public async Task<ActionResult<CompanyUserMessageResponse>> Assign(
        [FromBody] AssignCompanyUserRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (request.CompanyId <= 0)
            return BadRequest(new { message = "Please select a company." });

        if (string.IsNullOrWhiteSpace(request.UserId) || request.UserId == "0")
            return BadRequest(new { message = "Please select a user." });

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        await _companyUserService.AssignUserToCompanyAsync(request.CompanyId, request.UserId);

        user.Role = Role.ManagementUser;
        await _userManager.UpdateAsync(user);

        return Ok(new CompanyUserMessageResponse
        {
            Message = "User assigned to company successfully.",
            SuccessCount = 1
        });
    }

    [HttpPost("remove")]
    public async Task<ActionResult<CompanyUserMessageResponse>> Remove(
        [FromBody] RemoveCompanyUserRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (request.CompanyUserId <= 0)
            return BadRequest(new { message = "Invalid company-user mapping id." });

        if (string.IsNullOrWhiteSpace(request.UserId))
            return BadRequest(new { message = "User id is required." });

        await _companyUserService.RemoveUserFromCompanyAsync(request.CompanyUserId);

        var remaining = (List<CompanyUser>)await _companyUserService.GetUsersByCompanyAsync();
        bool stillAssignedToAnyCompany = remaining.Any(x => x.UserId == request.UserId);

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user != null && !stillAssignedToAnyCompany)
        {
            user.Role = 0;
            await _userManager.UpdateAsync(user);
        }

        return Ok(new CompanyUserMessageResponse
        {
            Message = "User removed from company successfully.",
            SuccessCount = 1
        });
    }

    [HttpGet("{id:int}/edit")]
    public async Task<ActionResult<CompanyUserEditResponse>> GetCompanyUserForEdit(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid company-user mapping id." });

        var mappings = ((List<CompanyUser>)await _companyUserService.GetUsersByCompanyAsync()).ToList();

        var currentMapping = mappings.FirstOrDefault(x => x.Id == id);
        if (currentMapping == null)
            return NotFound(new { message = "Company-user mapping not found." });

        var user = await _userManager.FindByIdAsync(currentMapping.UserId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        var allCompanies = (await _companyService.GetAllAsync()).ToList();

        var userMappings = mappings
            .Where(x => x.UserId == user.Id)
            .Select(x => new AssignedCompanyRowDto
            {
                CompanyUserMappingId = x.Id,
                CompanyId = x.CompanyId,
                CompanyName = x.Company?.CompanyName ?? ""
            })
            .OrderBy(x => x.CompanyName)
            .ToList();

        var assignedCompanyIds = userMappings
            .Select(x => x.CompanyId)
            .ToHashSet();

        return Ok(new CompanyUserEditResponse
        {
            CompanyUserId = currentMapping.Id,
            UserId = user.Id,

            UserName = user.UserName ?? "",
            Name = user.Name ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber,

            Role = user.Role,
            RoleName = user.Role.ToString(),
            IsApprove = user.IsApprove,

            LastLoginAtUtc = user.LastLoginAtUtc,
            LastActivityAtUtc = user.LastActivityAtUtc,

            AssignedCompanies = userMappings,

            CompanyOptions = allCompanies
                .Where(c => !assignedCompanyIds.Contains(c.Id))
                .OrderBy(c => c.CompanyName)
                .Select(c => new CompanyOptionDto
                {
                    Id = c.Id,
                    CompanyName = c.CompanyName
                })
                .ToList()
        });
    }

    [HttpPut("{companyUserId:int}/user")]
    public async Task<ActionResult<CompanyUserMessageResponse>> UpdateCompanyUser(
        int companyUserId,
        [FromBody] UpdateCompanyUserRequest request)
    {
        if (companyUserId <= 0)
            return BadRequest(new { message = "Invalid company-user mapping id." });

        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.UserId))
            return BadRequest(new { message = "User id is required." });

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        user.Name = request.Name;
        user.Email = request.Email;
        user.NormalizedEmail = request.Email?.ToUpperInvariant();
        user.PhoneNumber = request.PhoneNumber;
        user.Role = request.Role;
        user.IsApprove = request.IsApprove.Value;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Failed to update user details.",
                errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        return Ok(new CompanyUserMessageResponse
        {
            Message = "Company user updated successfully.",
            SuccessCount = 1
        });
    }

    [HttpPost("add-companies-to-user")]
    public async Task<ActionResult<CompanyUserMessageResponse>> AddCompaniesToUser(
        [FromBody] AddCompaniesToUserRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.UserId))
            return BadRequest(new { message = "User id is required." });

        if (request.SelectedCompanyIds == null || !request.SelectedCompanyIds.Any())
            return BadRequest(new { message = "Please select at least one company." });

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        int successCount = 0;
        int failCount = 0;

        foreach (var companyId in request.SelectedCompanyIds.Distinct())
        {
            try
            {
                await _companyUserService.AssignUserToCompanyAsync(companyId, request.UserId);
                successCount++;
            }
            catch
            {
                failCount++;
            }
        }

        return Ok(new CompanyUserMessageResponse
        {
            SuccessCount = successCount,
            FailCount = failCount,
            Message = successCount > 0
                ? $"{successCount} company(s) assigned successfully."
                : "No company was assigned."
        });
    }

    [HttpPost("remove-company-from-user")]
    public async Task<ActionResult<CompanyUserMessageResponse>> RemoveCompanyFromUser(
        [FromBody] RemoveCompanyFromUserRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (request.CompanyUserMappingId <= 0)
            return BadRequest(new { message = "Invalid company-user mapping id." });

        if (string.IsNullOrWhiteSpace(request.UserId))
            return BadRequest(new { message = "User id is required." });

        await _companyUserService.RemoveUserFromCompanyAsync(request.CompanyUserMappingId);

        var response = new CompanyUserMessageResponse
        {
            Message = "Company removed successfully.",
            SuccessCount = 1
        };

        if (request.EditId == request.CompanyUserMappingId)
        {
            var remaining = ((List<CompanyUser>)await _companyUserService.GetUsersByCompanyAsync()).ToList();
            var next = remaining.FirstOrDefault(x => x.UserId == request.UserId);

            if (next != null)
            {
                response.Message = "Company removed successfully. Open next remaining company-user mapping.";
                response.SuccessCount = 1;
            }
            else
            {
                response.Message = "Company removed successfully. User has no remaining company mappings.";
            }
        }

        return Ok(response);
    }

    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportCompanyUsersXml([FromQuery] CompanyUserQueryRequest request)
    {
        request = NormalizeQuery(request);

        var rows = await GetFilteredCompanyUsersAsync(request);

        var xml = new XDocument(
            new XElement("CompanyUsers",
                rows.Select(item =>
                    new XElement("CompanyUser",
                        new XElement("User", item.User?.UserName ?? ""),
                        new XElement("Company", item.Company?.CompanyName ?? ""),
                        new XElement("Vessels", GetVesselText(item)),
                        new XElement("LastActivity", GetLastActivityText(item.User?.LastActivityAtUtc))
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(
            bytes,
            "application/xml",
            $"CompanyUsers_{DateTime.UtcNow:yyyyMMddHHmmss}.xml");
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportCompanyUsersExcel(
        [FromBody] CompanyUserQueryRequest? request)
    {
        request = NormalizeQuery(request ?? new CompanyUserQueryRequest());

        var rows = await GetFilteredCompanyUsersAsync(request);

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
                Name = "Company Users"
            });

            var headers = new[]
            {
                "User",
                "Company",
                "Vessels",
                "Last Activity"
            };

            var maxLengths = new int[headers.Length];

            var headerRow = new Row();

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.AppendChild(CreateTextCell(headers[i]));
                maxLengths[i] = headers[i].Length;
            }

            sheetData.AppendChild(headerRow);

            foreach (var item in rows)
            {
                var values = new[]
                {
                    item.User?.UserName ?? "",
                    item.Company?.CompanyName ?? "",
                    GetVesselText(item),
                    GetLastActivityText(item.User?.LastActivityAtUtc)
                };

                var row = new Row();

                for (int i = 0; i < values.Length; i++)
                {
                    row.AppendChild(CreateTextCell(values[i]));
                    maxLengths[i] = Math.Max(maxLengths[i], values[i]?.Length ?? 0);
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
            $"CompanyUsers_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    private async Task<List<CompanyUser>> GetFilteredCompanyUsersAsync(CompanyUserQueryRequest request)
    {
        var allMappings = (List<CompanyUser>)await _companyUserService.GetUsersByCompanyAsync();

        return ApplyFilters(allMappings.AsQueryable(), request).ToList();
    }

    private static IQueryable<CompanyUser> ApplyFilters(
        IQueryable<CompanyUser> query,
        CompanyUserQueryRequest request)
    {
        if (request.CompanyId.HasValue && request.CompanyId.Value > 0)
        {
            query = query.Where(x => x.CompanyId == request.CompanyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.FilterUserName))
        {
            var value = request.FilterUserName.Trim();

            query = query.Where(x =>
                x.User != null &&
                x.User.UserName != null &&
                x.User.UserName.Contains(value, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterCompany))
        {
            var value = request.FilterCompany.Trim();

            query = query.Where(x =>
                x.Company != null &&
                x.Company.CompanyName != null &&
                x.Company.CompanyName.Contains(value, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterVesselName))
        {
            var value = request.FilterVesselName.Trim();

            query = query.Where(x =>
                x.Company != null &&
                x.Company.VesselDetailList != null &&
                x.Company.VesselDetailList.Any(v =>
                    v.VesselName != null &&
                    v.VesselName.Contains(value, StringComparison.OrdinalIgnoreCase)));
        }

        return query;
    }

    private static CompanyUserQueryRequest NormalizeQuery(CompanyUserQueryRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        request.FilterUserName = request.FilterUserName?.Trim();
        request.FilterCompany = request.FilterCompany?.Trim();
        request.FilterVesselName = request.FilterVesselName?.Trim();

        return request;
    }

    private static CompanyUserDto ToDto(CompanyUser item)
    {
        return new CompanyUserDto
        {
            Id = item.Id,
            CompanyId = item.CompanyId,
            UserId = item.UserId,

            UserName = item.User?.UserName,
            Name = item.User?.Name,
            Email = item.User?.Email,

            CompanyName = item.Company?.CompanyName,
            Vessels = GetVesselText(item),

            LastActivityAtUtc = item.User?.LastActivityAtUtc,
            LastActivityText = GetLastActivityText(item.User?.LastActivityAtUtc)
        };
    }

    private static ApplicationUserOptionDto ToUserOptionDto(ApplicationUser user)
    {
        return new ApplicationUserOptionDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            IsApprove = user.IsApprove
        };
    }

    private static string GetVesselText(CompanyUser item)
    {
        if (item.Company?.VesselDetailList != null && item.Company.VesselDetailList.Any())
        {
            var names = item.Company.VesselDetailList
                .Select(v => v.VesselName)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var text = string.Join(", ", names);

            return string.IsNullOrWhiteSpace(text)
                ? "No Vessels Assigned"
                : text;
        }

        return "No Vessels Assigned";
    }

    private static string GetLastActivityText(DateTime? lastActivityAtUtc)
    {
        return lastActivityAtUtc != null
            ? lastActivityAtUtc.Value.ToLocalTime().ToString("dd-MMM-yyyy hh:mm tt")
            : "Never";
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