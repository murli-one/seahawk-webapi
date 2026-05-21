using System.ComponentModel.DataAnnotations;

namespace Seahawk_WebAPI.Contracts.Auth;

public sealed class LoginRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public sealed class RegisterRequest
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? FullName { get; set; }
    public string? CompanyName { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
}


public class TokenRefreshCheckRequest
{
    public string Token { get; set; } = "";
}

public sealed class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string ClientResetUrl { get; set; } = string.Empty;
}

public sealed class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string ConfirmPassword { get; set; } = string.Empty;
}