using JonasWahle.de.Data;
using JonasWahle.de.Data.Models;
using JonasWahle.de.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;

namespace JonasWahle.de.Domain.Services
{
    public class SmtpSettingService(ILogger<SmtpSettingService> Logger, IDbContextFactory<ApplicationContext> DbFactory) : ISmtpSettingService
    {
        public async Task<SmtpSetting?> GetSmtpSettingAsync()
        {
            using ApplicationContext context = await DbFactory.CreateDbContextAsync();
            return await context.SmtpSettings
                .FirstOrDefaultAsync(x => x.IsActive);
        }

        public async Task UpdateSmtpSettingAsync(SmtpSetting newSetting)
        {
            try
            {
                // Test new setting before saving
                using SmtpClient client = new();
                await client.ConnectAsync(newSetting.Host, newSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(newSetting.Username, newSetting.Password);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to validate new SMTP settings.");
                throw new($"Neue SMTP Einstellungen nicht valide. Bestehende Einstellungen wurden nicht überschrieben.\n\n{ex.Message}");
            }
            
            using ApplicationContext context = await DbFactory.CreateDbContextAsync();
            SmtpSetting currentSetting = await context.SmtpSettings
                .FirstOrDefaultAsync(x => x.IsActive) ?? new();

            currentSetting.Host = newSetting.Host;
            currentSetting.Port = newSetting.Port;
            currentSetting.UseSsl = newSetting.UseSsl;
            currentSetting.Username = newSetting.Username;
            currentSetting.Password = newSetting.Password;
            currentSetting.FromAddress = newSetting.FromAddress;
            currentSetting.ToAddress = newSetting.ToAddress;

            await context.SaveChangesAsync();
        }
    }
}
