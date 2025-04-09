using System.Text.Json.Serialization;

namespace JonasWahle.de.Models
{
    public class QuoteModel
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;
    }
}
