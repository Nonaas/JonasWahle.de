using JonasWahle.de.Data;
using Microsoft.EntityFrameworkCore;

namespace JonasWahle.de.UnitTests.Helper
{
    public class TestDbContextFactory : IDbContextFactory<ApplicationContext>
    {
        private readonly string _databaseName;

        public TestDbContextFactory()
        {
            // Use same database for all contexts created by the same factory instance
            _databaseName = $"DB_{Guid.CreateVersion7()}";
        }

        public ApplicationContext CreateDbContext()
        {
            DbContextOptions<ApplicationContext> options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options;

            ApplicationContext context = new(options);

            return context;
        }
    }
}
