using JonasWahle.de.Data;
using JonasWahle.de.Data.Models;
using JonasWahle.de.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JonasWahle.de.Domain.Services
{
    public class DownloadItemService(IDbContextFactory<ApplicationContext> DbFactory) : IDownloadItemService
    {
        public async Task<List<DownloadItem>> GetAllDownloadItemsAsync()
        {
            using ApplicationContext context = await DbFactory.CreateDbContextAsync();
            return await context.DownloadItems
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();
        }

        public async Task<DownloadItem?> GetDownloadItemByIdAsync(Guid id)
        {
            using ApplicationContext context = await DbFactory.CreateDbContextAsync();
            return await context.DownloadItems
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<List<DownloadItem>> GetDownloadItemsByCategoryAsync(string category)
        {
            using ApplicationContext context = await DbFactory.CreateDbContextAsync();
            return await context.DownloadItems
                .Where(x => x.Category == category && x.IsActive)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();
        }

        public async Task<DownloadItem> CreateDownloadItemAsync(DownloadItem downloadItem)
        {
            using ApplicationContext context = await DbFactory.CreateDbContextAsync();
            context.DownloadItems.Add(downloadItem);
            await context.SaveChangesAsync();
            return downloadItem;
        }

        public async Task<bool> UpdateDownloadItemAsync(DownloadItem updatedItem)
        {
            if (updatedItem == null)
            {
                return false;
            }

            using ApplicationContext context = await DbFactory.CreateDbContextAsync();

            // Get existing item
            DownloadItem? existingItem = await context.DownloadItems
                .FirstOrDefaultAsync(x => x.Id == updatedItem.Id);
            
            if (existingItem == null)
            {
                return false;
            }

            // Properly update existing entity with updatedItem
            context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
            
            await context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteDownloadItemAsync(Guid id)
        {
            using ApplicationContext context = await DbFactory.CreateDbContextAsync();
            DownloadItem? item = await context.DownloadItems.FindAsync(id);
            if (item != null)
            {
                item.IsActive = false; // Soft delete
                await context.SaveChangesAsync();
            }
        }
    }
}
