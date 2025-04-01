using ICS_Project.Common.Tests.Seeds;
using ICS_Project.DAL;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.Common.Tests;

public class MusicTestingDbContext(DbContextOptions contextOptions, bool seedTestingData = false)
    : MusicDbContext(contextOptions)
{
    protected void OnModelCreating(MusicDbContext modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);

        if (seedTestingData)
        {
            ArtistSeeds.SeedArtists(modelBuilder);
        }
    }
}