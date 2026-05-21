using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Domain.Entities
{
    public class Career
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string JobDescription { get; set; }
        public int Experience { get; set; }
        public Status Status { get; set; }
    }
}
