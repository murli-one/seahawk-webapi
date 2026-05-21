using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class RequestLogModalVM
    {
        // Existing
        public int RequestId { get; set; }
        public string RequestType { get; set; } // "SampleCollection" or "SamplingKit"
        public IEnumerable<RequestHistory> History { get; set; } = Enumerable.Empty<RequestHistory>();

        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }

        public string CommentActionUrl { get; set; }

        // ✅ NEW: Filters (NO date filters)
        public string? FilterRequestType { get; set; }
        public string? FilterRequestId { get; set; }
        public string? FilterAction { get; set; }
        public string? FilterPerformedBy { get; set; }
        public string? FilterNotes { get; set; }
        // ✅ Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
