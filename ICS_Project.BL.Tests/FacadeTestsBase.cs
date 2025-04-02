using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Factories;
using ICS_Project.DAL;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Factories;
using ICS_Project.DAL.Repositories;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace ICS_Project.BL.Tests;

public class FacadeTestsBase: IAsyncLifetime
{
    protected ArtistModelMapper ArtistModelMapper { get; }
    protected GenreModelMapper GenreModelMapper { get; }
    protected MusicTrackModelMapper MusicTrackModelMapper { get; }
    protected PlaylistModelMapper PlaylistModelMapper { get; }
    
    protected UnitOfWorkFactory UnitOfWorkFactory { get; }
    protected IDbContextFactory<MusicDbContext> DbContextFactory { get; }
    
    protected FacadeTestsBase(ITestOutputHelper output)
    {
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);
        
        DbContextFactory = new DbContextSqLiteTestingFactory(GetType().FullName!, seedTestingData: true);
        
        ArtistModelMapper = new ArtistModelMapper();
        GenreModelMapper = new GenreModelMapper();
        MusicTrackModelMapper = new MusicTrackModelMapper();
        PlaylistModelMapper = new PlaylistModelMapper();
        
        UnitOfWorkFactory = new UnitOfWorkFactory(DbContextFactory);
    }

    public async Task InitializeAsync()
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
        await dbx.Database.EnsureCreatedAsync();
        
        
    }

    public async Task DisposeAsync()
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
    }
}