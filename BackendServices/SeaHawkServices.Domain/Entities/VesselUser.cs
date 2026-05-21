using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    public class VesselUser
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public VesselDetail VesselDetail { get; set; }
        public int VesselDetailId { get; set; }
    }
}
