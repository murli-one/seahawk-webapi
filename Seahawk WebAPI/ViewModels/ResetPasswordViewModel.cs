using System.ComponentModel.DataAnnotations;

namespace SeaHawkServices.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Token coming from the email link (Base64 URL encoded)
        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "Password must be at least {2} characters long.")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
