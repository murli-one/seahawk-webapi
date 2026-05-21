using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PdfiumViewer;
using Seahawk_WebAPI.Contracts.Pdf;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System.Drawing.Imaging;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/pdfs")]
[Authorize]
public class PdfsApiController : ControllerBase
{
    private readonly IPDFService _pdfService;
    private readonly string _uploadsPath;
    private readonly string _thumbsPath;
    private readonly UserManager<ApplicationUser> _userManager;

    public PdfsApiController(
        IWebHostEnvironment env,
        IPDFService pdfService,
        UserManager<ApplicationUser> userManager)
    {
        _pdfService = pdfService;
        _userManager = userManager;

        _uploadsPath = Path.Combine(env.WebRootPath, "uploads");
        _thumbsPath = Path.Combine(_uploadsPath, "thumbnails");

        Directory.CreateDirectory(_uploadsPath);
        Directory.CreateDirectory(_thumbsPath);
    }

    [HttpGet]
    public async Task<ActionResult<PdfPagedResponse>> GetPdfs([FromQuery] PdfQueryRequest request)
    {
        request = NormalizeQuery(request);

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Unauthorized(new { message = "User not found." });

        var all = await _pdfService.GetAllAsync();
        var query = (all ?? new List<PDF>()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.FilterName))
        {
            var name = request.FilterName.Trim();

            query = query.Where(x =>
                !string.IsNullOrEmpty(x.Name) &&
                x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        query = query.OrderByDescending(x => x.Id);

        var totalCount = query.Count();

        var pageItems = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Ok(new PdfPagedResponse
        {
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = request.PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / request.PageSize),

            Name = currentUser.UserName,
            Email = currentUser.Email,
            Role = currentUser.Role.ToString(),

            FilterName = request.FilterName,

            Items = pageItems.Select(ToDto).ToList()
        });
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<PdfDto>>> GetAllPdfs()
    {
        var files = await _pdfService.GetAllAsync();

        return Ok((files ?? new List<PDF>())
            .OrderByDescending(x => x.Id)
            .Select(ToDto)
            .ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PdfDto>> GetPdfById(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid PDF id." });

        var pdf = await _pdfService.GetByIdAsync(id);
         
        if (pdf == null)
            return NotFound(new { message = "PDF not found." });

        return Ok(ToDto(pdf));
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PdfUploadResponse>> UploadPdf([FromForm] PdfUploadRequest request)
    {
        Directory.CreateDirectory(_uploadsPath);
        Directory.CreateDirectory(_thumbsPath);

        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Name is required." });

        string fileName;
        string physicalPdfFullPath;

        if (request.PdfFile != null && request.PdfFile.Length > 0)
        {
            fileName = Path.GetFileName(request.PdfFile.FileName);

            if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Only PDF files are allowed." });

            physicalPdfFullPath = Path.Combine(_uploadsPath, fileName);

            if (!System.IO.File.Exists(physicalPdfFullPath))
            {
                await using var fs = new FileStream(physicalPdfFullPath, FileMode.Create);
                await request.PdfFile.CopyToAsync(fs);
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.ExistingFileName))
        {
            fileName = Path.GetFileName(request.ExistingFileName);

            if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Only PDF files are allowed." });

            physicalPdfFullPath = Path.Combine(_uploadsPath, fileName);

            if (!System.IO.File.Exists(physicalPdfFullPath))
            {
                return NotFound(new
                {
                    message = "Selected PDF does not exist on server."
                });
            }
        }
        else
        {
            return BadRequest(new
            {
                message = "Upload a PDF or select an existing one."
            });
        }

        var thumbFileName = Path.GetFileNameWithoutExtension(fileName) + ".png";
        var physicalThumbPath = Path.Combine(_thumbsPath, thumbFileName);

        if (!System.IO.File.Exists(physicalThumbPath))
        {
            using var document = PdfDocument.Load(physicalPdfFullPath);
            using var image = document.Render(0, 100, 140, true);
            image.Save(physicalThumbPath, ImageFormat.Png);
        }

        var webPdfPath = Path.Combine("uploads", fileName).Replace('\\', '/');
        var webImagePath = Path.Combine("uploads", "thumbnails", thumbFileName).Replace('\\', '/');

        var entity = new PDF
        {
            Name = request.Name.Trim(),
            PDFPath = webPdfPath,
            ImagePath = webImagePath
        };

        await _pdfService.AddAsync(entity);

        return Ok(new PdfUploadResponse
        {
            Message = "PDF uploaded successfully.",
            Pdf = ToDto(entity)
        });
    }

    [HttpGet("{id:int}/preview")]
    [AllowAnonymous]
    public async Task<IActionResult> PreviewPdf(int id, [FromQuery] string? filename = null)
    {
        PDF? pdf = null;

        if (id > 0)
        {
            pdf = await _pdfService.GetByIdAsync(id);
        }

        if (pdf == null && !string.IsNullOrWhiteSpace(filename))
        {
            var justName = Path.GetFileName(filename.Trim());
            var nameNoExt = Path.GetFileNameWithoutExtension(justName);
            pdf = await _pdfService.GetByFileNameAsync(nameNoExt);
        }

        if (pdf == null)
            return NotFound(new { message = "PDF not found." });

        if (string.IsNullOrWhiteSpace(pdf.PDFPath))
            return NotFound(new { message = "PDF path not found." });

        var relativePath = pdf.PDFPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (!System.IO.File.Exists(physicalPath))
            return NotFound(new { message = "PDF file does not exist on server." });

        var fileName = Path.GetFileName(physicalPath);

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";

        return PhysicalFile(physicalPath, "application/pdf");
    }

    [HttpGet("preview-by-file")]
    [AllowAnonymous]
    public async Task<IActionResult> PreviewPdfByFileName([FromQuery] string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return BadRequest(new { message = "File name is required." });

        var justName = Path.GetFileName(filename.Trim());
        var nameNoExt = Path.GetFileNameWithoutExtension(justName);

        var pdf = await _pdfService.GetByFileNameAsync(nameNoExt);

        if (pdf == null)
            return NotFound(new { message = "PDF not found." });

        if (string.IsNullOrWhiteSpace(pdf.PDFPath))
            return NotFound(new { message = "PDF path not found." });

        var relativePath = pdf.PDFPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (!System.IO.File.Exists(physicalPath))
            return NotFound(new { message = "PDF file does not exist on server." });

        var fileName = Path.GetFileName(physicalPath);

        Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";

        return PhysicalFile(physicalPath, "application/pdf");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePdf(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid PDF id." });

        var pdf = await _pdfService.GetByIdAsync(id);

        if (pdf == null)
            return NotFound(new { message = "PDF not found." });

        await _pdfService.DeleteAsync(id);

        return NoContent();
    }

    private static PdfQueryRequest NormalizeQuery(PdfQueryRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        request.FilterName = request.FilterName?.Trim();

        return request;
    }

    private static PdfDto ToDto(PDF pdf)
    {
        var fileName = !string.IsNullOrWhiteSpace(pdf.PDFPath)
            ? Path.GetFileName(pdf.PDFPath)
            : null;

        return new PdfDto
        {
            Id = pdf.Id,
            Name = pdf.Name,
            PDFPath = pdf.PDFPath,
            ImagePath = pdf.ImagePath,
            FileName = fileName,
            PreviewUrl = pdf.Id > 0 ? $"/api/pdfs/{pdf.Id}/preview" : null,
            ThumbnailUrl = !string.IsNullOrWhiteSpace(pdf.ImagePath)
                ? "/" + pdf.ImagePath.TrimStart('/')
                : null
        };
    }
}