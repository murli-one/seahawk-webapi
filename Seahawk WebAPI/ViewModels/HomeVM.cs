using Microsoft.AspNetCore.Mvc.Rendering;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<string> PDFList { get; set; }
        public ContactUs ContactUs { get; set; }
        public IEnumerable<ContactUs> ContactUsList { get; set; }
        public IEnumerable<Career> CareerList { get; set; }
        public string SampleNumber { get; set; }
        public int SelectedReportType { get; set; }
        public List<SelectListItem> ReportTypeDDL()
        {
            var ddl = new List<SelectListItem>()
               {
                    new SelectListItem() { Text = "XML Distillate", Value = "0"},
                    new SelectListItem() { Text = "XML Residual", Value = "1"},

              };
            return ddl;
        }
    }
}
