using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class PDFVM
    {
        public string Id { get; set; }

        public IEnumerable<PDF> Files { get; set; } = new List<PDF>();

        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public string? ExistingFileName { get; set; }
        public IFormFile PdfFile { get; set; }

        // ✅ Optional filter
        public string? FilterName { get; set; }

        // ✅ Pagination (same pattern)
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}