using AwesomeAssertions;
using JonasWahle.de.Data;
using JonasWahle.de.Data.Enums.DownloadItem;
using JonasWahle.de.Data.Models;
using JonasWahle.de.Domain.Services;
using JonasWahle.de.UnitTests.Helper;
using Xunit;

namespace JonasWahle.de.UnitTests.Domain.Services
{
    public class DownloadServiceTests
    {
        [Fact]
        public async Task GetAllDownloadItemsAsync_ShouldReturnOnlyActiveItems()
        {
            // Arrange
            var activeItem = CreateTestDownloadItem("Active Item", true);
            var inactiveItem = CreateTestDownloadItem("Inactive Item", false);

            TestDbContextFactory factory = new();
            using ApplicationContext context = factory.CreateDbContext();
            context.DownloadItems.AddRange(activeItem, inactiveItem);
            await context.SaveChangesAsync();

            // Act
            DownloadItemService downloadService = new(factory);
            var result = await downloadService.GetAllDownloadItemsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Active Item");
            result.First().IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task GetDownloadItemByIdAsync_WithValidId_ShouldReturnItem()
        {
            // Arrange
            var item = CreateTestDownloadItem("Test Item", true);
            TestDbContextFactory factory = new();
            
            using (ApplicationContext setupContext = factory.CreateDbContext())
            {
                setupContext.DownloadItems.Add(item);
                await setupContext.SaveChangesAsync();
            }

            // Act
            DownloadItemService downloadService = new(factory);
            var result = await downloadService.GetDownloadItemByIdAsync(item.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("Test Item");
        }

        [Fact]
        public async Task GetDownloadItemByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            TestDbContextFactory factory = new();

            // Act
            DownloadItemService downloadService = new(factory);
            var result = await downloadService.GetDownloadItemByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateDownloadItemAsync_ShouldCreateAndReturnItem()
        {
            // Arrange
            var newItem = CreateTestDownloadItem("New Item", true);
            TestDbContextFactory factory = new();

            // Act
            DownloadItemService downloadService = new(factory);
            var result = await downloadService.CreateDownloadItemAsync(newItem);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("New Item");
            using ApplicationContext verificationContext = factory.CreateDbContext();
            var itemInDb = await verificationContext.DownloadItems.FindAsync(result.Id);
            itemInDb.Should().NotBeNull();
            itemInDb!.Title.Should().Be("New Item");
        }

        [Fact]
        public async Task DeleteDownloadItemAsync_ShouldMarkItemAsInactive()
        {
            // Arrange
            var item = CreateTestDownloadItem("Item to Delete", true);
            TestDbContextFactory factory = new();
            using (ApplicationContext setupContext = factory.CreateDbContext())
            {
                setupContext.DownloadItems.Add(item);
                await setupContext.SaveChangesAsync();
            }

            // Act
            DownloadItemService downloadService = new(factory);
            await downloadService.DeleteDownloadItemAsync(item.Id);

            // Assert
            using ApplicationContext verificationContext = factory.CreateDbContext();
            var deletedItem = await verificationContext.DownloadItems.FindAsync(item.Id);
            deletedItem.Should().NotBeNull();
            deletedItem!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task GetDownloadItemsByCategoryAsync_ShouldReturnItemsOfSpecificCategory()
        {
            // Arrange
            var gameItem = CreateTestDownloadItem("Game Item", true);
            gameItem.Category = "game";
            
            var appItem = CreateTestDownloadItem("App Item", true);
            appItem.Category = "app";

            TestDbContextFactory factory = new();
            using (ApplicationContext setupContext = factory.CreateDbContext())
            {
                setupContext.DownloadItems.AddRange(gameItem, appItem);
                await setupContext.SaveChangesAsync();
            }

            // Act
            DownloadItemService downloadService = new(factory);
            var result = await downloadService.GetDownloadItemsByCategoryAsync("game");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Category.Should().Be("game");
            result.First().Title.Should().Be("Game Item");
        }

        [Fact]
        public async Task UpdateDownloadItemAsync_ShouldUpdateAndReturnItem()
        {
            // Arrange
            var item = CreateTestDownloadItem("Original Title", true);

            TestDbContextFactory factory = new();
            using (ApplicationContext setupContext = factory.CreateDbContext())
            {
                setupContext.DownloadItems.Add(item);
                await setupContext.SaveChangesAsync();
            }

            item.Title = "Updated Title";
            item.Description = "Updated Description";

            // Act
            DownloadItemService downloadService = new(factory);
            bool result = await downloadService.UpdateDownloadItemAsync(item);

            // Assert
            result.Should().BeTrue();
            using ApplicationContext verificationContext = factory.CreateDbContext();
            var updatedItem = await verificationContext.DownloadItems.FindAsync(item.Id);
            updatedItem.Should().NotBeNull();
            updatedItem!.Title.Should().Be("Updated Title");
            updatedItem.Description.Should().Be("Updated Description");
        }

        private static DownloadItem CreateTestDownloadItem(string title, bool isActive)
        {
            return new DownloadItem
            {
                Title = title,
                Description = "Test Description",
                Category = "test",
                ImagePath = "/test.jpg",
                BackgroundColor = "#000000",
                FontColor = "#FFFFFF",
                Platforms = [Platform.Windows],
                ReleaseDate = DateTime.Now,
                DownloadUrl = "https://test.com",
                IsActive = isActive
            };
        }
    }
}
