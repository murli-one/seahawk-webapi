using Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Domain.StoredProcedures;
using System.Xml.Serialization;

namespace SeaHawkServices.Web.ViewModels
{
    public class AccountHistoryVM
    {
        /* List */
        public List<AnalysisResult> AnalysisResult { get; set; }
        public List<AccountHistoryWithVessel> AccountHistoryWithVessels { get; set; }
        public List<LiveDataRow> LiveDataRows { get; set; }
        public LiveTableVM LiveTable { get; set; } = new();
        public IEnumerable<SelectListItem> ShowRangeDDL { get; set; }
        // ✅ Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserRole { get; set; }
        public string? FilterVesselName { get; set; }
        public string? FilterPortBunkered { get; set; }
        /* string */
        public string CurrentTab { get; set; }
        public string FilterSampleNumber { get; set; }
        public string FilterVessel { get; set; }
        public string CurrentUser { get; set; }
        public string CurrentUserRole { get; set; }
        public string CurrentUserEmail { get; set; }

        /* int */
        public int SelectedShowRange { get; set; }
        public int SelectedSpecification { get; set; }
        public int SelectedFuelType { get; set; }
        public string IMONumber { get; set; }
        public string Comment { get; set; }

        /* DateTime */
        public DateTime? FilterByFromDate { get; set; }
        public DateTime? FilterByToDate { get; set; }

        /* DDL*/
        public List<SelectListItem> ShowingPeriodDDL()
        {
            var ddl = new List<SelectListItem>()
               {
                    new SelectListItem() { Text = "Last 15 Days", Value = "0"},
                    new SelectListItem() { Text = "Last 30 Days", Value = "1"},
                    new SelectListItem() { Text = "Last 60 Days", Value = "2"},
                    new SelectListItem() { Text = "Last 120 Days", Value = "3"},
                    new SelectListItem() { Text = "Current Quarter", Value = "4"},
                    new SelectListItem() { Text = "Last Quarter", Value = "5"},
                    new SelectListItem() { Text = "All", Value = "6"}
              };
            return ddl;
        }
        public List<SelectListItem> FuelTypeDDL()
        {
            var ddl = new List<SelectListItem>()
               {
                    new SelectListItem() { Text = "All", Value = "0"},
                    new SelectListItem() { Text = "IFO", Value = "1"},
                    new SelectListItem() { Text = "DO", Value = "2"},

              };
            return ddl;
        }
        public List<SelectListItem> SpecificationDDL()
        {
            var ddl = new List<SelectListItem>()
               {
                    new SelectListItem() { Text = "All", Value = "0"},
                    new SelectListItem() { Text = "Normal", Value = "1"},
                    new SelectListItem() { Text = "Caution", Value = "2"},
                    new SelectListItem() { Text = "Critical", Value = "3"},
                    new SelectListItem() { Text = "In Process", Value = "4"},
                    new SelectListItem() { Text = "Completed", Value = "5"},
                    new SelectListItem() { Text = "OK", Value = "6"},
              };
            return ddl;
        }

        public string? SortColumn { get; set; } = "DateBunkered";   // default sort column
        public string? SortDirection { get; set; } = "desc";        // asc | desc

        public class LiveTableVM
        {
            public List<LiveColumnVM> Columns { get; set; } = new();
            public List<LiveDataRow> Samples { get; set; } = new(); // new
        }


        public sealed class LiveColumnVM
        {
            public string VesselName { get; set; } = "";
            public string SampleNumber { get; set; } = "";
            public string SampleDate { get; set; } = "";
        }

        public sealed class LiveMetricRowVM
        {
            public string Label { get; set; } = "";
            public List<string> Values { get; set; } = new(); // one value per column
        }
        [XmlRoot("Row")]
        public class XmlRowDto
        {
            [XmlElement("Field")]
            public List<XmlFieldDto> Fields { get; set; } = new();
        }

        public class XmlFieldDto
        {
            [XmlAttribute("name")]
            public string Name { get; set; } = "";

            [XmlText]
            public string Value { get; set; } = "";
        }
    }
}
