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
    protected UnitOfWorkFactory UnitOfWorkFactory { get; }
    protected IDbContextFactory<MusicDbContext> DbContextFactory { get; }
    
    protected FacadeTestsBase(ITestOutputHelper output)
    {
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);

        // DbContextFactory = new DbContextTestingInMemoryFactory(GetType().Name, seedTestingData: true);
        // DbContextFactory = new DbContextLocalDBTestingFactory(GetType().FullName!, seedTestingData: true);
        DbContextFactory = new DbContextSqLiteTestingFactory(GetType().FullName!, seedTestingData: true);

        /*IngredientModelMapper = new IngredientModelMapper();
        IngredientAmountModelMapper = new IngredientAmountModelMapper();
        RecipeModelMapper = new RecipeModelMapper(IngredientAmountModelMapper);*/
        
        ArtistModelMapper = new ArtistModelMapper();

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