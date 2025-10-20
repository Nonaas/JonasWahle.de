using JonasWahle.de.Data.Enums.DownloadItem;
using JonasWahle.de.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JonasWahle.de.Data
{
    public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
    {
        public DbSet<DownloadItem> DownloadItems => Set<DownloadItem>();

        public DbSet<SmtpSetting> SmtpSettings => Set<SmtpSetting>();


        /*
         * Create migration and update db:
         * dotnet ef migrations add "Move SmtpSetting from appsettings.json to local DB" --project JonasWahle.de.Data --startup-project JonasWahle.de.UI
         * dotnet ef database update --project JonasWahle.de.Data --startup-project JonasWahle.de.UI
         * 
         * Remove last migration (if not applied):
         * dotnet ef migrations remove --project JonasWahle.de.Data --startup-project JonasWahle.de.UI
         * 
         * List migrations:
         * dotnet ef migrations list --project JonasWahle.de.Data --startup-project JonasWahle.de.UI
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DownloadItem>().ToTable("JW_DownloadItem").HasKey(x => x.Id);

            // json options for string enums
            JsonSerializerOptions jsonOptions = new()
            {
                Converters = { new JsonStringEnumConverter() }
            };

            // Convert list to json with value comparer
            modelBuilder.Entity<DownloadItem>()
                .Property(x => x.Platforms)
                .HasConversion(
                    list => JsonSerializer.Serialize(list, jsonOptions),
                    json => JsonSerializer.Deserialize<List<Platform>>(json, jsonOptions) ?? new List<Platform>()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<Platform>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

            // Convert list to json with value comparer
            modelBuilder.Entity<DownloadItem>()
                .Property(x => x.Tags)
                .HasConversion(
                    list => JsonSerializer.Serialize(list ?? new List<Tag>(), jsonOptions),
                    json => JsonSerializer.Deserialize<List<Tag>>(json, jsonOptions) ?? new List<Tag>()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<Tag>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

            modelBuilder.Entity<SmtpSetting>().ToTable("JW_SmtpSetting").HasKey(x => x.Id);
        }

    }
}
