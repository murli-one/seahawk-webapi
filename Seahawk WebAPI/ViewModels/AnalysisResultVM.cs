using Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SeaHawkServices.Web.ViewModels
{
    public class AnalysisResultVM
    {
        public AnalysisResult AnalysisResult { get; set; }
        public IEnumerable<AnalysisResult> AnalysisResultList { get; set; }
        public IEnumerable<AnalysisResultDto> AnalysisResultDtoList { get; set; }

        public string ErrorMessage { get; set; }
        public string FilterSampleNumber { get; set; }
        public string FilterVessel { get; set; }
        public int Id { get; set; }

        // For header / topbar info (same pattern as Company)
        public string CurrentUser { get; set; }
        public string CurrentUserEmail { get; set; }
        public string CurrentUserRole { get; set; }

        // ✅ Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        // Dropdown for Vessel
        public IEnumerable<SelectListItem> VesselOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CompanyOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> SampleCollectionOptions { get; set; } = new List<SelectListItem>();

        public string? FilterPort { get; set; }
        public DateTime? FilterBunkerDateFrom { get; set; }
        public DateTime? FilterBunkerDateTo { get; set; }
        public string? FilterFuelType { get; set; }
        public string? FilterComment { get; set; }
        public string? FilterAiQuery { get; set; }

    }

    public class AnalysisResultDto
    {
        public int Id { get; set; }
        public string SampleNumber { get; set; }
        public string PortBunkered { get; set; }
        public DateTime? DateBunkered { get; set; }
        public string Specification { get; set; }
        public string FuelType { get; set; }
        public DateTime? DateReceived { get; set; }
        public string Comment { get; set; }
        public int? VesselDetailId { get; set; }
        public string VesselName { get; set; }
        public string Grade { get; set; }
        public decimal? H2S { get; set; }
        public decimal? AlSi { get; set; }
    }
}
