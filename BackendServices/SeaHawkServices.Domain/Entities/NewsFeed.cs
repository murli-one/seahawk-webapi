using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Domain.Entities
{
    public class NewsFeed
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public DateTime PublishDate { get; set; }
        public Status Status { get; set; }

        public string TinyTitle
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Title))
                    return string.Empty;
                var words = Title.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length <= 6)
                    return Title;
                return string.Join(" ", words.Take(6)) + "...";
            }
        }

        public string ShortPublishDate
        {
            get
            {
                return PublishDate.ToString("MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        public string TinyDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content))
                    return string.Empty;
                var words = Content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length <= 20)
                    return Content;
                return string.Join(" ", words.Take(20)) + "...";
            }
        }

        public string TinyContent
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Content))
                    return string.Empty;
                var words = Content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length <= 50)
                    return Content;
                return string.Join(" ", words.Take(50)) + "...";
            }
        }

    }
}
