using JonasWahle.de.Utilities;

namespace JonasWahle.de.Models.API
{
    public class RequestTableDataModel
    {
        public string Token { get; set; } = Constants.API.Token;
        public required string TableCode { get; set; }
        public string? Area { get; set; }
        public string? Language { get; set; }
        public string? StartYear { get; set; }
        public string? EndYear { get; set; }
    }
}
