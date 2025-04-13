using System.Collections.ObjectModel;
using ICS_Project.BL.Facades;
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

public class PlaylistFacadeTests : FacadeTestsBase
{
    private readonly IPlaylistFacade _facadeSUT;
    private readonly IPlaylistModelMapper _playlistModelMapper;
    private readonly IMusicTrackModelMapper _musicTrackModelMapper;

    public PlaylistFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = ServiceProvider.GetRequiredService<IPlaylistFacade>();
        _playlistModelMapper = ServiceProvider.GetRequiredService<IPlaylistModelMapper>();
        _musicTrackModelMapper = ServiceProvider.GetRequiredService<IMusicTrackModelMapper>();
    }
    
    [Fact]
    public async Task Create_WithNonExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = new PlaylistDetailModel()
        {
            Id = Guid.NewGuid(),
            Name = "Sabaton Ultimate",
            Description = "For the grace, for the might of our lord",
            NumberOfMusicTracks = 1,
            TotalPlayTime = TimeSpan.FromHours(1),
            
            MusicTracks = new ObservableCollection<MusicTrackListModel>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = string.Empty,
                    Length = TimeSpan.Zero,
                    Size = 0,
                    Title = "The Last Stand",
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
        var detailModel = _playlistModelMapper.MapToDetailModel(PlaylistSeeds.PlaylistUpdate);

        var returnedModel = await _facadeSUT.SaveAsync(detailModel);

        FixIds(detailModel, returnedModel); // What is this good for? <Vitan_, AKA xkolosv00>
        DeepAssert.Equal(detailModel, returnedModel);
    }

    private static void FixIds(PlaylistDetailModel expectedModel, PlaylistDetailModel returnedModel) //TODO: Delete this, as it has no usage? <Vitan_, AKA xkolosv00>
    {
        returnedModel.Id = expectedModel.Id;
    }
    
    [Fact]
    public async Task Create_WithExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = _playlistModelMapper.MapToDetailModel(PlaylistSeeds.NonEmptyPlaylist);
        
        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(detailModel));
    }
    
    [Fact]
    public async Task Create_WithAndWoutExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = new PlaylistDetailModel()
        {
            Id = Guid.NewGuid(),
            Name = "Training",
            Description = "Just keep on the good work",
            NumberOfMusicTracks = 2,
            TotalPlayTime = TimeSpan.FromHours(2),
            
            MusicTracks = 
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = string.Empty,
                    Length = TimeSpan.Zero,
                    Size = 0,
                    Title = "Im so sorry",
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
        //We need to populate the database without a facade SaveAsync method
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        
        repository.Insert(PlaylistSeeds.NonEmptyPlaylist);
        uow.CommitAsync();
        
        var detailModel = _playlistModelMapper.MapToDetailModel(PlaylistSeeds.NonEmptyPlaylist);

        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        DeepAssert.Equal(detailModel, returnedModel);
    }
    
    [Fact]
    public async Task GetID_OneToMany_Test()
    {
        //We need to populate the database without a facade SaveAsync method
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        
        int numOfPlaylists = 5;
        var lastArtist = FillPlaylistDatabase(numOfPlaylists, repository, uow);
        
        var detailModel = _playlistModelMapper.MapToDetailModel(lastArtist.Dequeue());

        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task Get_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        
        int numOfPlaylists = 5;
        var FirstPlaylist = FillPlaylistDatabase(numOfPlaylists, repository, uow);
        
        var PlaylistList = await _facadeSUT.GetAsync();
        foreach (var Playlist in PlaylistList)
        {
            numOfPlaylists--;
            var currentModel = _playlistModelMapper.MapToListModel(FirstPlaylist.Dequeue());
            
            DeepAssert.Equal(currentModel, Playlist);
        }
        //Check that number of inserted artist is correct 
        Assert.Equal(0, numOfPlaylists);
    }

    [Fact]
    public async Task Delete_OneToOne_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        
        int numOfPlaylists = 1;
        var currPlaylist = FillPlaylistDatabase(numOfPlaylists, repository, uow);
        
        var derailModel = _playlistModelMapper.MapToDetailModel(currPlaylist.Dequeue());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var playlists = await _facadeSUT.GetAsync();
        
        Assert.True(!playlists.Any());
    }
    [Fact]
    public async Task Delete_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        
        int numOfPlaylists = 5;
        var currPlaylist = FillPlaylistDatabase(numOfPlaylists, repository, uow);

        for (int i = 0; i < numOfPlaylists; i++)
        {
            var detailModel = _playlistModelMapper.MapToDetailModel(currPlaylist.Dequeue());
            await _facadeSUT.DeleteAsync(detailModel.Id);
        }

        var playlists = await _facadeSUT.GetAsync();
        
        Assert.True(!playlists.Any());
    }
    [Fact]
    public async Task Delete_OneToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        
        int numOfPlaylists = 5;
        var lastPlaylist = FillPlaylistDatabase(numOfPlaylists, repository, uow);
        
        var derailModel = _playlistModelMapper.MapToDetailModel(lastPlaylist.Peek());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var playlist = await _facadeSUT.GetAsync(derailModel.Id);
        
        Assert.Null(playlist);
    }
    
    
    private static Queue<Playlist> FillPlaylistDatabase(int numOfPlaylists, IRepository<Playlist> repository, IUnitOfWork uow)
    {
        Queue<Playlist> playlists = new();
        for (int i = 0; i < numOfPlaylists; i++)
        {
            var playlistToSave = PlaylistSeeds.PlaylistClone($"64EA02A0-4986-4E19-BF0D-DC35F5E00AD{i}", $"Name= {i}{i}",
                $"Description= {i}");
            playlists.Enqueue(playlistToSave);
            repository.Insert(playlistToSave);
            uow.CommitAsync();
        }
        return playlists;
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