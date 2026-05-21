using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class CareerVM
    {
        public int Id { get; set;}
        public Career Career { get; set; }
        public IEnumerable<Career> CareerList { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }

        // ✅ Filters
        public string? FilterTitle { get; set; }
        public string? FilterStatus { get; set; }  // store as string for dropdown/text

        // ✅ Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
