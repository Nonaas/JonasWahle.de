namespace JonasWahle.de.Domain.Interfaces
{
    public interface ICookieService
    {
        Task DeleteCookieAsync(string key);
        Task<string?> GetCookieAsync(string key);
        Task SetCookieAsync(string key, string value, int expireDays);
    }
}