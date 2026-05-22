using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using SeaHawkService.Application.Contract;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using Seahawk_WebAPI.Contracts.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using static SeaHawkServices.Domain.Entities.Enums;
using SeaHawkServices.Application.Contract;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string LoginSessionCookie = "SEAHWK_LOGIN_SESSION";

    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserLoginHistoryService _userLoginHistoryService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IEmailSender emailSender,
        IUserLoginHistoryService userLoginHistoryService)
    {
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _userLoginHistoryService = userLoginHistoryService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return Unauthorized(new { message = "Username or password is wrong." });
        }

        var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        var (ip, ua) = GetRequestInfo();

        if (!passwordCheck.Succeeded)
        {
            await _userLoginHistoryService.LogAsync(user, ip, ua, false);
            return Unauthorized(new { message = "Username or password is wrong." });
        }

        if (user.IsApprove != true)
        {
            await _userLoginHistoryService.LogAsync(user, ip, ua, false);

            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "Your user is not approved yet."
            });
        }

        await _signInManager.SignInAsync(user, request.RememberMe);
        user.LastLoginAtUtc = DateTime.UtcNow;
        user.LastActivityAtUtc = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var sessionId = await _userLoginHistoryService.LogAsync(user, ip, ua, true);
        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            Response.Cookies.Append(LoginSessionCookie, sessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(request.RememberMe ? 30 : 1),
                Path = "/"
            });
        }

        var token = GenerateAccessToken(user);
        return Ok(new
        {
            message = "Login successful.",
            tokenType = "Bearer",
            accessToken = token,
            user = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Name,
                role = user.Role.ToString()
            }
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingByUserName = await _userManager.FindByNameAsync(request.UserName.Trim());
        if (existingByUserName != null)
        {
            return Conflict(new { message = "A user with this username already exists." });
        }

        var existingByEmail = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (existingByEmail != null)
        {
            return Conflict(new { message = "This email already exists. Please use another email ID." });
        }
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new
            {
                message = "Passwords do not match."
            });
        }

        var newUser = new ApplicationUser
        {
            UserName = request.UserName.Trim(),
            Name = request.FullName?.Trim(),
            Email = request.Email.Trim(),
            EmailConfirmed = true,
            PhoneNumber = request.PhoneNumber,
            CompanyName = request.CompanyName,
            Address = request.Address,
            Role = Role.Pending,
            IsApprove = false,
            CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                DateTime.UtcNow,
                "UTC",
                "Central Standard Time")
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "User registration failed.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new
        {
            message = "Registration submitted for admin approval. You can log in once approved."
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            user.LastActivityAtUtc = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        if (Request.Cookies.TryGetValue(LoginSessionCookie, out var sessionId) &&
            !string.IsNullOrWhiteSpace(sessionId))
        {
            await _userLoginHistoryService.MarkLogoutAsync(sessionId, "Manual");
            Response.Cookies.Delete(LoginSessionCookie, new CookieOptions
            {
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
        }

        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully." });
    }



    // POST: /api/auth/check-token-expiry
    [HttpPost("check-token-expiry")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckTokenExpiryAndRefresh(
        [FromBody] TokenRefreshCheckRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Token))
        {
            return BadRequest(new
            {
                message = "Token is required."
            });
        }

        JwtSecurityToken jwtToken;

        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(request.Token);
        }
        catch
        {
            return Unauthorized(new
            {
                message = "Invalid token format."
            });
        }

        var tokenExpiryUtc = jwtToken.ValidTo;

        // Token is still valid
        if (tokenExpiryUtc > DateTime.UtcNow)
        {
            return Ok(new
            {
                isExpired = false,
                message = "Token is not expired.",
                tokenType = "Bearer",
                accessToken = request.Token,
                expiresAtUtc = tokenExpiryUtc
            });
        }

        ClaimsPrincipal principal;

        try
        {
            // Validate token signature but ignore expiry
            principal = GetPrincipalFromExpiredToken(request.Token);
        }
        catch
        {
            return Unauthorized(new
            {
                isExpired = true,
                message = "Token is expired and invalid."
            });
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new
            {
                isExpired = true,
                message = "Invalid token payload."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Unauthorized(new
            {
                isExpired = true,
                message = "User not found."
            });
        }

        if (user.IsApprove != true)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                isExpired = true,
                message = "Your user is not approved yet."
            });
        }

        user.LastActivityAtUtc = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var newToken = GenerateAccessToken(user);

        return Ok(new
        {
            isExpired = true,
            message = "Token was expired. New token generated successfully.",
            tokenType = "Bearer",
            accessToken = newToken,
            user = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Name,
                role = user.Role.ToString()
            }
        });
    }



    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.Name,
            user.Role
        });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Ok(new { message = "If an account exists for this email, a reset link has been sent." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var resetLink = $"{request.ClientResetUrl}?token={encodedToken}&email={Uri.EscapeDataString(user.Email!)}";

        var htmlBody = $@"
Hello {user.Name ?? user.UserName},<br/><br/>
You requested to reset your password for the SeaHawk Client Portal.<br/><br/>
Please click the link below to set a new password:<br/>
<a href='{HtmlEncoder.Default.Encode(resetLink)}'>Reset Password</a><br/><br/>
If you did not request this, you can safely ignore this email.";

       
        await _emailSender.SendEmailAsync(user.Email!, "Password Reset - SeaHawk Services", htmlBody);
        return Ok(new { message = "If an account exists for this email, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new
            {
                message = "Passwords do not match."
            });
        }
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Ok(new { message = "Password has been reset successfully." });
        }

        string decodedToken;
        try
        {
            var decodedBytes = WebEncoders.Base64UrlDecode(request.Token);
            decodedToken = Encoding.UTF8.GetString(decodedBytes);
        }
        catch
        {
            return BadRequest(new { message = "Invalid or expired password reset token." });
        }

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Password reset failed.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new { message = "Password has been reset successfully." });
    }

    private (string ip, string agent) GetRequestInfo()
    {
        string? ip = null;

        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ip = Request.Headers["X-Forwarded-For"]
                .FirstOrDefault()?
                .Split(',')
                .FirstOrDefault()?
                .Trim();
        }

        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        if (ip == "::1")
        {
            ip = "127.0.0.1";
        }

        var agent = Request.Headers["User-Agent"].ToString();
        return (ip ?? "Unknown", agent);
    }

    private string GenerateAccessToken(ApplicationUser user)
    {
        var jwt = _configuration.GetSection("Jwt");
        var issuer = jwt["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing.");
        var audience = jwt["Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing.");
        var key = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key missing.");

        var expiryMinutes = 60;
        _ = int.TryParse(jwt["ExpiryMinutes"], out expiryMinutes);
        if (expiryMinutes <= 0)
        {
            expiryMinutes = 60;
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id ?? string.Empty),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        if (user.Role == Role.SystemAdmin)
        {
            claims.Add(new Claim("SystemAdmin", "True"));
        }
        if (user.Role == Role.VesselUser)
        {
            claims.Add(new Claim("VesselUser", "True"));
        }
        if (user.Role == Role.ManagementUser)
        {
            claims.Add(new Claim("ManagementUser", "True"));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }



    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwt = _configuration.GetSection("Jwt");

        var issuer = jwt["Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer missing.");

        var audience = jwt["Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience missing.");

        var key = jwt["Key"]
            ?? throw new InvalidOperationException("Jwt:Key missing.");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            // Important: we allow expired token here
            ValidateLifetime = false,

            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token.");
        }

        return principal;
    }


    [HttpGet("access-denied")]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return StatusCode(StatusCodes.Status403Forbidden, new
        {
            message = "Access denied."
        });
    }
}
