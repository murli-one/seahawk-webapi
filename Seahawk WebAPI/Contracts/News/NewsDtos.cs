using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.News;

public sealed class NewsQueryRequest
{
    public string? Title { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

public sealed class NewsPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserRole { get; set; }

    public string? FilterTitle { get; set; }

    public List<NewsFeedDto> Items { get; set; } = new();
}

public sealed class NewsDetailsResponse
{
    public NewsFeedDto NewsFeed { get; set; } = new();
    public List<NewsFeedDto> LatestNews { get; set; } = new();
}

public sealed class NewsFeedDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? ShortDescription { get; set; }
    public string? Content { get; set; }
    public DateTime PublishDate { get; set; }
    public Status Status { get; set; }
    public string StatusName { get; set; } = "";

    public string TinyTitle { get; set; } = "";
    public string ShortPublishDate { get; set; } = "";
    public string TinyDescription { get; set; } = "";
    public string TinyContent { get; set; } = "";
}

public sealed class NewsStatusOptionDto
{
    public string Text { get; set; } = "";
    public string Value { get; set; } = "";
    public int NumericValue { get; set; }
}

public sealed class NewsFormOptionsResponse
{
    public List<NewsStatusOptionDto> Statuses { get; set; } = new();
}

public sealed class NewsMessageResponse
{
    public string Message { get; set; } = string.Empty;
}