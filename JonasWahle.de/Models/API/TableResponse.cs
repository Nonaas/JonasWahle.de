using System.Text.Json;

namespace JonasWahle.de.Models.API
{
    public class TableResponse
    {
        public string TableId { get; set; }
        public string Title { get; set; }
        public List<TableColumn> Columns { get; set; } = new();
        public List<Dictionary<string, JsonElement>> Data { get; set; } = new();
        public string Source { get; set; }
        public string DateStand { get; set; }
    }

}
