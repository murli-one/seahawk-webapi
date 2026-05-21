using Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Seahawk_WebAPI.Contracts.Users;
using SeaHawkService.Infrastructure.Emails;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Xml.Linq;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersApiController : ControllerBase
{
    private readonly IApplicationUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptions<SMTPSettings> _smtpSettings;
    private readonly IVesselDetailService _vesselDetailService;
    private readonly ICompanyService _companyService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICompanyUserService _companyUserService;
    private readonly IVesselUserService _vesselUserService;

    public UsersApiController(
        IVesselUserService vesselUserService,
        ICompanyUserService companyUserService,
        IVesselDetailService vesselDetailService,
        ICompanyService companyService,
        IApplicationUserService userService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IHttpContextAccessor httpContextAccessor,
        IOptions<SMTPSettings> smtpSettings,
        IUnitOfWork unitOfWork)
    {
        _vesselUserService = vesselUserService;
        _companyUserService = companyUserService;
        _vesselDetailService = vesselDetailService;
        _companyService = companyService;
        _userManager = userManager;
        _userService = userService;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _smtpSettings = smtpSettings;
        _unitOfWork = unitOfWork;
    }

    // GET: /api/users?tab=approved&search=test&page=1&pageSize=13
    [HttpGet]
    public async Task<ActionResult<UserListResponse>> GetAllUsers([FromQuery] UserListRequest request)
    {
        request.Tab = string.IsNullOrWhiteSpace(request.Tab) ? "approved" : request.Tab;

        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        var currentUserName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(currentUserName))
            return Unauthorized(new { message = "Current user not found." });

        var currentUser = await _userService.GetUserByUserNameAsync(currentUserName);

        var users = (List<ApplicationUser>)await _userService.GetAllUsersInsteadOfYours(currentUserName);

        users = request.Tab switch
        {
            "approved" => users.Where(u => u.IsApprove).ToList(),
            "unapproved" => users.Where(u => !u.IsApprove).ToList(),
            _ => users
        };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim().ToLowerInvariant();

            users = users
                .Where(u =>
                    (
                        (!string.IsNullOrEmpty(u.Name) && u.Name.ToLowerInvariant().Contains(s)) ||
                        (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLowerInvariant().Contains(s)) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.ToLowerInvariant().Contains(s))
                    )
                    && u.IsDeleted == false)
                .ToList();
        }

        var totalCount = users.Count;

        var pagedUsers = users
            .OrderBy(u => u.UserName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Ok(new UserListResponse
        {
            Users = pagedUsers.Select(ToDto).ToList(),

            UserName = currentUserName,
            Email = currentUser?.Email,
            UserRole = currentUser?.Role.ToString(),

            CurrentTab = request.Tab,
            Search = request.Search,

            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }

    // GET: /api/users/approved?search=test&page=1&pageSize=15
    [HttpGet("approved")]
    public async Task<ActionResult<UserListResponse>> GetApprovedUsers(
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

        var users = (await _userService.GetAllUsersInsteadOfYours(currentUserName))
            .Where(u => u.IsApprove && u.IsDeleted == false)
            .ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();

            users = users
                .Where(u =>
                    (!string.IsNullOrEmpty(u.Name) && u.Name.ToLowerInvariant().Contains(s)) ||
                    (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLowerInvariant().Contains(s)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLowerInvariant().Contains(s)) ||
                    (!string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.ToLowerInvariant().Contains(s)))
                .ToList();
        }

        var totalCount = users.Count;

        var pagedUsers = users
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new UserListResponse
        {
            Users = pagedUsers.Select(ToDto).ToList(),

            UserName = currentUserName,
            Email = currentUser?.Email,
            UserRole = currentUser?.Role.ToString(),

            CurrentTab = "approved",
            Search = search,

            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }


    // GET: /api/users/unapproved?search=test&page=1&pageSize=15
    [HttpGet("unapproved")]
    public async Task<ActionResult<UserListResponse>> GetUnapprovedUsers(
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

        var users = (await _userService.GetAllUsersInsteadOfYours(currentUserName))
            .Where(u => !u.IsApprove && u.IsDeleted == false)
            .ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();

            users = users
                .Where(u =>
                    (!string.IsNullOrEmpty(u.Name) && u.Name.ToLowerInvariant().Contains(s)) ||
                    (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLowerInvariant().Contains(s)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLowerInvariant().Contains(s)) ||
                    (!string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.ToLowerInvariant().Contains(s)))
                .ToList();
        }

        var totalCount = users.Count;

        var pagedUsers = users
            .OrderBy(u => u.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new UserListResponse
        {
            Users = pagedUsers.Select(ToDto).ToList(),

            UserName = currentUserName,
            Email = currentUser?.Email,
            UserRole = currentUser?.Role.ToString(),

            CurrentTab = "unapproved",
            Search = search,

            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }
    // GET: /api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { message = "User id is required." });

        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(ToDto(user));
    }

    // GET: /api/users/{id}/edit-options
    [HttpGet("{id}/edit-options")]
    public async Task<ActionResult<UserEditOptionsResponse>> GetUserEditOptions(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { message = "User id is required." });

        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "User not found." });

        var response = new UserEditOptionsResponse
        {
            User = ToDto(user),
            IsApprovedUser = user.IsApprove
        };

        if (!user.IsApprove)
        {
            var vessels = await _vesselDetailService.GetAllAsync();
            response.Vessels = vessels
                .OrderBy(x => x.VesselName)
                .Select(x => new SelectOptionDto
                {
                    Value = x.Id.ToString(),
                    Text = x.VesselName ?? ""
                })
                .ToList();

            var companies = await _companyService.GetAllAsync();
            response.Companies = companies
                .OrderBy(x => x.CompanyName)
                .Select(x => new SelectOptionDto
                {
                    Value = x.Id.ToString(),
                    Text = x.CompanyName ?? ""
                })
                .ToList();
        }

        return Ok(response);
    }

    // GET: /api/users/my-account
    [HttpGet("my-account")]
    public async Task<ActionResult<UserDto>> MyAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(new { message = "Current user id not found." });

        var user = await _userService.GetByIdAsync(userId);

        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(ToDto(user));
    }

    // POST: /api/users
    // POST: /api/users
    // POST: /api/users
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        if (request == null)
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Invalid user data."
            });

        if (string.IsNullOrWhiteSpace(request.UserName))
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Username is required."
            });

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Email is required."
            });

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Password is required."
            });

        if (request.Password != request.ConfirmPassword)
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Passwords do not match."
            });

        var selectedRole = (Role)request.SelectedRole;

        if (selectedRole != Role.SystemAdmin &&
            selectedRole != Role.ManagementUser &&
            selectedRole != Role.VesselUser)
        {
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Please select a valid role."
            });
        }

        if (selectedRole == Role.ManagementUser && !request.SelectedCompanyId.HasValue)
        {
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Please select a company."
            });
        }

        if (selectedRole == Role.VesselUser && !request.SelectedVesselId.HasValue)
        {
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Please select a vessel."
            });
        }

        var userName = request.UserName.Trim();
        var email = request.Email.Trim();

        var existingByUserName = await _userManager.FindByNameAsync(userName);
        if (existingByUserName != null)
        {
            return Conflict(new UserApiResponse
            {
                Success = false,
                Message = "A user with this username already exists."
            });
        }

        var existingByEmail = await _userManager.FindByEmailAsync(email);
        if (existingByEmail != null)
        {
            return Conflict(new UserApiResponse
            {
                Success = false,
                Message = "This email already exists. Please use another email ID."
            });
        }

        var newUser = new ApplicationUser
        {
            UserName = userName,
            Name = request.Name?.Trim(),
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = request.PhoneNumber?.Trim(),

            // Admin-created user is approved immediately
            Role = selectedRole,
            IsApprove = true,

            CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                DateTime.UtcNow,
                "UTC",
                "Central Standard Time"),

            Address = "",
            CompanyName = ""
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                success = false,
                message = result.Errors.Any()
                    ? result.Errors.FirstOrDefault()?.Description
                    : "User was not successfully created.",
                errors = result.Errors.Select(x => x.Description).ToList()
            });
        }

        string claimType = selectedRole switch
        {
            Role.SystemAdmin => "SystemAdmin",
            Role.ManagementUser => "ManagementUser",
            Role.VesselUser => "VesselUser",
            _ => ""
        };

        if (!string.IsNullOrWhiteSpace(claimType))
        {
            var claimResult = await _userManager.AddClaimAsync(
                newUser,
                new Claim(claimType, "True"));

            if (!claimResult.Succeeded)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "User created, but role claim assignment failed.",
                    errors = claimResult.Errors.Select(x => x.Description).ToList()
                });
            }
        }

        if (selectedRole == Role.ManagementUser)
        {
            var assigned = await _companyUserService.AssignUserToCompanyAsync(
                request.SelectedCompanyId!.Value,
                newUser.Id);

            if (!assigned)
            {
                return BadRequest(new UserApiResponse
                {
                    Success = false,
                    Message = "User created, but company assignment failed or already exists."
                });
            }
        }
        else if (selectedRole == Role.VesselUser)
        {
            var assigned = await _vesselUserService.AssignUserToVesselAsync(
                request.SelectedVesselId!.Value,
                newUser.Id);

            if (!assigned)
            {
                return BadRequest(new UserApiResponse
                {
                    Success = false,
                    Message = "User created, but vessel assignment failed or already exists."
                });
            }
        }

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = newUser.Id },
            ToDto(newUser));
    }

    // PUT: /api/users/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<UserApiResponse>> UpdateUser(string id, [FromBody] UpdateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new UserApiResponse { Success = false, Message = "User id is required." });

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound(new UserApiResponse { Success = false, Message = "User not found." });

        user.UserName = !string.IsNullOrWhiteSpace(request.UserName)
            ? request.UserName
            : user.UserName;

        user.Name = !string.IsNullOrWhiteSpace(request.Name)
            ? request.Name
            : user.Name;

        user.Email = !string.IsNullOrWhiteSpace(request.Email)
            ? request.Email
            : user.Email;

        user.PhoneNumber = !string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? request.PhoneNumber
            : user.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return BadRequest(new
            {
                success = false,
                message = "User update failed.",
                errors = updateResult.Errors.Select(x => x.Description).ToList()
            });
        }

        if (!user.IsApprove)
        {
            var selectedRole = (Role)request.SelectedRole;

            if (selectedRole != Role.ManagementUser && selectedRole != Role.VesselUser)
            {
                return BadRequest(new UserApiResponse
                {
                    Success = false,
                    Message = "Please select a valid role."
                });
            }

            if (selectedRole == Role.ManagementUser && !request.SelectedCompanyId.HasValue)
            {
                return BadRequest(new UserApiResponse
                {
                    Success = false,
                    Message = "Please select a company."
                });
            }

            if (selectedRole == Role.VesselUser && !request.SelectedVesselId.HasValue)
            {
                return BadRequest(new UserApiResponse
                {
                    Success = false,
                    Message = "Please select a vessel."
                });
            }

            user.Role = selectedRole;
            user.IsApprove = true;

            string desiredType = user.Role switch
            {
                Role.ManagementUser => "ManagementUser",
                Role.VesselUser => "VesselUser",
                _ => ""
            };

            if (!string.IsNullOrEmpty(desiredType))
            {
                var addClaimResult = await _userManager.AddClaimAsync(user, new Claim(desiredType, "True"));

                if (!addClaimResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Role claim update failed.",
                        errors = addClaimResult.Errors.Select(x => x.Description).ToList()
                    });
                }
            }

            var approveUpdate = await _userManager.UpdateAsync(user);

            if (!approveUpdate.Succeeded)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "User approval update failed.",
                    errors = approveUpdate.Errors.Select(x => x.Description).ToList()
                });
            }

            if (user.Role == Role.ManagementUser)
            {
                await _companyUserService.AssignUserToCompanyAsync(request.SelectedCompanyId!.Value, user.Id);
            }
            else if (user.Role == Role.VesselUser)
            {
                await _vesselUserService.AssignUserToVesselAsync(request.SelectedVesselId!.Value, user.Id);
            }
        }

        if (user.Role != Role.SystemAdmin)
        {
            if (!string.IsNullOrWhiteSpace(request.Password) &&
                !string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest(new UserApiResponse
                    {
                        Success = false,
                        Message = "Confirm Password does not match the New Password."
                    });
                }

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, request.Password);

                var passResult = await _userManager.UpdateAsync(user);

                if (!passResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Password update failed.",
                        errors = passResult.Errors.Select(x => x.Description).ToList()
                    });
                }
            }
        }
        else if (user.Role == Role.SystemAdmin && !string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new UserApiResponse
                {
                    Success = false,
                    Message = "Confirm Password does not match the New Password."
                });
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passResult = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);

            if (!passResult.Succeeded)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Password reset failed.",
                    errors = passResult.Errors.Select(x => x.Description).ToList()
                });
            }

            var loginUrl = $"{Request.Scheme}://{Request.Host}/Account/Login";

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                await SendPasswordChangedEmailAsync(
                    toEmail: user.Email,
                    fullName: user.Name ?? "User",
                    newPassword: request.Password,
                    loginUrl: loginUrl);
            }
        }

        return Ok(new UserApiResponse
        {
            Success = true,
            Message = "User updated successfully."
        });
    }

    // DELETE: /api/users/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<UserApiResponse>> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound(new UserApiResponse { Success = false, Message = "User not found." });

        try
        {
            var companyMappings = await _unitOfWork.CompanyUser.GetAllAsync(x => x.UserId == user.Id);
            var vesselMappings = await _unitOfWork.VesselUser.GetAllAsync(x => x.UserId == user.Id);

            await _unitOfWork.SampleCollections.DeleteUserAllEnteries(id);
            await _unitOfWork.SamplingKit.DeleteUserAllEnteries(id);

            if (companyMappings != null && companyMappings.Any())
            {
                foreach (var mapping in companyMappings)
                    await _unitOfWork.CompanyUser.RemoveAsync(mapping);
            }

            if (vesselMappings != null && vesselMappings.Any())
            {
                foreach (var mapping in vesselMappings)
                    await _unitOfWork.VesselUser.RemoveAsync(mapping);
            }

            user.IsDeleted = true;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest(new
                {
                    success = false,
                    message = string.Join(", ", updateResult.Errors.Select(e => e.Description))
                });
            }

            await _unitOfWork.SaveAsync();

            return Ok(new UserApiResponse
            {
                Success = true,
                Message = "User marked as deleted successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new UserApiResponse
            {
                Success = false,
                Message = $"Delete failed: {ex.Message}"
            });
        }
    }

    // POST: /api/users/{id}/change-password
    [HttpPost("{id}/change-password")]
    public async Task<ActionResult<UserApiResponse>> ChangePassword(string id, [FromBody] ChangePasswordRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Passwords do not match."
            });
        }

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound(new UserApiResponse { Success = false, Message = "User not found." });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                success = false,
                message = "Password reset failed.",
                errors = result.Errors.Select(x => x.Description).ToList()
            });
        }

        var verifies = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!verifies)
        {
            return BadRequest(new UserApiResponse
            {
                Success = false,
                Message = "Password reset did not persist for this user. Check duplicate users/DB."
            });
        }

        return Ok(new UserApiResponse
        {
            Success = true,
            Message = "Password changed successfully."
        });
    }

    // POST: /api/users/{id}/approve
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<UserApiResponse>> ApproveUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new UserApiResponse { Success = false, Message = "User id is required." });

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null || currentUser.Role != Role.SystemAdmin)
            return Forbid();

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound(new UserApiResponse { Success = false, Message = "User not found." });

        user.IsApprove = true;
        await _userManager.UpdateAsync(user);

        return Ok(new UserApiResponse
        {
            Success = true,
            Message = $"User '{user.UserName}' has been approved."
        });
    }

    // GET: /api/users/export-xml
    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportXml()
    {
        var users = await _userService.GetAllAsync();

        var xml = new XDocument(
            new XElement("Users",
                users.Select(u =>
                    new XElement("User",
                        new XElement("Id", u.Id),
                        new XElement("Name", u.Name),
                        new XElement("UserName", u.UserName),
                        new XElement("PhoneNumber", u.PhoneNumber),
                        new XElement("Email", u.Email),
                        new XElement("Role", u.Role)
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(bytes, "application/xml", "Users.xml");
    }

    // POST: /api/users/export-excel
    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var users = await _userService.GetAllAsync();

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
                Name = "Users"
            });

            var headers = new[] { "Id", "Name", "UserName", "PhoneNumber", "Email", "Role" };

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

            foreach (var user in users)
            {
                var row = new Row();

                string[] values =
                {
                    user.Id.ToString(),
                    user.Name ?? "",
                    user.UserName ?? "",
                    user.PhoneNumber ?? "",
                    user.Email ?? "",
                    user.Role.ToString()
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
            "Users.xlsx");
    }

    private (string Ip, string UserAgent) GetRequestInfo()
    {
        var ctx = _httpContextAccessor.HttpContext;
        var ip = ctx?.Connection?.RemoteIpAddress?.ToString() ?? "N/A";
        var userAgent = ctx?.Request?.Headers["User-Agent"].ToString() ?? "N/A";

        return (ip, userAgent);
    }

    private async Task SendPasswordChangedEmailAsync(
        string toEmail,
        string fullName,
        string newPassword,
        string? loginUrl = null)
    {
        var smtpHost = _smtpSettings.Value.Host;
        int smtpPort = Convert.ToInt32(_smtpSettings.Value.Port);
        var smtpUser = _smtpSettings.Value.Username;
        var smtpPass = _smtpSettings.Value.Password;
        var fromEmail = _smtpSettings.Value.FromEmail;
        var displayName = _smtpSettings.Value.DisplayName;

        var safeName = HttpUtility.HtmlEncode(string.IsNullOrWhiteSpace(fullName) ? "User" : fullName);
        var safeEmail = HttpUtility.HtmlEncode(toEmail);
        var safePassword = HttpUtility.HtmlEncode(newPassword);
        var safeLoginUrl = HttpUtility.HtmlEncode(loginUrl ?? "");

        var subject = "Your SeaHawk account password has been updated";

        var body = $@"
<!DOCTYPE html>
<html>
  <body style=""margin:0;padding:24px;font-family:Segoe UI,Arial,sans-serif;
               font-size:14px;color:#111827;background-color:#f3f4f6;"">

    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%""
           style=""max-width:640px;margin:0 auto;background-color:#ffffff;
                  border-radius:10px;border:1px solid #e5e7eb;overflow:hidden;"">

      <tr>
         <td style=""padding:20px 24px;border-bottom:1px solid #e5e7eb;background:#ffffff;"">

    <div style=""text-align:center;margin-bottom:12px;"">
      <img src=""https://ak.mt.cisinlive.com/ddts/images/logo1.png""
           alt=""SeaHawk Services""
           style=""max-width:180px;height:auto;display:block;margin:0 auto;"" />
    </div>

          <h2 style=""margin:10px;font-size:18px;color:#198754;"">
            Password Updated
          </h2>
          <p style=""margin:6px 0 0 0;color:#6b7280;"">
            Your SeaHawk Services account password has been updated by an administrator.
          </p>
        </td>
      </tr>

      <tr>
        <td style=""padding:18px 24px 22px 24px;"">

          <p style=""margin:0 0 12px 0;"">Hi <strong>{safeName}</strong>,</p>

          <p style=""margin:0 0 14px 0;color:#374151;"">
            Your account password has been reset. Please use the details below to log in.
          </p>

          <div style=""padding:14px;border-radius:8px;background:#f9fafb;border:1px solid #e5e7eb;"">
            <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse:collapse;"">
              <tr>
                <td style=""padding:6px 0;width:140px;color:#6b7280;"">Username</td>
                <td style=""padding:6px 0;""><strong>{safeEmail}</strong></td>
              </tr>
              <tr>
                <td style=""padding:6px 0;color:#6b7280;"">New Password</td>
                <td style=""padding:6px 0;"">
                  <span style=""display:inline-block;padding:8px 10px;border-radius:6px;
                               background:#ffffff;border:1px dashed #d1d5db;font-family:Consolas,monospace;"">
                    {safePassword}
                  </span>
                </td>
              </tr>
            </table>
          </div>

          {(string.IsNullOrWhiteSpace(loginUrl) ? "" : $@"
          <div style=""margin:16px 0 0 0;"">
            <a href=""{safeLoginUrl}""
               style=""display:inline-block;background:#198754;color:#ffffff;text-decoration:none;
                      padding:10px 14px;border-radius:8px;font-weight:600;"">
              Login to SeaHawk
            </a>
          </div>
          <p style=""margin:10px 0 0 0;color:#6b7280;font-size:12px;"">
            If the button doesn’t work, copy and paste this link into your browser:<br/>
            <span style=""word-break:break-all;"">{safeLoginUrl}</span>
          </p>
          ")}

          <hr style=""border:none;border-top:1px solid #e5e7eb;margin:18px 0;"" />

          <p style=""margin:0;color:#6b7280;font-size:12px;"">
            For security, please change this password after logging in.
            If you did not expect this change, contact support immediately.
          </p>

        </td>
      </tr>

      <tr>
        <td style=""padding:14px 24px;background:#f9fafb;border-top:1px solid #e5e7eb;"">
          <p style=""margin:0;color:#9ca3af;font-size:12px;"">
            © {DateTime.UtcNow.Year} SeaHawk Services. This is an automated email.
          </p>
        </td>
      </tr>

    </table>
  </body>
</html>";

        using var mail = new MailMessage();
        mail.From = new MailAddress(fromEmail, displayName);
        mail.To.Add(new MailAddress(toEmail));
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        using var client = new SmtpClient(smtpHost, smtpPort);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(smtpUser, smtpPass);

        await client.SendMailAsync(mail);
    }

    private static UserDto ToDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,

            Role = user.Role,
            RoleText = user.Role.ToString(),

            IsApprove = user.IsApprove,
            IsDeleted = user.IsDeleted,

            CreatedAt = user.CreatedAt,

            Address = user.Address,
            CompanyName = user.CompanyName
        };
    }
}