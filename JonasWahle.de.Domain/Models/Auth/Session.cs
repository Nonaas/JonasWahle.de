using JonasWahle.de.Data.Models;

namespace JonasWahle.de.Domain.Models.Auth
{
    public class Session
    {
        public Guid SessionId { get; set; } = Guid.CreateVersion7();
        public required Guid UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastActivity { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
