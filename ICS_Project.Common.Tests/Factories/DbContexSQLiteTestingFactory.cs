using ICS_Project.DAL;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.Common.Tests.Factories;

public class DbContextSqLiteTestingFactory(string databaseName, bool seedTestingData = false)
    : IDbContextFactory<MusicDbContext>
{
    public MusicDbContext CreateDbContext()
    {
        DbContextOptionsBuilder<MusicDbContext> builder = new();
        builder.UseSqlite($"Data Source={databaseName};Cache=Shared");

        // builder.LogTo(Console.WriteLine); //Enable in case you want to see tests details, enabled may cause some inconsistencies in tests
        // builder.EnableSensitiveDataLogging();

        return new MusicTestingDbContext(builder.Options, seedTestingData);
    }
}