using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class UserVM
    {
       
        /*Models*/
        public ApplicationUser ApplicationUser { get; set; }
        public List<ApplicationUser> Users { get; set; }

        /* strings */
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Repeat password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        // ✅ Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        // ✅ keep search text
        public string? Search { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public string CurrentTab { get; set; } = "approved";
        public string PhoneNumber { get; set; }
        public string UserRole { get; set; }
        public string? ReturnUrl { get; set; }
        // ✅ Assignment selection
        public int? SelectedCompanyId { get; set; }
        public int? SelectedVesselId { get; set; }

        // ✅ Dropdown lists
        public int SelectedRole { get; set; }
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Vessels { get; set; } = new();

        public string CompanyName { get; set; }
        public string Address { get; set; }

        /*bool*/
        public bool PasswordInvalid { get; set; }
        public bool InvalidLogin { get; set; }
        public bool PasswordDoesNotMatch { get; set; }
        public bool UpdateSuccessful { get; set; }
        public bool CreateSuccessful { get; set; }
        public bool PasswordSentViaText { get; set; }
        public bool PasswordSentViaEmail { get; set; }
        public bool NoMatchEmail { get; set; }
        public bool NoMatchText { get; set; }
        public bool RememberMe { get; set; }
        public bool SendTempPasswordViaEmail { get; set; }
        public bool SendTempPasswordViaText { get; set; }
        public string ErrorMessage { get; set; }
      
        public bool ShowAddOtherAccount { get; set; }
        public bool ConnectMicrosoftAccount {  get; set; }
        public bool ConnectGmailAccount {  get; set; }
        public bool ShowAddStartSyncDate { get; set; }
        public bool ShowExistingAccount { get; set; }
        public bool isAdmin { get; set; }
        public bool passwordsent { get; set; }
        public bool nomatch { get; set; }
        public bool IsSystemAdmin { get; set; }
        public bool ShowAddPhoneNumber { get; set; }
        public bool ShowExistingPhoneNumber { get; set; }
        public bool ShowAddAssignedRole { get; set; }

        public bool IsApprovedUser { get; set; }
        public string CurrentUserId { get; set; }
        public bool OpenPopupOnLoad { get; set; }

        /*int*/
        public int CompanyId { get; set; }

        //[Required(ErrorMessage = "Role is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "Role is required")]
        public DateTime? LastSyncDate { get; set; }

    
    }
}
