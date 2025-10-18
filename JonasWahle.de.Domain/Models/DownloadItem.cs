using JonasWahle.de.Domain.Enums;

namespace JonasWahle.de.Domain.Models
{
    public class DownloadItem
    {
        public required string ImagePath { get; set; }
        public required string BackgroundColor { get; set; }
        public required string FontColor { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required List<PlatformEnum> Platforms { get; set; }
        public required DateTime ReleaseDate { get; set; }
        public required string DownloadUrl { get; set; }

        public List<string>? Tags { get; set; }
        public string? GitHubUrl { get; set; }
    }
}
