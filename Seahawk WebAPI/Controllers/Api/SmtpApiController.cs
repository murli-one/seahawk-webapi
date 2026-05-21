using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.Smtp;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/smtp")]
[Authorize]
public class SmtpApiController : ControllerBase
{
    private readonly ISmtpService _smtpService;

    public SmtpApiController(ISmtpService smtpService)
    {
        _smtpService = smtpService;
    }

    // GET: /api/smtp
    [HttpGet]
    public async Task<ActionResult<SmtpSettingDto>> Get()
    {
        if (!IsSystemAdmin())
        {
            return Forbid();
        }

        var smtpSetting = await _smtpService.GetAsync();

        if (smtpSetting == null)
        {
            smtpSetting = new SmtpSetting
            {
                Email = "info@seahawkservices.com",
                SmtpPort = 587,
                EnableSSL = true
            };
        }

        return Ok(ToDto(smtpSetting));
    }

    // PUT: /api/smtp
    [HttpPut]
    public async Task<ActionResult<SmtpApiResponse>> Update([FromBody] UpdateSmtpSettingRequest request)
    {
        if (!IsSystemAdmin())
        {
            return Forbid();
        }

        if (request == null)
        {
            return BadRequest(new SmtpApiResponse
            {
                Success = false,
                Message = "SMTP setting request is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new SmtpApiResponse
            {
                Success = false,
                Message = "Email is required."
            });
        }

        if (request.SmtpPort <= 0)
        {
            return BadRequest(new SmtpApiResponse
            {
                Success = false,
                Message = "SMTP port must be greater than 0."
            });
        }

        var model = new SmtpSetting
        {
            Id = request.Id,
            Email = request.Email,
            SmtpPort = request.SmtpPort,
            Password = request.SmtpPassword,
            EnableSSL = request.EnableSSL
        };

        await _smtpService.UpdateAsync(model);

        return Ok(new SmtpApiResponse
        {
            Success = true,
            Message = "SMTP settings updated successfully."
        });
    }

    private bool IsSystemAdmin()
    {
        return User.HasClaim("SystemAdmin", "True");
    }

    private static SmtpSettingDto ToDto(SmtpSetting setting)
    {
        return new SmtpSettingDto
        {
            Id = setting.Id,
            Email = setting.Email,
            SmtpPort = setting.SmtpPort,
            SmtpUsername = setting.SmtpHost,
            SmtpPassword = setting.Password,
            EnableSSL = setting.EnableSSL
        };
    }
}