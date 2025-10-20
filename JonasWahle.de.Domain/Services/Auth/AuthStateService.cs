using JonasWahle.de.Data.Models;
using JonasWahle.de.Domain.Interfaces;
using JonasWahle.de.Domain.Models.Auth;
using Microsoft.Extensions.Logging;
using static JonasWahle.de.Domain.Utilities.Constants;

namespace JonasWahle.de.Domain.Services.Auth
{
    public class AuthStateService(ILogger<AuthStateService> Logger, IAuthService authService, ICookieService cookieService) : IAuthStateService
    {
        public event Action<bool>? AuthStateChanged;
        public bool IsAuthenticated { get; private set; }
        public User? CurrentUser { get; private set; }


        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                User? loggedInUser = await authService.ValidateCredentialsAsync(username, password);
                if (loggedInUser != null)
                {
                    Session session = await authService.CreateSessionAsync(loggedInUser.Id);
                    await cookieService.SetCookieAsync(CookieKeys.SessionCookie, session.SessionId.ToString(), 1); // 1 day expiry

                    IsAuthenticated = true;
                    CurrentUser = loggedInUser;
                    AuthStateChanged?.Invoke(IsAuthenticated);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Login failed for user {0}", username);
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            try
            {
                string? sessionCookieId = await cookieService.GetCookieAsync(CookieKeys.SessionCookie);
                if (Guid.TryParse(sessionCookieId, out Guid sessionId))
                {
                    await authService.InvalidateSessionAsync(sessionId);
                    await cookieService.DeleteCookieAsync(CookieKeys.SessionCookie);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Logout failed");
            }

            IsAuthenticated = false;
            CurrentUser = null;
            AuthStateChanged?.Invoke(IsAuthenticated);
        }

        public async Task<bool> CheckAuthStateAsync()
        {
            try
            {
                string? sessionCookieId = await cookieService.GetCookieAsync(CookieKeys.SessionCookie);
                if (Guid.TryParse(sessionCookieId, out Guid sessionId))
                {
                    Session? session = await authService.GetSessionAsync(sessionId);
                    if (session != null && session.IsActive)
                    {
                        await authService.UpdateSessionActivityAsync(sessionId);
                        IsAuthenticated = true;
                        CurrentUser = session.User;
                        AuthStateChanged?.Invoke(IsAuthenticated);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CheckAuthStateAsync failed");
            }

            IsAuthenticated = false;
            CurrentUser = null;
            AuthStateChanged?.Invoke(IsAuthenticated);
            return false;
        }
    }
}
