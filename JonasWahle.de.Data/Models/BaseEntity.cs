namespace JonasWahle.de.Data.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public DateTime InsertTimestamp { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
