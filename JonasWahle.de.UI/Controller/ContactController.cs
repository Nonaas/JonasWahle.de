using JonasWahle.de.Domain.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace JonasWahle.de.UI.Controller
{
    [Route("api/[controller]")]
    public class ContactController(IOptions<SmtpSettings> SmtpSettings, ILogger<ContactController> Logger) : ControllerBase
    {
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
                Logger.LogError(ex, "Fehler beim Senden der Kontakt Nachricht");
                return StatusCode(500, new { message = "Fehler beim Senden der Nachricht" });
            }
        }

        private async Task SendEmailAsync(SendMessageDto dto)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("jonaswahle.de | Kontaktformular", SmtpSettings.Value.FromAddress));
            message.To.Add(new MailboxAddress("Jonas Wahle", SmtpSettings.Value.ToAddress));
            message.Subject = $"jonaswahle.de | Kontaktformular ({dto.Name})";
            message.Body = new TextPart("plain")
            {
                Text = $@"
Neue Nachricht über das Kontaktformular:

Name: {dto.Name}
E-Mail: {dto.Email}
Nachricht: {dto.Message}

Gesendet am: {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
            };

            using SmtpClient client = new();
            await client.ConnectAsync(SmtpSettings.Value.Host, SmtpSettings.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(SmtpSettings.Value.Username, SmtpSettings.Value.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }

}
