namespace JonasWahle.de.Models.API
{
    public class TableRequest
    {
        public required string TableCode { get; set; }
        public string? Token { get; set; }
        public string? Area { get; set; }
        public string? Language { get; set; }
        public string? StartYear { get; set; }
        public string? EndYear { get; set; }
    }
}
