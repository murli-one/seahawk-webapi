using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Keyless]
    public class AccountHistoryWithVessel
    {
        public string IMONumber { get; set; }
        public string VesselName { get; set; }
        public string PortBunkered { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateBunkered { get; set; }

        public string Specification { get; set; }
        public string FuelType { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateReceived { get; set; }
        public string SampleNumber { get; set; }
        public string Comment { get; set; }
    }
}
