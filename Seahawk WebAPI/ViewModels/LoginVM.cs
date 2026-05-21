using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Web.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class LoginVM
    {
        // ---------------- LOGIN FIELDS ----------------
        [Required(ErrorMessage = "User name is required")]
        [Display(Name = "User Name")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "User name must be 3–30 characters.")]
        [RegularExpression(@"^[A-Za-z0-9._-]{3,30}$", ErrorMessage = "User name can contain letters, numbers, dot, underscore or hyphen only.")]
        public string UserName { get; set; } = "";

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = "";

        public bool RememberMe { get; set; }
        public Role SelectedRole { get; set; } = Role.ManagementUser;

        // ---------------- PAGE FLAGS ----------------
        public string? LoginErrorMessage { get; set; }
        public string? RegisterErrorMessage { get; set; }
        public bool IsRegister { get; set; }
        public bool UpdateSuccessful { get; set; }
        public string? RedirectUrl { get; set; }

        // ---------------- REGISTER FIELDS (Annotated) ----------------
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Full name must be 2–80 characters.")]
        [RegularExpression(@"^[A-Za-z\s.'-]{2,80}$", ErrorMessage = "Full name can contain letters and spaces only.")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "User name is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "User name must be 3–30 characters.")]
        [RegularExpression(@"^[A-Za-z0-9._-]{3,30}$", ErrorMessage = "User name can contain letters, numbers, dot, underscore or hyphen only.")]
        public string RegisterUserName { get; set; } = "";

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "Company name must be 2–120 characters.")]
        public string CompanyName { get; set; } = "";

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be 5–200 characters.")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Phone number must be 10–15 digits.")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(120)]
        public string Email { get; set; } = "";

        // Register password rules (your requirement: alphanumeric and > 6)
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$",
        ErrorMessage = "Password must contain at least 1 letter and 1 number, and be at least 6 characters.")]
        public string? RegisterPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("RegisterPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        // keep these if you use them elsewhere
        public UserVM? UserVM { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}

