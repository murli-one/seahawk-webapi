using System;
using System.Collections.Generic;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Web.ViewModels
{
    public class UserLoginHistoryVM
    {
        /* meta */
        public string CurrentUser { get; set; }
        public string CurrentUserRole { get; set; }

        // ✅ Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; } = 0;

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        /* filters */
        public string FilterUserName { get; set; }
        public string FilterRole { get; set; }
        public DateTime? FilterFromDate { get; set; }
        public DateTime? FilterToDate { get; set; }

        /* data */
        public List<UserLoginHistory> LoginHistoryList { get; set; } = new();
    }
}
