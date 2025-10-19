using AwesomeAssertions;
using JonasWahle.de.Data;
using JonasWahle.de.Data.Enums.DownloadItem;
using JonasWahle.de.Data.Models;
using JonasWahle.de.UnitTests.Helper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JonasWahle.de.UnitTests.Data
{
    public class ApplicationContextTests
    {
        [Fact]
        public async Task DownloadItem_PlatformsProperty_ShouldSerializeToJson()
        {
            // Arrange
            var item = new DownloadItem
            {
                Title = "Test Item",
                Description = "Test Description",
                Category = "test",
                ImagePath = "/test.jpg",
                BackgroundColor = "#000000",
                FontColor = "#FFFFFF",
                Platforms = [Platform.Windows, Platform.Android],
                ReleaseDate = DateTime.Now,
                DownloadUrl = "https://test.com"
            };

            // Act
            TestDbContextFactory factory = new();
            ApplicationContext context = factory.CreateDbContext();
            context.DownloadItems.Add(item);
            await context.SaveChangesAsync();

            // Assert
            var savedItem = await context.DownloadItems.FirstAsync();
            savedItem.Platforms.Should().HaveCount(2);
            savedItem.Platforms.Should().Contain(Platform.Windows);
            savedItem.Platforms.Should().Contain(Platform.Android);
        }

        [Fact]
        public async Task DownloadItem_TagsProperty_ShouldHandleNullValues()
        {
            // Arrange
            var item = new DownloadItem
            {
                Title = "Test Item",
                Description = "Test Description",
                Category = "test",
                ImagePath = "/test.jpg",
                BackgroundColor = "#000000",
                FontColor = "#FFFFFF",
                Platforms = [Platform.Windows],
                ReleaseDate = DateTime.Now,
                DownloadUrl = "https://test.com",
                Tags = null
            };

            // Act
            TestDbContextFactory factory = new();
            ApplicationContext context = factory.CreateDbContext();
            context.DownloadItems.Add(item);
            await context.SaveChangesAsync();

            // Assert
            var savedItem = await context.DownloadItems.FirstAsync();
            savedItem.Tags.Should().BeNull();
        }

    }
}
