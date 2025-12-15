namespace Test.Util;

public class Seeder : ISeeder
{
    public Task Seed()
    {
        throw new NotImplementedException();
    /**public class DatabaseFixture : IAsyncLifetime
    {
        public TestDbContext DbContext { get; private set; }

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=GameTestsDb;Trusted_Connection=True;")
                .Options;

            DbContext = new TestDbContext(options);

            // 1. Create DB + apply migrations
            await DbContext.Database.MigrateAsync();

            // 2. Seed from SQL file
            await SeedDatabaseAsync(DbContext);
        }

        public async Task DisposeAsync()
        {
            await DbContext.Database.EnsureDeletedAsync();
        }

        private static async Task SeedDatabaseAsync(DbContext context)
        {
            var sql = await File.ReadAllTextAsync("SeedData/seed.sql");
            await context.Database.ExecuteSqlRawAsync(sql);
        }
    }*/

    }
}