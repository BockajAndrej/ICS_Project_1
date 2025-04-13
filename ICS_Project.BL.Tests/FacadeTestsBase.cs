using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Factories;
using ICS_Project.DAL;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using ICS_Project.Common.Tests.Seeds; // << *** ADD THIS NAMESPACE ***

namespace ICS_Project.BL.Tests;

public class FacadeTestsBase: IAsyncLifetime
{
    protected IServiceProvider ServiceProvider { get; }

    protected UnitOfWorkFactory UnitOfWorkFactory { get; }
    protected IDbContextFactory<MusicDbContext> DbContextFactory { get; }

    protected FacadeTestsBase(ITestOutputHelper output)
    {
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);

        // Use the test-specific SQLite factory, seeding enabled by default? (true)
        DbContextFactory = new DbContextSqLiteTestingFactory(GetType().FullName!, seedTestingData: true);
        UnitOfWorkFactory = new UnitOfWorkFactory(DbContextFactory);

        var services = new ServiceCollection();

        services.AddSingleton(DbContextFactory);
        services.AddSingleton<IUnitOfWorkFactory>(UnitOfWorkFactory);

        // Register Mappers
        services.AddTransient<IArtistModelMapper, ArtistModelMapper>();
        services.AddTransient<IMusicTrackModelMapper, MusicTrackModelMapper>();
        services.AddTransient<IGenreModelMapper, GenreModelMapper>();
        services.AddTransient<IPlaylistModelMapper, PlaylistModelMapper>();

        // Register Lazy Mappers (if needed by facades)
        services.AddTransient<Lazy<IArtistModelMapper>>(sp =>
            new Lazy<IArtistModelMapper>(() => sp.GetRequiredService<IArtistModelMapper>()));
        services.AddTransient<Lazy<IMusicTrackModelMapper>>(sp =>
            new Lazy<IMusicTrackModelMapper>(() => sp.GetRequiredService<IMusicTrackModelMapper>()));
        services.AddTransient<Lazy<IGenreModelMapper>>(sp =>
            new Lazy<IGenreModelMapper>(() => sp.GetRequiredService<IGenreModelMapper>()));
        services.AddTransient<Lazy<IPlaylistModelMapper>>(sp =>
            new Lazy<IPlaylistModelMapper>(() => sp.GetRequiredService<IPlaylistModelMapper>()));

        // Register Facades
        services.AddTransient<IArtistFacade, ArtistFacade>();
        services.AddTransient<IMusicTrackFacade, MusicTrackFacade>();
        services.AddTransient<IGenreFacade, GenreFacade>();
        services.AddTransient<IPlaylistFacade, PlaylistFacade>();

        ServiceProvider = services.BuildServiceProvider();
    }

    // This method runs ONCE before all tests in the class
    public async Task InitializeAsync()
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        // Ensure clean state
        await dbx.Database.EnsureDeletedAsync();
        await dbx.Database.EnsureCreatedAsync();

        // *** ADD SEEDING LOGIC HERE ***
        // Use the extension methods from ICS_Project.Common.Tests.Seeds
        Console.WriteLine("--- Seeding database for tests ---"); // Optional: Log seeding start
        dbx.SeedArtists();   // Assuming this name exists in ArtistSeeds
        dbx.SeedGenres();    // Assuming this name exists in GenreSeeds
        dbx.SeedPlaylists(); // Assuming this name exists in PlaylistSeeds
        dbx.SeedMusicTracks(); // Assuming this name exists in MusicTrackSeeds
        // Add any other seed methods needed (e.g., for relationships if not handled by entity seeds)

        // *** CRUCIAL STEP: Save the seeded data ***
        await dbx.SaveChangesAsync();
        Console.WriteLine("--- Seeding complete ---"); // Optional: Log seeding end

    }

    // This method runs ONCE after all tests in the class
    public async Task DisposeAsync()
    {
        // Dispose ServiceProvider if necessary
        if (ServiceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        // Clean up the database
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
    }
}