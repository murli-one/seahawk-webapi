using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    public class Company
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? BillingAddress { get; set; }
        public string? City { get; set; }
        public string? StateOrProvince { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? Notes { get; set; }
        public string? ShipOwner { get; set; }
        public string? FuelSupplier { get; set; }
        public string? ContractLab { get; set; }
        public string? Charterer { get; set; }
        public string? InvoiceType { get; set; }
        public string? CompanyKey { get; set; }
        public string? FaxCountry { get; set; }
        public string? FaxArea { get; set; }
        public string? ClientRef { get; set; }

        public List<VesselDetail> VesselDetailList { get; set; }
    }
}
