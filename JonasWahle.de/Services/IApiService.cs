using JonasWahle.de.Models.API;

namespace JonasWahle.de.Services
{
    public interface IApiService
    {
        string FormatValue(object value, TableColumn column);
        Task<TableResponse> RequestTableDataAsync(TableRequest requestModel);
        Task<bool> TestConnectionAsync();
    }
}