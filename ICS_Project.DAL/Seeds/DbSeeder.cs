using ICS_Project.DAL.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ICS_Project.DAL.Seeds;

public class DbSeeder(IDbContextFactory<MusicDbContext> dbContextFactory, IOptions<DALOptions> options)
    : IDbSeeder
{
    public void Seed()
    {
        using MusicDbContext dbContext = dbContextFactory.CreateDbContext();

        if(options.Value.SeedDemoData)
        {
            dbContext
                .SeedArtists()
                .SeedMusicTracks()
                .SeedGenre()
                .SeedPlaylists();
            dbContext.SaveChanges();
        }
    }
}