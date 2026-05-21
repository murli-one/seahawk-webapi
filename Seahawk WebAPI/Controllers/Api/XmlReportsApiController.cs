using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.XmlReports;
using SeaHawkServices.Application.Services.Interface;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/xml-reports")]
[Authorize]
public class XmlReportsApiController : ControllerBase
{
    private readonly IXMLReportService _xmlService;

    public XmlReportsApiController(IXMLReportService xmlService)
    {
        _xmlService = xmlService;
    }

    // POST: /api/xml-reports/generate
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateXmlReport([FromBody] GenerateXmlReportRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.SampleNumber))
        {
            return BadRequest(new XmlReportErrorResponse
            {
                Success = false,
                Message = "Sample number is required."
            });
        }

        try
        {
            byte[] content;
            string fileName;

            var sampleNumber = request.SampleNumber.Trim();

            if (request.SelectedReportType == 1)
            {
                (content, fileName) = await _xmlService.BuildResidualXmlAsync(sampleNumber);
            }
            else
            {
                (content, fileName) = await _xmlService.BuildDistillateXmlAsync(sampleNumber);
            }

            if (content == null || content.Length == 0 || string.IsNullOrWhiteSpace(fileName))
            {
                return NotFound(new XmlReportErrorResponse
                {
                    Success = false,
                    Message = $"No data found for sample number '{sampleNumber}'."
                });
            }

            return File(content, "application/xml", fileName);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new XmlReportErrorResponse
            {
                Success = false,
                Message = "An error occurred while generating the XML report."
            });
        }
    }
}