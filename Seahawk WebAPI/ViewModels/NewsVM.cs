using SeaHawkServices.Domain.Entities;


namespace SeaHawkServices.Web.ViewModels
{
    public class NewsVM
    {
        public NewsFeed NewsFeed { get; set; }
        public IEnumerable<NewsFeed> NewsFeedList { get; set; } = new List<NewsFeed>();

        public int SelectedStatus { get; set; }
        public int Id { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserRole { get; set; }

        // ✅ Filters
        public string? FilterTitle { get; set; }

        // ✅ Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
