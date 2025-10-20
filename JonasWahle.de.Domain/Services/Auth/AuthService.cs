using JonasWahle.de.Data;
using JonasWahle.de.Data.Enums;
using JonasWahle.de.Data.Models;
using JonasWahle.de.Domain.Interfaces;
using JonasWahle.de.Domain.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace JonasWahle.de.Domain.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IDbContextFactory<ApplicationContext> _dbFactory;
        private readonly ConcurrentDictionary<Guid, Session> _sessions = new();
        private readonly TimeSpan _sessionTimeout = TimeSpan.FromHours(2);
        private readonly Timer _cleanupTimer;


        public AuthService(ILogger<AuthService> logger, IDbContextFactory<ApplicationContext> dbFactory)
        {
            _logger = logger;
            _dbFactory = dbFactory;

            // Cleanup expired sessions every 30 minutes
            _cleanupTimer = new Timer(CleanupExpiredSessions, null, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
        }

        public async Task<User?> ValidateCredentialsAsync(string username, string password)
        {
            ApplicationContext context = await _dbFactory.CreateDbContextAsync();
            User? possibleUser = await context.Users
                .AsNoTracking()
                .Where(x => x.IsActive &&
                            x.Role == Role.Admin &&
                            x.Username == username)
                .FirstOrDefaultAsync();

            if (possibleUser != null)
            {
                if (possibleUser.Password == ComputeHash(password))
                {
                    return possibleUser;
                }
                else
                {
                    _logger.LogWarning("Invalid password for admin user {0}", username);
                    throw new UnauthorizedAccessException("Invalid password");
                }
            }
            else
            {
                _logger.LogWarning("No active admin user found with username {0}", username);
                throw new UnauthorizedAccessException("Invalid username");
            }
        }

        public async Task<Session> CreateSessionAsync(Guid userId)
        {
            await Task.CompletedTask;

            Session session = new()
            {
                UserId = userId,
                SessionId = Guid.CreateVersion7(),
                CreatedAt = DateTime.Now,
                LastActivity = DateTime.Now,
                IsActive = true
            };

            _sessions[session.SessionId] = session;
            return session;
        }

        public async Task<Session?> GetSessionAsync(Guid sessionId)
        {
            using ApplicationContext context = await _dbFactory.CreateDbContextAsync();
            
            if (_sessions.TryGetValue(sessionId, out Session? session))
            {
                if (IsSessionExpired(session))
                {
                    _sessions.TryRemove(sessionId, out _);
                    return null;
                }
                else
                {
                    // Check for user in memory
                    List<User> allUsers = await context.Users.ToListAsync();
                    User? sessionUser = allUsers.FirstOrDefault(x => x.Id == session.UserId);

                    if (sessionUser == null)
                    {
                        _logger.LogWarning("No user found with ID '{0}' for session '{1}'", session.UserId, session.SessionId);
                    }

                    session.User = sessionUser;
                    return session;
                }
            }
            
            _logger.LogDebug("Session not found in memory: {0}", sessionId);
            return null;
        }

        public async Task UpdateSessionActivityAsync(Guid sessionId)
        {
            await Task.CompletedTask;

            if (_sessions.TryGetValue(sessionId, out Session? session))
            {
                session.LastActivity = DateTime.Now;
            }
        }

        public async Task InvalidateSessionAsync(Guid sessionId)
        {
            await Task.CompletedTask;
            _sessions.TryRemove(sessionId, out _);
        }

        public async Task<bool> IsSessionValidAsync(Guid sessionId)
        {
            Session? session = await GetSessionAsync(sessionId);
            return session != null && session.IsActive && !IsSessionExpired(session);
        }

        private bool IsSessionExpired(Session session)
        {
            return DateTime.Now - session.LastActivity > _sessionTimeout;
        }

        private static string ComputeHash(string input)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        private void CleanupExpiredSessions(object? state)
        {
            List<KeyValuePair<Guid, Session>> expiredSessions = _sessions
                .Where(kvp => IsSessionExpired(kvp.Value))
                .ToList();

            foreach (KeyValuePair<Guid, Session> session in expiredSessions)
            {
                _sessions.TryRemove(session.Key, out _);
                _logger.LogDebug("Cleaned up expired session {0}", session.Key);
            }
        }
    }
}
