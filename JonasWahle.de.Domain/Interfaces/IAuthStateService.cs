using JonasWahle.de.Data.Models;

namespace JonasWahle.de.Domain.Interfaces
{
    public interface IAuthStateService
    {
        User? CurrentUser { get; }
        bool IsAuthenticated { get; }

        event Action<bool>? AuthStateChanged;

        Task<bool> CheckAuthStateAsync();
        Task<bool> LoginAsync(string username, string password);
        Task LogoutAsync();
    }
}