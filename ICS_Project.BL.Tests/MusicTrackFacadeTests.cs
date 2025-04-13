using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.BL.Tests;

public class MusicTrackFacadeTests : FacadeTestsBase
{
    private readonly IMusicTrackFacade _facadeSUT;
    private readonly IMusicTrackModelMapper _musicTrackModelMapper;
    private readonly IGenreModelMapper _genreModelMapper;
    private readonly IArtistModelMapper _artistModelMapper;
    private readonly IPlaylistModelMapper _playlistModelMapper;

    public MusicTrackFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = ServiceProvider.GetRequiredService<IMusicTrackFacade>();
        _musicTrackModelMapper = ServiceProvider.GetRequiredService<IMusicTrackModelMapper>();
        _genreModelMapper = ServiceProvider.GetRequiredService<IGenreModelMapper>();
        _artistModelMapper = ServiceProvider.GetRequiredService<IArtistModelMapper>();
        _playlistModelMapper = ServiceProvider.GetRequiredService<IPlaylistModelMapper>();
    }
    
    //Seeded tests
    [Fact]
    public async Task Create_WithExistingArtists_Genres_Playlists_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = _musicTrackModelMapper.MapToDetailModel(MusicTrackSeeds.NonEmptyMusicTrack2);
        
        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(detailModel));
    }

    private static void FixIds(MusicTrackDetailModel expectedModel, MusicTrackDetailModel returnedModel) //TODO: Delete this, as it has no usage? <Vitan_, AKA xkolosv00>
    {
        returnedModel.Id = expectedModel.Id;
    }
    
    [Fact]
    public async Task Create_WithNonExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = new MusicTrackDetailModel()
        {
            Id = Guid.NewGuid(),
            Title = "Titulok",
            Description = "Opis",
            Length = TimeSpan.FromSeconds(200),
            Size = 20,
            UrlAddress = "https://www.google.com",
            Artists = new ObservableCollection<ArtistListModel>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ArtistName = "Meno"
                }
            },
        };
        
        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(detailModel));
    }
    
    [Fact]
    public async Task GetAll_Single_SeededMusicTrack_Test()
    {
        var musictracks = await _facadeSUT.GetAsync();
        var musictrack = musictracks.Single(i => i.Id == MusicTrackSeeds.NonEmptyMusicTrack1.Id);

        DeepAssert.Equal(_musicTrackModelMapper.MapToListModel(MusicTrackSeeds.NonEmptyMusicTrack1), musictrack);
    }

    [Fact]
    public async Task GetById_SeededMusicTrack_Test()
    {
        var musictrack = await _facadeSUT.GetAsync(MusicTrackSeeds.NonEmptyMusicTrack1.Id);
        DeepAssert.Equal(_musicTrackModelMapper.MapToDetailModel(MusicTrackSeeds.NonEmptyMusicTrack1), musictrack);
    }

    [Fact]
    public async Task GetById_NonExistent_Test()
    {
        var musictrack = await _facadeSUT.GetAsync(MusicTrackSeeds.EmptyMusicTrack.Id);
        Assert.Null(musictrack);
    }

    [Fact]
    public async Task SeededNonEmptyMusicTrack1_DeleteById_Deleted()
    {
        await _facadeSUT.DeleteAsync(MusicTrackSeeds.NonEmptyMusicTrack1.Id);

        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        Assert.False(await dbxAssert.MusicTracks.AnyAsync(i => i.Id == MusicTrackSeeds.NonEmptyMusicTrack1.Id));
    }
    
    [Fact]
    public async Task NewMusicTrack_InsertOrUpdate_MusicTrackAdded()
    {
        //Arrange
        
        var musicTrack = new MusicTrackDetailModel()
        {
            Id = Guid.Empty,
            Title = "Titulok",
            Description = "Opis",
            Length = TimeSpan.FromSeconds(200),
            Size = 20,
            UrlAddress = "https://www.google.com",
        };

        //Act
        musicTrack = await _facadeSUT.SaveAsync(musicTrack);

        //Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        var musicTrackFromDb = await dbxAssert.MusicTracks.SingleAsync(i => i.Id == musicTrack.Id);
        DeepAssert.Equal(musicTrack, _musicTrackModelMapper.MapToDetailModel(musicTrackFromDb));
    }

    [Fact]
    public async Task SeededWater_InsertOrUpdate_MusicTrackUpdated()
    {
        //Arrange
        var musicTrack = new MusicTrackDetailModel()
        {
            Id = MusicTrackSeeds.NonEmptyMusicTrack1.Id,
            Title = MusicTrackSeeds.NonEmptyMusicTrack1.Title,
            Description = MusicTrackSeeds.NonEmptyMusicTrack1.Description,
            Length = MusicTrackSeeds.NonEmptyMusicTrack1.Length,
            Size = MusicTrackSeeds.NonEmptyMusicTrack1.Size,
            UrlAddress = MusicTrackSeeds.NonEmptyMusicTrack1.UrlAddress,
        };
        musicTrack.Title += "updated";
        musicTrack.Description += "updated";
        musicTrack.Length = TimeSpan.FromMinutes(5);
        musicTrack.Size = 5;
        musicTrack.UrlAddress += "updated";

        //Act
        await _facadeSUT.SaveAsync(musicTrack);

        //Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        var musicTrackFromDb = await dbxAssert.MusicTracks.SingleAsync(i => i.Id == musicTrack.Id);
        DeepAssert.Equal(musicTrack, _musicTrackModelMapper.MapToDetailModel(musicTrackFromDb));
    }
}