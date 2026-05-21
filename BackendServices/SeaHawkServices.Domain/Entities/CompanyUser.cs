using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    public class CompanyUser
    {
        public int Id { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
