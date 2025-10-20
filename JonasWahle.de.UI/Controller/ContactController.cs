using JonasWahle.de.Data.Models;
using JonasWahle.de.Domain.Interfaces;
using JonasWahle.de.Domain.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace JonasWahle.de.UI.Controller
{
    [Route("api/[controller]")]
    public class ContactController(ILogger<ContactController> Logger, ISmtpSettingService SmtpSettingService) : ControllerBase
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
                Logger.LogError(ex, "Error while sending contact message.");
                return StatusCode(500, new { message = "Fehler beim Senden der Nachricht" });
            }
        }

        private async Task SendEmailAsync(SendMessageDto dto)
        {
            SmtpSetting? smtpSetting = await SmtpSettingService.GetSmtpSettingAsync();

            if(smtpSetting != null)
            {
                MimeMessage message = new();
                message.From.Add(new MailboxAddress("jonaswahle.de | Kontaktformular", smtpSetting.FromAddress));
                message.To.Add(new MailboxAddress("Jonas Wahle", smtpSetting.ToAddress));
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
                await client.ConnectAsync(smtpSetting.Host, smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpSetting.Username, smtpSetting.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            else
            {
                throw new InvalidOperationException("SMTP settings are not configured.");
            }
        }
    }

}
