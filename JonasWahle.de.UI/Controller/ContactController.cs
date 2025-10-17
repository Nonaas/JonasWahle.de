using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace JonasWahle.de.UI.Controller
{
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IConfiguration configuration, ILogger<ContactController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> SendContactMessage([FromBody] SendMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await SendEmailAsync(dto);
                return Ok(new { message = "Nachricht erfolgreich gesendet" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Senden der Kontakt Nachricht");
                return StatusCode(500, new { message = "Fehler beim Senden der Nachricht" });
            }
        }

        private async Task SendEmailAsync(SendMessageDto dto)
        {
            using var mail = new MailMessage();
            using var smtp = new SmtpClient();

            // Email setup
            mail.From = new MailAddress(_configuration["Email:FromAddress"], "Kontaktformular");
            mail.To.Add(new MailAddress(_configuration["Email:ToAddress"], "Jonas Wahle"));
            mail.Subject = $"Kontaktformular-Nachricht von {dto.Name}";
            mail.Body = $@"
Neue Nachricht über das Kontaktformular:

Name: {dto.Name}
E-Mail: {dto.Email}
Nachricht: {dto.Message}

Gesendet am: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            mail.IsBodyHtml = false;

            // SMTP setup
            smtp.Host = _configuration["Email:SmtpHost"];
            smtp.Port = int.Parse(_configuration["Email:SmtpPort"]);
            smtp.EnableSsl = bool.Parse(_configuration["Email:UseSsl"]);
            smtp.Credentials = new NetworkCredential(
                _configuration["Email:Username"], 
                _configuration["Email:Password"]);

            await smtp.SendMailAsync(mail);
        }
    }

    public class SendMessageDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bitte geben Sie Ihren Namen an")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "Mindestens 2 und maximal 50 Zeichen erlaubt")]
        public string? Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bitte geben Sie Ihre Email an")]
        [EmailAddress(ErrorMessage = "Bitte geben Sie eine gültige Email-Adresse ein")]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "Mindestens 5 und maximal 100 Zeichen erlaubt")]
        public string? Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bitte geben Sie Ihre Nachricht an")]
        [StringLength(maximumLength: 1000, MinimumLength = 10, ErrorMessage = "Mindestens 10 und maximal 1000 Zeichen erlaubt")]
        public string? Message { get; set; }
    }
}
