using JonasWahle.de.Data.Models;

namespace JonasWahle.de.Domain.Interfaces
{
    public interface ISmtpSettingService
    {
        Task<SmtpSetting?> GetSmtpSettingAsync();
        Task UpdateSmtpSettingAsync(SmtpSetting newSetting);
    }
}