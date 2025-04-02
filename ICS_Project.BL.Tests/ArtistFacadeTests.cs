using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Models;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.Repositories;
using ICS_Project.DAL.UnitOfWork;
using Xunit.Abstractions;

namespace ICS_Project.BL.Tests;

public class ArtistFacadeTests : FacadeTestsBase
{
    private readonly IArtistFacade _facadeSUT;

    public ArtistFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = new ArtistFacade(UnitOfWorkFactory, ArtistModelMapper);
    }
    
    //Seeded tests
    [Fact]
    public async Task Save_Test()
    {
        var detailModel = ArtistModelMapper.MapToDetailModel(ArtistSeeds.Artist);

        var returnedModel = await _facadeSUT.SaveAsync(detailModel);

        FixIds(detailModel, returnedModel);
        DeepAssert.Equal(detailModel, returnedModel);
    }
    
    [Fact]
    public async Task GetID_OneToOne_Test()
    {
        //We need to populate the database without a facade SaveAsync method
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        repository.Insert(ArtistSeeds.Artist);
        uow.CommitAsync();
        
        var detailModel = ArtistModelMapper.MapToDetailModel(ArtistSeeds.Artist);

        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        DeepAssert.Equal(detailModel, returnedModel);
    }
    [Fact]
    public async Task GetID_OneToMany_Test()
    {
        //We need to populate the database without a facade SaveAsync method
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        int numOfArtists = 5;
        var lastArtist = FillArtistDatabase(numOfArtists, repository, uow);
        
        var detailModel = ArtistModelMapper.MapToDetailModel(lastArtist.Pop());

        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task Get_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        int numOfArtists = 5;
        FillArtistDatabase(numOfArtists, repository, uow);
        
        var ArtistList = await _facadeSUT.GetAsync();
        foreach (var artist in ArtistList)
        {
            numOfArtists--;
            var currentModel = ArtistModelMapper.MapToDetailModel(ArtistSeeds.Artist);
            var databaseModel = ArtistModelMapper.MapToDetailModel(ArtistSeeds.Artist);
            DeepAssert.Equal(currentModel, databaseModel);
        }
        //Check that number of inserted artist is correct 
        Assert.Equal(0, numOfArtists);
    }

    [Fact]
    public async Task Delete_OneToOne_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        int numOfArtists = 1;
        var currArtist = FillArtistDatabase(numOfArtists, repository, uow);
        
        var derailModel = ArtistModelMapper.MapToDetailModel(currArtist.Pop());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var artists = await _facadeSUT.GetAsync();
        
        Assert.True(!artists.Any());
    }
    [Fact]
    public async Task Delete_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        int numOfArtists = 5;
        var currArtist = FillArtistDatabase(numOfArtists, repository, uow);

        for (int i = 0; i < numOfArtists; i++)
        {
            var detailModel = ArtistModelMapper.MapToDetailModel(currArtist.Pop());
            await _facadeSUT.DeleteAsync(detailModel.Id);
        }

        var artists = await _facadeSUT.GetAsync();
        
        Assert.True(!artists.Any());
    }
    [Fact]
    public async Task Delete_OneToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        int numOfArtists = 5;
        var lastArtist = FillArtistDatabase(numOfArtists, repository, uow);
        
        var derailModel = ArtistModelMapper.MapToDetailModel(lastArtist.Peek());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var artist = await _facadeSUT.GetAsync(derailModel.Id);
        
        Assert.Null(artist);
    }
    
    
    private static Stack<Artist> FillArtistDatabase(int numOfArtists, IRepository<Artist> repository, IUnitOfWork uow)
    {
        Stack<Artist> artists = new();
        for (int i = 0; i < numOfArtists; i++)
        {
            artists.Push(ArtistSeeds.ArtistClone($"58D0C03C-C539-4A16-96B1-9A95AAAAAAA{i}", $"Name= {i}{i}"));
            repository.Insert(artists.Peek());
            uow.CommitAsync();
        }
        return artists;
    }
    
    private static void FixIds(ArtistDetailModel expectedModel, ArtistDetailModel returnedModel)
    {
        returnedModel.Id = expectedModel.Id;
    }
}