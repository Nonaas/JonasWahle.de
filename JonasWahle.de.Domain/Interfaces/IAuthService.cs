using JonasWahle.de.Data.Models;
using JonasWahle.de.Domain.Models.Auth;

namespace JonasWahle.de.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<Session> CreateSessionAsync(Guid userId);
        Task<Session?> GetSessionAsync(Guid sessionId);
        Task InvalidateSessionAsync(Guid sessionId);
        Task<bool> IsSessionValidAsync(Guid sessionId);
        Task UpdateSessionActivityAsync(Guid sessionId);
        Task<User?> ValidateCredentialsAsync(string username, string password);
    }
}