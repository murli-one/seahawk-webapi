using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.Roles;
using SeaHawkServices.Application.Services.Interface;
using System.Xml.Linq;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesApiController : ControllerBase
{
    private readonly IRoleService _service;

    public RolesApiController(IRoleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<RoleListResponse>> GetRoles()
    {
        var roles = await _service.GetAllAsync();

        return Ok(new RoleListResponse
        {
            CurrentTab = "Roles",
            Items = roles
                .Select(ToDto)
                .ToList()
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoleDto>> GetRole(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid role id." });

        var role = await _service.GetByIdAsync(id);

        if (role == null)
            return NotFound(new { message = "Role not found." });

        return Ok(ToDto(role));
    }

    [HttpPost]
    public async Task<ActionResult<RoleMessageResponse>> CreateRole([FromBody] RoleUpsertRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.RoleName))
            return BadRequest(new { message = "Role name is required." });

        var error = await _service.CreateAsync(
            request.RoleName.Trim(),
            request.Description?.Trim());

        if (error is not null)
        {
            return BadRequest(new RoleMessageResponse
            {
                Message = error
            });
        }

        return Ok(new RoleMessageResponse
        {
            Message = "Role created successfully."
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RoleMessageResponse>> UpdateRole(
        int id,
        [FromBody] RoleUpsertRequest request)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid role id." });

        if (request == null)
            return BadRequest(new { message = "Request body is required." });

        if (string.IsNullOrWhiteSpace(request.RoleName))
            return BadRequest(new { message = "Role name is required." });

        var existingRole = await _service.GetByIdAsync(id);

        if (existingRole == null)
            return NotFound(new { message = "Role not found." });

        existingRole.RoleName = request.RoleName.Trim();
        existingRole.Description = request.Description?.Trim();

        var error = await _service.UpdateAsync(existingRole);

        if (error is not null)
        {
            return BadRequest(new RoleMessageResponse
            {
                Message = error
            });
        }

        return Ok(new RoleMessageResponse
        {
            Message = "Role updated successfully."
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid role id." });

        var role = await _service.GetByIdAsync(id);

        if (role == null)
            return NotFound(new { message = "Role not found." });

        await _service.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("export-xml")]
    public async Task<IActionResult> ExportXml()
    {
        var roles = await _service.GetAllAsync();

        var xml = new XDocument(
            new XElement("Roles",
                roles.Select(r =>
                    new XElement("Role",
                        new XElement("Id", r.Id),
                        new XElement("RoleName", r.RoleName),
                        new XElement("Description", r.Description)
                    )
                )
            )
        );

        var bytes = System.Text.Encoding.UTF8.GetBytes(xml.ToString());

        return File(
            bytes,
            "application/xml",
            $"Roles_{DateTime.UtcNow:yyyyMMddHHmmss}.xml");
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel()
    {
        var roles = await _service.GetAllAsync();

        using var stream = new MemoryStream();

        using (var spreadsheet = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = spreadsheet.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet();

            var sheetData = new SheetData();
            worksheetPart.Worksheet.AppendChild(sheetData);

            var sheets = spreadsheet.WorkbookPart!.Workbook.AppendChild(new Sheets());

            sheets.Append(new Sheet
            {
                Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Roles"
            });

            var headers = new[]
            {
                "Id",
                "Role Name",
                "Description"
            };

            var maxLengths = new int[headers.Length];

            var headerRow = new Row();

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.AppendChild(CreateTextCell(headers[i]));
                maxLengths[i] = headers[i].Length;
            }

            sheetData.AppendChild(headerRow);

            foreach (var r in roles)
            {
                var values = new[]
                {
                    r.Id.ToString(),
                    r.RoleName ?? "",
                    r.Description ?? ""
                };

                var row = new Row();

                for (int i = 0; i < values.Length; i++)
                {
                    row.AppendChild(CreateTextCell(values[i]));
                    maxLengths[i] = Math.Max(maxLengths[i], values[i].Length);
                }

                sheetData.AppendChild(row);
            }

            var columns = new Columns();

            for (uint i = 1; i <= headers.Length; i++)
            {
                columns.Append(new Column
                {
                    Min = i,
                    Max = i,
                    Width = Math.Min(maxLengths[i - 1] + 2, 80),
                    CustomWidth = true
                });
            }

            worksheetPart.Worksheet.InsertAt(columns, 0);

            workbookPart.Workbook.Save();
        }

        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Roles_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    private static RoleDto ToDto(dynamic role)
    {
        return new RoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName,
            Description = role.Description
        };
    }

    private static Cell CreateTextCell(string value)
    {
        return new Cell
        {
            CellValue = new CellValue(value ?? string.Empty),
            DataType = CellValues.String
        };
    }
}