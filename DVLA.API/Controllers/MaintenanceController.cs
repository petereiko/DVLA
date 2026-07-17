using DVLA.Business.Repository;
using DVLA.Business.TempPasswordModule;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [EnableRateLimiting("AuthenticatedRead")]
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IRepositoryQuery<EmailTemplate> _emailTemplateRepository;
        private readonly IRepositoryQuery<SmsTemplate> _smsTemplateRepository;
        private readonly ITempPasswordService _tempPasswordService;

        public MaintenanceController(
            IRepositoryQuery<EmailTemplate> emailTemplateRepository,
            IRepositoryQuery<SmsTemplate> smsTemplateRepository,
            ITempPasswordService tempPasswordService)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _smsTemplateRepository = smsTemplateRepository;
            _tempPasswordService = tempPasswordService;
        }

        [HttpGet("email-templates")]
        public IActionResult GetEmailTemplates()
        {
            return Ok(_emailTemplateRepository.GetAll());
        }

        [HttpGet("email-templates/{code}")]
        public IActionResult GetEmailTemplate(string code)
        {
            var result = _emailTemplateRepository.Filter(x => x.Code == code).FirstOrDefault();
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("email-templates/{id:int}")]
        [EnableRateLimiting("SensitiveWrite")]
        public async Task<IActionResult> UpdateEmailTemplate(int id, [FromBody] EmailTemplateDto model)
        {
            var template = _emailTemplateRepository.GetById(id);
            if (template == null)
            {
                return NotFound();
            }

            template.EmailName = model.EmailName;
            template.EmailBody = model.EmailBody;
            template.EmailSubject = model.EmailSubject;
            await _emailTemplateRepository.UpdateAsync(template);
            return Ok(new { success = true, message = "Email template updated successfully." });
        }

        [HttpGet("sms-templates")]
        public IActionResult GetSmsTemplates()
        {
            return Ok(_smsTemplateRepository.GetAll());
        }

        [HttpGet("sms-templates/{code}")]
        public IActionResult GetSmsTemplate(string code)
        {
            var result = _smsTemplateRepository.Filter(x => x.Code == code).FirstOrDefault();
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("sms-templates/{id:int}")]
        [EnableRateLimiting("SensitiveWrite")]
        public async Task<IActionResult> UpdateSmsTemplate(int id, [FromBody] SmsTemplateDto model)
        {
            var template = _smsTemplateRepository.GetById(id);
            if (template == null)
            {
                return NotFound();
            }

            template.Name = model.Name;
            template.Body = model.Body;
            template.Subject = model.Subject;
            await _smsTemplateRepository.UpdateAsync(template);
            return Ok(new { success = true, message = "SMS template updated successfully." });
        }

        [HttpPost("temp-password")]
        [EnableRateLimiting("SensitiveWrite")]
        public async Task<IActionResult> CreateTempPassword([FromBody] TempPasswordDto model)
        {
            var result = await _tempPasswordService.Create(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
