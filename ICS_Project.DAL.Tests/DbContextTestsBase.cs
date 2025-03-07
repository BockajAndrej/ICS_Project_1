using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using Xunit.Abstractions;
using ICS_Project.DAL.Factories;

namespace ICS_Project.DAL.Tests;

public class DbContextTestsBase : IAsyncLifetime
{
    protected DbContextTestsBase(ITestOutputHelper output)
    {
        // Redirects test output to XUnit
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);

        // Use the factory to create a test database context
        DbContextFactory = new MusicDbContextFactory();
        MusicDbContextSUT = DbContextFactory.CreateDbContext(Array.Empty<string>());
    }

    protected MusicDbContextFactory DbContextFactory { get; }
    protected MusicDbContext MusicDbContextSUT { get; }


    public async Task InitializeAsync()
    {
        //Deletes and then Creates clean Database
        await MusicDbContextSUT.Database.EnsureDeletedAsync();
        await MusicDbContextSUT.Database.EnsureCreatedAsync();

        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        dbx.SeedGenres()
            .SeedMusicTracks()
            .SeedArtists()
            .SeedPlaylists();
        await dbx.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await MusicDbContextSUT.Database.EnsureDeletedAsync();
        await MusicDbContextSUT.DisposeAsync();
    }
}