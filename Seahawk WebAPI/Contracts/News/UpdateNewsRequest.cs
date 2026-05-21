using Microsoft.AspNetCore.Http;
using System;

namespace Seahawk_WebAPI.Contracts.News
{
    public class UpdateNewsRequest
    {
        public string? Title { get; set; }

        public string? ShortDescription { get; set; }

        public string? Content { get; set; }

        public DateTime? PublishDate { get; set; }

        public int SelectedStatus { get; set; } = 0;

        public IFormFile? UploadFile { get; set; }
    }
}