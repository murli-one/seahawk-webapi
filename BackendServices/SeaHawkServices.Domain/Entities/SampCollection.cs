using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Data
{
    public class SampCollection
    {
        public int Id { get; set; }
        public SampleType SampleType { get; set; }
        public required string Vessel { get; set; }
        public required int BoxQuantity { get; set; }
        public required DateTime PickUpDate { get; set; }
        public required string RequestorName { get; set; }
        public required string RequestorEmail { get; set; }
        public required string EmailCC { get; set; }
        public Country Country { get; set; }
        public required string Company { get; set; }
        public required string ContactPerson { get; set; }
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public required string City { get; set; }
    }
}
