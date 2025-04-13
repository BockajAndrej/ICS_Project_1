using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Factories;
using ICS_Project.DAL;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Factories;
using ICS_Project.DAL.Repositories;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

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

        DbContextFactory = new DbContextSqLiteTestingFactory(GetType().FullName!, seedTestingData: true);
        UnitOfWorkFactory = new UnitOfWorkFactory(DbContextFactory);

        var services = new ServiceCollection();
        
        services.AddSingleton(DbContextFactory);
        services.AddSingleton<IUnitOfWorkFactory>(UnitOfWorkFactory);
        
        services.AddTransient<IArtistModelMapper, ArtistModelMapper>();
        services.AddTransient<IMusicTrackModelMapper, MusicTrackModelMapper>();
        services.AddTransient<IGenreModelMapper, GenreModelMapper>();
        services.AddTransient<IPlaylistModelMapper, PlaylistModelMapper>();
        
        services.AddTransient<Lazy<IArtistModelMapper>>(sp =>
            new Lazy<IArtistModelMapper>(() => sp.GetRequiredService<IArtistModelMapper>()));
        services.AddTransient<Lazy<IMusicTrackModelMapper>>(sp =>
            new Lazy<IMusicTrackModelMapper>(() => sp.GetRequiredService<IMusicTrackModelMapper>()));
        services.AddTransient<Lazy<IGenreModelMapper>>(sp =>
            new Lazy<IGenreModelMapper>(() => sp.GetRequiredService<IGenreModelMapper>()));
        services.AddTransient<Lazy<IPlaylistModelMapper>>(sp =>
            new Lazy<IPlaylistModelMapper>(() => sp.GetRequiredService<IPlaylistModelMapper>()));
        
        services.AddTransient<IArtistFacade, ArtistFacade>();
        services.AddTransient<IMusicTrackFacade, MusicTrackFacade>();
        services.AddTransient<IGenreFacade, GenreFacade>();
        services.AddTransient<IPlaylistFacade, PlaylistFacade>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
        await dbx.Database.EnsureCreatedAsync();
        
        
    }

    public async Task DisposeAsync()
    {
        if (ServiceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
    }
}