using JonasWahle.de.Data.Models;

namespace JonasWahle.de.Domain.Interfaces
{
    public interface IDownloadItemService
    {
        Task<DownloadItem> CreateDownloadItemAsync(DownloadItem downloadItem);
        Task DeleteDownloadItemAsync(Guid id);
        Task<List<DownloadItem>> GetAllDownloadItemsAsync();
        Task<DownloadItem?> GetDownloadItemByIdAsync(Guid id);
        Task<List<DownloadItem>> GetDownloadItemsByCategoryAsync(string category);
        Task<DownloadItem> UpdateDownloadItemAsync(DownloadItem downloadItem);
    }
}