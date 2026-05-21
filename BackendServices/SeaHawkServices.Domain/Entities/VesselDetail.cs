using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    using System.ComponentModel.DataAnnotations;

    public class VesselDetail
    {
        public int Id { get; set; }
        public string? VesselName { get; set; }
        public string? Owner { get; set; }
        public string? CallSign { get; set; }
        public string? IMONumber { get; set; }
        public string? IFO { get; set; }
        public string? HFO { get; set; }
        public string? Diesel { get; set; }
        public string? Charterer { get; set; }
        public string? Built { get; set; }
        public string? Class { get; set; }
        public string? Type { get; set; }
        public string? Dwt { get; set; }
        public string? Draft { get; set; }
        public string? Registry { get; set; }
        public string? FaxNumber { get; set; }
        public string? TlxNumber { get; set; }
        public string? Email { get; set; }
        public string? Propulsion { get; set; }

        [Display(Name = "Generator Type")]
        public string? GeneratorType { get; set; }
        public string? Purifier { get; set; }
        public string? Filter { get; set; }
        public string? FuelSystem { get; set; }
        public string? GO { get; set; }
        public string? HFOGrade { get; set; }
        public string? IFOGrade { get; set; }
        public string? DOGrade { get; set; }
        public string? GOGrade { get; set; }
        public string? ByFax { get; set; }
        public string? ByEMail { get; set; }
        public string? ByTelex { get; set; }
        public string? FOReportType { get; set; }
        public string? FaxCountry { get; set; }
        public string? FaxArea { get; set; }
        public string? DOReportType { get; set; }
        public string? CommissionType { get; set; }
        public string? BillTo { get; set; }
        public string? Comments { get; set; }
        public string? ExVesselName { get; set; }

        // Nullable foreign key to Company
        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
    }

}
