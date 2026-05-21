using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.VesselUsers;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Xml.Linq;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/vessel-users")]
[Authorize]
public class VesselUsersApiController : ControllerBase
{
    private readonly IVesselService _vesselService;
    private readonly IApplicationUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVesselUserService _vesselUserService;

    public VesselUsersApiController(
        IVesselService vesselService,
        IApplicationUserService userService,
        UserManager<ApplicationUser> userManager,
        IVesselUserService vesselUserService)
    {
        _vesselService = vesselService;
        _userService = userService;
        _userManager = userManager;
        _vesselUserService = vesselUserService;
    }

    // GET: /api/vessel-users?page=1&pageSize=15&filterUserName=test&filterVesselName=vessel
    [HttpGet]
    public async Task<ActionResult<VesselUserListResponse>> GetVesselUsers(
        [FromQuery] VesselUserListRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        var userName = User.Identity?.Name;
        var userDetails = await _userService.GetUserByUserNameAsync(userName);

        var vessels = await _vesselService.GetAllAsync();
        var users = await _userService.GetAllUsersInsteadOfYours(userName);

        var vesselUsers = (await _vesselUserService.GetVesselUserAsync()).ToList();

        IEnumerable<VesselUser> filtered = vesselUsers;

        if (!string.IsNullOrWhiteSpace(request.FilterUserName))
        {
            var name = request.FilterUserName.Trim();

            filtered = filtered.Where(x =>
                x.User != null &&
                !string.IsNullOrEmpty(x.User.UserName) &&
                x.User.UserName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterVesselName))
        {
            var vesselName = request.FilterVesselName.Trim();

            filtered = filtered.Where(x =>
                x.VesselDetail != null &&
                !string.IsNullOrEmpty(x.VesselDetail.VesselName) &&
                x.VesselDetail.VesselName.Contains(vesselName, StringComparison.OrdinalIgnoreCase));
        }

        var filteredList = filtered.ToList();

        var groupedUsers = filteredList
            .GroupBy(x => x.UserId)
            .Select(g => g.First())
            .ToList();

        var totalCount = groupedUsers.Count;

        var pagedGroupedUsers = groupedUsers
            .OrderBy(x => x.User?.UserName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var pageUserIds = pagedGroupedUsers
            .Select(x => x.UserId)
            .ToHashSet();

        var userVesselNames = filteredList
            .Where(x => pageUserIds.Contains(x.UserId))
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => string.Join(", ",
                    g.Select(x => x.VesselDetail?.VesselName)
                        .Where(vn => !string.IsNullOrWhiteSpace(vn))
                        .Distinct()
                        .OrderBy(vn => vn))
            );

        var rows = pagedGroupedUsers
            .Select(x => ToRowDto(
                x,
                userVesselNames.TryGetValue(x.UserId, out var vesselsText) ? vesselsText : ""))
            .ToList();

        var response = new VesselUserListResponse
        {
            VesselUserGrouped = rows,

            Vessels = vessels
                .Select(v => new SelectOptionDto
                {
                    Value = v.Id.ToString(),
                    Text = v.VesselName ?? ""
                })
                .ToList(),

            ApplicationUsers = users
                .Select(u => new SelectOptionDto
                {
                    Value = u.Id,
                    Text = u.UserName ?? u.Email ?? u.Id
                })
                .ToList(),

            UserVesselNames = userVesselNames,

            FilterUserName = request.FilterUserName,
            FilterVesselName = request.FilterVesselName,

            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,

            SelectedUser = userDetails?.UserName ?? "",
            SelectedUserEmail = userDetails?.Email ?? "",
            SelectedUserRole = userDetails?.Role.ToString() ?? ""
        };

        return Ok(response);
    }

    // GET: /api/vessel-users/unassigned-users?search=test&page=1&pageSize=15
    [HttpGet("unassigned-users")]
    public async Task<ActionResult<UnassignedVesselUsersResponse>> GetUnassignedVesselUsers(
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

        var vesselUsers = (await _vesselUserService.GetVesselUserAsync()).ToList();

        var assignedUserIds = vesselUsers
            .Select(x => x.UserId)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet();

        var query = applicationUsers
            .Where(u =>
                !assignedUserIds.Contains(u.Id) &&
                u.IsApprove == true &&
                u.IsDeleted == false)
            .ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var value = search.Trim();

            query = query
                .Where(u =>
                    (!string.IsNullOrEmpty(u.UserName) &&
                     u.UserName.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(u.Name) &&
                     u.Name.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(u.Email) &&
                     u.Email.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(u.PhoneNumber) &&
                     u.PhoneNumber.Contains(value, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        var totalCount = query.Count;

        var users = query
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new SelectOptionDto
            {
                Value = u.Id,
                Text = u.UserName ?? u.Email ?? u.Id
            })
            .ToList();

        return Ok(new UnassignedVesselUsersResponse
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = pageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / pageSize),

            Search = search,

            SelectedUser = currentUser?.UserName ?? "",
            SelectedUserEmail = currentUser?.Email ?? "",
            SelectedUserRole = currentUser?.Role.ToString() ?? "",

            Items = users
        });
    }

    // GET: /api/vessel-users/{id}/edit
    // id = VesselUser mapping table id.
    [HttpGet("{id:int}/edit")]
    public async Task<ActionResult<VesselUserEditResponse>> GetEditVesselUser(int id)
    {
        var vesselUsers = await _vesselUserService.GetVesselUserAsync();
        var selectedMapping = vesselUsers.FirstOrDefault(x => x.Id == id);

        if (selectedMapping == null)
            return NotFound(new { message = $"VesselUser mapping not found for id: {id}" });

        var user = await _userManager.FindByIdAsync(selectedMapping.UserId);

        if (user == null)
            return NotFound(new { message = $"User not found for userId: {selectedMapping.UserId}" });

        var userMappings = vesselUsers
            .Where(x => x.UserId == user.Id)
            .ToList();

        var assigned = userMappings
            .Where(x => x.VesselDetail != null)
            .Select(x => new AssignedVesselDto
            {
                VesselUserMappingId = x.Id,
                VesselId = x.VesselDetail!.Id,
                VesselName = x.VesselDetail.VesselName,
                IMONumber = x.VesselDetail.IMONumber
            })
            .ToList();

        var allVessels = await _vesselService.GetAllAsync();
        var assignedVesselIds = assigned.Select(a => a.VesselId).ToHashSet();

        var response = new VesselUserEditResponse
        {
            VesselUserId = id,
            UserId = user.Id,
            UserName = user.UserName ?? "",
            Name = user.Name ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber,

            Role = user.Role,
            RoleText = user.Role.ToString(),

            IsApprove = user.IsApprove,
            LastLoginAtUtc = user.LastLoginAtUtc,
            LastActivityAtUtc = user.LastActivityAtUtc,

            AssignedVessels = assigned,

            VesselOptions = allVessels
                .Where(v => !assignedVesselIds.Contains(v.Id))
                .Select(v => new SelectOptionDto
                {
                    Value = v.Id.ToString(),
                    Text = v.VesselName ?? ""
                })
                .ToList()
        };

        return Ok(response);
    }

    // PUT: /api/vessel-users/{id}
    // id = VesselUser mapping table id. UserId is also sent in body.
    [HttpPut("{id:int}")]
    public async Task<ActionResult<VesselUserApiResponse>> UpdateVesselUser(
        int id,
        [FromBody] UpdateVesselUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return BadRequest(new VesselUserApiResponse
            {
                Success = false,
                Message = "UserId is required."
            });
        }

        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user == null)
            return NotFound(new VesselUserApiResponse { Success = false, Message = "User not found." });

        user.Name = request.Name;
        user.Email = request.Email;
        user.NormalizedEmail = request.Email?.ToUpperInvariant();
        user.PhoneNumber = request.PhoneNumber;
        user.Role = request.Role;
        user.IsApprove = request.IsApprove;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                success = false,
                message = "Failed to update vessel user details.",
                errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        return Ok(new VesselUserApiResponse
        {
            Success = true,
            Message = "Vessel user updated successfully."
        });
    }

    // POST: /api/vessel-users/assign
    [HttpPost("assign")]
    public async Task<ActionResult<VesselUserApiResponse>> Assign([FromBody] AssignVesselUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SelectedUser) ||
            request.SelectedVesselIds == null ||
            !request.SelectedVesselIds.Any())
        {
            return BadRequest(new VesselUserApiResponse
            {
                Success = false,
                Message = "Please select both user and at least one vessel."
            });
        }

        int successCount = 0;
        int duplicateOrFailCount = 0;

        foreach (var vesselId in request.SelectedVesselIds.Distinct())
        {
            var ok = await _vesselUserService.AssignUserToVesselAsync(vesselId, request.SelectedUser);

            if (ok)
                successCount++;
            else
                duplicateOrFailCount++;
        }

        return Ok(new VesselUserApiResponse
        {
            Success = successCount > 0,
            SuccessCount = successCount,
            FailedCount = duplicateOrFailCount,
            Message = BuildAssignMessage(successCount, duplicateOrFailCount)
        });
    }

    // POST: /api/vessel-users/add-vessels
    [HttpPost("add-vessels")]
    public async Task<ActionResult<VesselUserApiResponse>> AddVesselToUser([FromBody] AddVesselToUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) ||
            request.SelectedVesselIds == null ||
            !request.SelectedVesselIds.Any())
        {
            return BadRequest(new VesselUserApiResponse
            {
                Success = false,
                Message = "Please select at least one vessel."
            });
        }

        int successCount = 0;
        int duplicateOrFailCount = 0;

        foreach (var vesselId in request.SelectedVesselIds.Distinct())
        {
            var ok = await _vesselUserService.AssignUserToVesselAsync(vesselId, request.UserId);

            if (ok)
                successCount++;
            else
                duplicateOrFailCount++;
        }

        return Ok(new VesselUserApiResponse
        {
            Success = successCount > 0,
            SuccessCount = successCount,
            FailedCount = duplicateOrFailCount,
            Message = BuildAssignMessage(successCount, duplicateOrFailCount)
        });
    }

    // DELETE: /api/vessel-users/{vesselUserId}?userId=...
    [HttpDelete("{vesselUserId:int}")]
    public async Task<ActionResult<VesselUserApiResponse>> Remove(
        int vesselUserId,
        [FromQuery] string? userId = null)
    {
        await _vesselUserService.RemoveUserFromVesselAsync(vesselUserId);

        return Ok(new VesselUserApiResponse
        {
            Success = true,
            Message = "Vessel removed successfully."
        });
    }

    // POST: /api/vessel-users/remove-from-user
    [HttpPost("remove-from-user")]
    public async Task<ActionResult<VesselUserApiResponse>> RemoveVesselFromUser(
        [FromBody] RemoveVesselFromUserRequest request)
    {
        await _vesselUserService.RemoveUserFromVesselAsync(request.VesselUserMappingId);

        int? nextEditId = null;

        if (request.EditId == request.VesselUserMappingId)
        {
            var vesselUsers = await _vesselUserService.GetVesselUserAsync();
            var selectedMapping = vesselUsers.FirstOrDefault(x => x.UserId == request.UserId);

            if (selectedMapping != null)
                nextEditId = selectedMapping.Id;
        }
        else
        {
            nextEditId = request.EditId;
        }

        return Ok(new VesselUserApiResponse
        {
            Success = true,
            Message = "Vessel removed successfully.",
            NextEditId = nextEditId
        });
    }

    // POST: /api/vessel-users/export-xml
    [HttpPost("export-xml")]
    public async Task<IActionResult> ExportVesselUsersXml()
    {
        var vesselUsers = await _vesselUserService.GetVesselUserAsync();

        var groupedList = vesselUsers
            .GroupBy(x => x.UserId)
            .Select(g => g.First())
            .ToList();

        var userVesselNames = vesselUsers
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => string.Join(", ",
                    g.Select(x => x.VesselDetail?.VesselName)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .Distinct()
                        .OrderBy(x => x))
            );

        var xml = new XDocument(
            new XElement("VesselUsers",
                groupedList.Select(item =>
                    new XElement("VesselUser",
                        new XElement("User", item.User?.UserName ?? ""),
                        new XElement("Vessels",
                            userVesselNames.ContainsKey(item.UserId)
                                ? userVesselNames[item.UserId]
                                : "")
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(bytes, "application/xml", "VesselUsers.xml");
    }

    // POST: /api/vessel-users/export-excel
    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportVesselUsersExcel()
    {
        var vesselUsers = await _vesselUserService.GetVesselUserAsync();

        var groupedList = vesselUsers
            .GroupBy(x => x.UserId)
            .Select(g => g.First())
            .ToList();

        var userVesselNames = vesselUsers
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => string.Join(", ",
                    g.Select(x => x.VesselDetail?.VesselName)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .Distinct()
                        .OrderBy(x => x))
            );

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
                Name = "Vessel Users"
            });

            var headers = new[] { "User", "Vessels" };

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

            foreach (var item in groupedList)
            {
                var vesselsText = userVesselNames.ContainsKey(item.UserId)
                    ? userVesselNames[item.UserId]
                    : "";

                string[] values =
                {
                    item.User?.UserName ?? "",
                    vesselsText
                };

                var row = new Row();

                for (int i = 0; i < values.Length; i++)
                {
                    row.AppendChild(new Cell
                    {
                        CellValue = new CellValue(values[i]),
                        DataType = CellValues.String
                    });

                    maxLengths[i] = Math.Max(maxLengths[i], values[i]?.Length ?? 0);
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
            "VesselUsers.xlsx");
    }

    private static VesselUserRowDto ToRowDto(VesselUser vesselUser, string vesselsText)
    {
        return new VesselUserRowDto
        {
            Id = vesselUser.Id,

            UserId = vesselUser.UserId,
            UserName = vesselUser.User?.UserName,
            Name = vesselUser.User?.Name,
            Email = vesselUser.User?.Email,

            VesselDetailId = vesselUser.VesselDetailId,
            VesselName = vesselUser.VesselDetail?.VesselName,
            IMONumber = vesselUser.VesselDetail?.IMONumber,

            VesselsText = vesselsText
        };
    }

    private static string BuildAssignMessage(int successCount, int duplicateOrFailCount)
    {
        var messages = new List<string>();

        if (successCount > 0)
            messages.Add($"{successCount} vessel(s) assigned successfully.");

        if (duplicateOrFailCount > 0)
            messages.Add($"{duplicateOrFailCount} vessel(s) were already assigned or failed.");

        return messages.Count > 0
            ? string.Join(" ", messages)
            : "No vessels were assigned.";
    }
}