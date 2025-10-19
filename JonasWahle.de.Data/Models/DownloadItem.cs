using JonasWahle.de.Data.Enums.DownloadItem;

namespace JonasWahle.de.Data.Models
{
    public class DownloadItem : BaseEntity
    {
        public required string ImagePath { get; set; }
        public required string BackgroundColor { get; set; }
        public required string FontColor { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required List<Platform> Platforms { get; set; }
        public required DateTime ReleaseDate { get; set; }
        public required string DownloadUrl { get; set; }

        public List<Tag>? Tags { get; set; }
        public string? GitHubUrl { get; set; }
    }
}
