using System.Collections.ObjectModel;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.Repositories;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ICS_Project.BL.Tests;

public class ArtistFacadeTests : FacadeTestsBase
{
    private readonly IArtistFacade _facadeSUT;
    private readonly IArtistModelMapper _artistModelMapper;
    private readonly IMusicTrackModelMapper _musicTrackModelMapper;

    public ArtistFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = ServiceProvider.GetRequiredService<IArtistFacade>();
        _artistModelMapper = ServiceProvider.GetRequiredService<IArtistModelMapper>();
        _musicTrackModelMapper = ServiceProvider.GetRequiredService<IMusicTrackModelMapper>();
    }
    
    [Fact]
    public async Task Create_WithNonExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = new ArtistDetailModel()
        {
            Id = Guid.NewGuid(),
            ArtistName = "Brumbex",
            MusicTrack = new ObservableCollection<MusicTrackListModel>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = string.Empty,
                    Length = TimeSpan.Zero,
                    Size = 0,
                    Title = "Honba za pramenem okeny",
                    UrlAddress = string.Empty,
                }
            },
        };
        
        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(detailModel));
    }
    
    //Seeded tests
    [Fact]
    public async Task Save_Test()
    {
        var detailModel = _artistModelMapper.MapToDetailModel(ArtistSeeds.ArtistWOutTracks);

        var returnedModel = await _facadeSUT.SaveAsync(detailModel);

        FixIds(detailModel, returnedModel); // What is this good for? <Vitan_, AKA xkolosv00>
        DeepAssert.Equal(detailModel, returnedModel);
    }
    
    [Fact]
    public async Task Create_WithExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = _artistModelMapper.MapToDetailModel(ArtistSeeds.Artist);
        
        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(detailModel));
    }
    
    [Fact]
    public async Task Create_WithAndWoutExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = new ArtistDetailModel()
        {
            Id = Guid.NewGuid(),
            ArtistName = "Horacio Junior",
            MusicTrack =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = string.Empty,
                    Length = TimeSpan.Zero,
                    Size = 0,
                    Title = "Bradavicky zlodej",
                    UrlAddress = string.Empty,
                },
            
                _musicTrackModelMapper.MapToListModel(MusicTrackSeeds.NonEmptyMusicTrack1)
            ],
        };
        
        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(detailModel));
    }
    
    [Fact]
    public async Task GetID_OneToOne_Test()
    {
        var detailModel = _artistModelMapper.MapToDetailModel(ArtistSeeds.Artist);

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
        
        var detailModel = _artistModelMapper.MapToDetailModel(lastArtist.Dequeue());

        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task Delete_OneToOne_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        var artistsStart = await _facadeSUT.GetAsync();
        
        int numOfArtists = 1;
        var currArtist = FillArtistDatabase(numOfArtists, repository, uow);
        
        var derailModel = _artistModelMapper.MapToDetailModel(currArtist.Dequeue());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var artists = await _facadeSUT.GetAsync();
        
        DeepAssert.Equal(artistsStart, artists);
    }
    [Fact]
    public async Task Delete_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        var artistsStart = await _facadeSUT.GetAsync();
        
        int numOfArtists = 5;
        var currArtist = FillArtistDatabase(numOfArtists, repository, uow);

        for (int i = 0; i < numOfArtists; i++)
        {
            var detailModel = _artistModelMapper.MapToDetailModel(currArtist.Dequeue());
            await _facadeSUT.DeleteAsync(detailModel.Id);
        }

        var artists = await _facadeSUT.GetAsync();
        
        DeepAssert.Equal(artistsStart, artists);
    }
    [Fact]
    public async Task Delete_OneToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        int numOfArtists = 5;
        var firstArtist = FillArtistDatabase(numOfArtists, repository, uow);
        
        var derailModel = _artistModelMapper.MapToDetailModel(firstArtist.Peek());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var artist = await _facadeSUT.GetAsync(derailModel.Id);
        
        Assert.Null(artist);
    }
    
    
    private static Queue<Artist> FillArtistDatabase(int numOfArtists, IRepository<Artist> repository, IUnitOfWork uow)
    {
        Queue<Artist> artists = new();
        for (int i = 0; i < numOfArtists; i++)
        {
            var artistToSave = ArtistSeeds.ArtistClone($"58D0C03C-C539-4A16-96B1-9A95AAAAAAA{i}", $"Name= {i}{i}");
            artists.Enqueue(artistToSave);
            repository.Insert(artistToSave);
            uow.CommitAsync();
        }
        return artists;
    }
    
    //Check if is 
    private static void FixIds(ArtistDetailModel expectedModel, ArtistDetailModel returnedModel) //TODO: Delete this, as it has no usage? <Vitan_, AKA xkolosv00>
    {
        returnedModel.Id = expectedModel.Id;
        foreach (var musicTrackModel in returnedModel.MusicTrack)
        {
            var musicTrackDetailModel = expectedModel.MusicTrack.FirstOrDefault(i =>
                i.Title == musicTrackModel.Title
                && i.Description == musicTrackModel.Description
                && i.Length == musicTrackModel.Length
                && Math.Abs(i.Size - musicTrackModel.Size) < 0.00001
                && i.UrlAddress == musicTrackModel.UrlAddress);

            if (musicTrackDetailModel != null)
            {
                musicTrackModel.Id = musicTrackDetailModel.Id;
            }
        }
    }
}