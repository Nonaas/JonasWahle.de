using JonasWahle.de.Domain.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace JonasWahle.de.UI.Controller
{
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IOptions<SmtpSettings> smtpSettings, ILogger<ContactController> logger)
        {
            _smtpSettings = smtpSettings.Value;
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
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Kontaktformular", _smtpSettings.FromAddress));
            message.To.Add(new MailboxAddress("Jonas Wahle", _smtpSettings.ToAddress));
            message.Subject = $"Kontaktformular-Nachricht von {dto.Name}";
            message.Body = new TextPart("plain")
            {
                Text = $@"
Neue Nachricht über das Kontaktformular:

Name: {dto.Name}
E-Mail: {dto.Email}
Nachricht: {dto.Message}

Gesendet am: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

}
