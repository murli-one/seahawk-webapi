namespace Seahawk_WebAPI.Contracts.Pdf;

public sealed class PdfQueryRequest
{
    public string? FilterName { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

public sealed class PdfPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }

    public string? FilterName { get; set; }

    public List<PdfDto> Items { get; set; } = new();
}

public sealed class PdfDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? PDFPath { get; set; }
    public string? ImagePath { get; set; }

    public string? FileName { get; set; }
    public string? PreviewUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
}


public sealed class PdfUploadRequest
{
    public string? Name { get; set; }

    public string? ExistingFileName { get; set; }

    public IFormFile? PdfFile { get; set; }
}
public sealed class PdfUploadResponse
{
    public string Message { get; set; } = string.Empty;
    public PdfDto Pdf { get; set; } = new();
}

public sealed class PdfMessageResponse
{
    public string Message { get; set; } = string.Empty;
}