using Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeaHawkServices.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SeaHawkServices.Web.ViewModels
{
    public class RoleVM
    {
        public List<Roles> RolesList { get; set; }
        public Roles Roles { get; set; }
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string CurrentTab { get; set; }
        public string ErrorMessage { get; set; }
        public string? Description { get; set; }
    }
}
