using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ICS_Project.DAL.Factories;

public class MusicDbContextFactory : IDesignTimeDbContextFactory<MusicDbContext>
{
    private readonly bool _seedTestingData;
    public MusicDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MusicDbContext>();
        builder.UseSqlite("Data Source=music.db");

        return new MusicDbContext(builder.Options, _seedTestingData);
    }
}