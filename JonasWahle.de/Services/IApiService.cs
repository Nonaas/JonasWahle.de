using JonasWahle.de.Models.API;

namespace JonasWahle.de.Services
{
    public interface IApiService
    {
        Task<List<ParsedTableRow>> RequestTableData(RequestTableDataModel requestModel);
        Task<bool> TestConnection();
    }
}