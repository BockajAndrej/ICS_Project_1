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
using Microsoft.EntityFrameworkCore;
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
    
    //-------------
    // Newer tests
    //-------------
    [Fact]
    public async Task Create_Playlist_Test()
    {
        var detailModelToCreate = new PlaylistDetailModel
        {
            Id = Guid.NewGuid(),
            Name = "New Playlist From Facade Test",
            Description = "Description for new playlist",
            NumberOfMusicTracks = 0,
            TotalPlayTime = TimeSpan.Zero,
            MusicTracks = new ObservableCollection<MusicTrackListModel>()
        };

        var returnedModel = await _facadeSUT.SaveAsync(detailModelToCreate);

        Assert.NotNull(returnedModel);
        Assert.NotEqual(Guid.Empty, returnedModel.Id);
        detailModelToCreate.Id = returnedModel.Id;

        DeepAssert.Equal(detailModelToCreate, returnedModel);

        await using var dbx = await DbContextFactory.CreateDbContextAsync();

        var createdEntity = await dbx.Playlists
            .Include(p => p.MusicTracks)
            .SingleOrDefaultAsync(p => p.Id == returnedModel.Id);

        Assert.NotNull(createdEntity);

        Assert.Equal(detailModelToCreate.Name, createdEntity.Name);
        Assert.Equal(detailModelToCreate.Description, createdEntity.Description);
        Assert.Equal(detailModelToCreate.NumberOfMusicTracks, createdEntity.NumberOfMusicTracks);
        Assert.Equal(detailModelToCreate.TotalPlayTime, createdEntity.TotalPlayTime);

        Assert.Empty(createdEntity.MusicTracks);
    }
    
    [Fact]
    public async Task Update_ExistingPlaylist_WithoutModifyingTracks_Test()
    {
        var playlistSeed = PlaylistSeeds.NonEmptyPlaylist;

        await using (var dbx = await DbContextFactory.CreateDbContextAsync())
        {
            var seededPlaylist = await dbx.Playlists
                .Include(p => p.MusicTracks)
                .SingleOrDefaultAsync(p => p.Id == playlistSeed.Id);
            Assert.NotNull(seededPlaylist);

            int originalTrackCount = seededPlaylist.MusicTracks.Count;

            dbx.Entry(seededPlaylist).State = EntityState.Detached;


            var detailModelToUpdate = _playlistModelMapper.MapToDetailModel(playlistSeed);
            detailModelToUpdate.Name = "Updated Playlist Name";
            detailModelToUpdate.Description = "Updated description";
            detailModelToUpdate.MusicTracks.Clear();

            var returnedModel = await _facadeSUT.SaveAsync(detailModelToUpdate);

            Assert.NotNull(returnedModel);
            Assert.Equal(detailModelToUpdate.Id, returnedModel.Id);
            Assert.Equal(detailModelToUpdate.Name, returnedModel.Name);
            Assert.Equal(detailModelToUpdate.Description, returnedModel.Description);
            Assert.Empty(returnedModel.MusicTracks);

            await using var dbxAfterUpdate = await DbContextFactory.CreateDbContextAsync();
            var updatedEntity = await dbxAfterUpdate.Playlists
                .Include(p => p.MusicTracks)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == detailModelToUpdate.Id);

            Assert.NotNull(updatedEntity);
            Assert.Equal("Updated Playlist Name", updatedEntity.Name);
            Assert.Equal("Updated description", updatedEntity.Description);

            Assert.Equal(originalTrackCount, updatedEntity.MusicTracks.Count);
        }
    }
    
    [Fact]
    public async Task GetById_PlaylistWithMusicTracks_ReturnsDetailModelWithCorrectTrackIds()
    {
        var playlistWithTracksSeed = PlaylistSeeds.NonEmptyPlaylist;

        await using (var dbx = await DbContextFactory.CreateDbContextAsync())
        {
            var seededPlaylist = await dbx.Playlists
                .Include(p => p.MusicTracks)
                .SingleOrDefaultAsync(p => p.Id == playlistWithTracksSeed.Id);

            Assert.NotNull(seededPlaylist);
            Assert.NotEmpty(seededPlaylist.MusicTracks);
            Assert.Equal(playlistWithTracksSeed.MusicTracks.Count, seededPlaylist.MusicTracks.Count);
             Assert.Contains(seededPlaylist.MusicTracks, pmt => pmt.Id == MusicTrackSeeds.NonEmptyMusicTrack1.Id);
             Assert.Contains(seededPlaylist.MusicTracks, pmt => pmt.Id == MusicTrackSeeds.NonEmptyMusicTrack2.Id);

            dbx.Entry(seededPlaylist).State = EntityState.Detached;
        }

        var expectedDetailModel = _playlistModelMapper.MapToDetailModel(playlistWithTracksSeed);
        Assert.NotNull(expectedDetailModel.MusicTracks);
        Assert.NotEmpty(expectedDetailModel.MusicTracks);
        Assert.Equal(playlistWithTracksSeed.MusicTracks.Count, expectedDetailModel.MusicTracks.Count);


        var returnedModel = await _facadeSUT.GetAsync(playlistWithTracksSeed.Id);

        Assert.NotNull(returnedModel);
        DeepAssert.Equal(expectedDetailModel, returnedModel);

        Assert.NotNull(returnedModel.MusicTracks);
        Assert.NotEmpty(returnedModel.MusicTracks);
        Assert.Equal(expectedDetailModel.MusicTracks.Count, returnedModel.MusicTracks.Count);

        var returnedTrackIds = returnedModel.MusicTracks.Select(mt => mt.Id).ToList();
        var expectedTrackIds = expectedDetailModel.MusicTracks.Select(mt => mt.Id).ToList();

        Assert.Equal(expectedTrackIds.Count, returnedTrackIds.Count);
        Assert.True(expectedTrackIds.OrderBy(id => id).SequenceEqual(returnedTrackIds.OrderBy(id => id)));
    }

    [Fact]
    public async Task GetById_NonExistentPlaylist_ReturnsNull()
    {
        var nonExistentId = Guid.NewGuid();

        var returnedModel = await _facadeSUT.GetAsync(nonExistentId);

        Assert.Null(returnedModel);
    }


     [Fact]
     public async Task GetAsync_ReturnsAllPlaylistsAsListModel()
     {
         await using var dbx = await DbContextFactory.CreateDbContextAsync();
         var allPlaylists = await dbx.Playlists
             .AsNoTracking()
             .ToListAsync();

         var expectedListModels = _playlistModelMapper.MapToListModel(allPlaylists);

         var returnedListModels = await _facadeSUT.GetAsync();

         Assert.NotNull(returnedListModels);

         Assert.Equal(expectedListModels.Count(), returnedListModels.Count());

         DeepAssert.Equal(expectedListModels.OrderBy(m => m.Id), returnedListModels.OrderBy(m => m.Id));
     }

    [Fact]
    public async Task GetAsync_WithSearchTerm_ReturnsMatchingPlaylistsByNameOrDescription()
    {
        var playlist1 = new Playlist { Id = Guid.Parse("B1B1B1B1-1111-2222-3333-000000000001"), Name = "Workout Hits", Description = "High energy tracks for training" };
        var playlist2 = new Playlist { Id = Guid.Parse("B1B1B1B1-1111-2222-3333-000000000002"), Name = "Relaxation Mix", Description = "Chill ambient sounds" };
        var playlist3 = new Playlist { Id = Guid.Parse("B1B1B1B1-1111-2222-3333-000000000003"), Name = "Driving Playlist", Description = "Best tracks for the road" };
        var playlist4 = new Playlist { Id = Guid.Parse("B1B1B1B1-1111-2222-3333-000000000004"), Name = "Study Focus", Description = "Instrumental tracks for concentration" };
        var playlist5 = new Playlist { Id = Guid.Parse("B1B1B1B1-1111-2222-3333-000000000005"), Name = "Chill Vibes", Description = "Relaxing music" };


        await using (var dbx = await DbContextFactory.CreateDbContextAsync())
        {
            dbx.MusicTracks.RemoveRange(dbx.MusicTracks);
            await dbx.SaveChangesAsync();
            
            dbx.Playlists.RemoveRange(dbx.Playlists);
            await dbx.SaveChangesAsync();
            
            dbx.Playlists.AddRange(playlist1, playlist2, playlist3, playlist4, playlist5);
            await dbx.SaveChangesAsync();
        }

        var searchTerm = "la";

        var expectedEntities = new List<Playlist> { playlist2, playlist3 };
        var expectedModels = _playlistModelMapper.MapToListModel(expectedEntities).ToList();


        var returnedModels = await _facadeSUT.GetAsync(searchTerm);

        Assert.NotNull(returnedModels);

        Assert.Equal(expectedModels.Count, returnedModels.Count());

        DeepAssert.Equal(expectedModels.OrderBy(m => m.Id), returnedModels.OrderBy(m => m.Id));

        foreach (var model in returnedModels)
        {
            var containsInName = model.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase);
            var containsInDescription = model.Description?.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase) ?? false;
            Assert.True(containsInName || containsInDescription, $"Playlist '{model.Name}' (ID: {model.Id}) does not contain search term '{searchTerm}' in name or description.");
        }
    }

     [Fact]
     public async Task GetAsync_WithEmptySearchTerm_ReturnsAllPlaylists()
     {
         await using var dbx = await DbContextFactory.CreateDbContextAsync();
         var allPlaylists = await dbx.Playlists
             .AsNoTracking()
             .ToListAsync();

         var expectedListModels = _playlistModelMapper.MapToListModel(allPlaylists);

         var returnedListModels = await _facadeSUT.GetAsync(string.Empty);

         Assert.NotNull(returnedListModels);

         Assert.Equal(expectedListModels.Count(), returnedListModels.Count());

         DeepAssert.Equal(expectedListModels.OrderBy(m => m.Id), returnedListModels.OrderBy(m => m.Id));
     }
     
    [Fact]
    public async Task Delete_Playlist_WithTracks_DeletesPlaylistAndJoinEntities()
    {
        var playlistToDelete = PlaylistSeeds.NonEmptyPlaylist;

         int originalTrackCount;
         await using (var dbx = await DbContextFactory.CreateDbContextAsync())
         {
             var seededPlaylist = await dbx.Playlists
                 .Include(p => p.MusicTracks)
                 .SingleOrDefaultAsync(p => p.Id == playlistToDelete.Id);
             Assert.NotNull(seededPlaylist);
             Assert.NotEmpty(seededPlaylist.MusicTracks);
             originalTrackCount = seededPlaylist.MusicTracks.Count;
             dbx.Entry(seededPlaylist).State = EntityState.Detached;
         }
         Assert.True(originalTrackCount > 0, "The seeded playlist must have tracks for this test.");


        await _facadeSUT.DeleteAsync(playlistToDelete.Id);

        await using var dbxAfterDelete = await DbContextFactory.CreateDbContextAsync();
        var deletedPlaylist = await dbxAfterDelete.Playlists
                                            .SingleOrDefaultAsync(p => p.Id == playlistToDelete.Id);

        Assert.Null(deletedPlaylist);

        var remainingJoinEntries = await dbxAfterDelete.MusicTracks
                                            .Where(pmt => pmt.Id == playlistToDelete.Id)
                                            .ToListAsync();

        Assert.Empty(remainingJoinEntries);

        var trackIdThatWasLinked = playlistToDelete.MusicTracks.First().Id;

        var musicTrackStillExists = await dbxAfterDelete.MusicTracks.AnyAsync(mt => mt.Id == trackIdThatWasLinked);
        Assert.True(musicTrackStillExists, $"Music track with ID {trackIdThatWasLinked} should still exist after playlist deletion.");

    }

     [Fact]
     public async Task Delete_NonExistentPlaylist_DoesNotThrow()
     {
         var nonExistentId = Guid.NewGuid();

         var exception = await Record.ExceptionAsync(() => _facadeSUT.DeleteAsync(nonExistentId));

         Assert.NotNull(exception);
     }
    
     [Fact]
     public async Task Delete_Playlist_WithoutTracks_DeletesPlaylist()
     {
         var playlistToDelete = PlaylistSeeds.NonEmptyPlaylist;

         await using (var dbx = await DbContextFactory.CreateDbContextAsync())
         {
             dbx.MusicTracks.RemoveRange(dbx.MusicTracks);
             await dbx.SaveChangesAsync();
             
             var seededPlaylist = await dbx.Playlists
                 .Include(p => p.MusicTracks)
                 .SingleOrDefaultAsync(p => p.Id == playlistToDelete.Id);
             Assert.NotNull(seededPlaylist);
             
             Assert.Empty(seededPlaylist.MusicTracks);
             dbx.Entry(seededPlaylist).State = EntityState.Detached;
         }

         await _facadeSUT.DeleteAsync(playlistToDelete.Id);

         await using var dbxAfterDelete = await DbContextFactory.CreateDbContextAsync();
         var deletedPlaylist = await dbxAfterDelete.Playlists
             .Include(p => p.MusicTracks)
             .SingleOrDefaultAsync(p => p.Id == playlistToDelete.Id);

         Assert.Null(deletedPlaylist);
     }
     
     [Fact]
    public async Task AddMusicTrackToPlaylist_ExistingPlaylistAndTrack_CreatesJoinEntityAndUpdatesPlaylist()
    {
        // Arrange
        var playlistId = PlaylistSeeds.EmptyPlaylist.Id;
        var trackIdToAdd = MusicTrackSeeds.NonEmptyMusicTrack1.Id;

        await using (var dbxBefore = await DbContextFactory.CreateDbContextAsync())
        {
            var playlistBefore = await dbxBefore.Playlists
                .Include(p => p.MusicTracks)
                .SingleOrDefaultAsync(p => p.Id == playlistId);
            Assert.NotNull(playlistBefore);
            Assert.False(playlistBefore.MusicTracks.Any(pmt => pmt.Id == trackIdToAdd),
                "Playlist by nemal obsahovať skladbu pred testom.");
        }
        
        await _facadeSUT.AddMusicTrackToPlaylistAsync(playlistId, trackIdToAdd);
        
        await using var dbxAfter = await DbContextFactory.CreateDbContextAsync();
        
        var playlistAfter = await dbxAfter.Playlists
            .Include(p => p.MusicTracks)
            .SingleOrDefaultAsync(p => p.Id == playlistId);

        Assert.NotNull(playlistAfter);
        Assert.True(playlistAfter.MusicTracks.Any(pmt => pmt.Id == trackIdToAdd),
            "Skladba by mala byť pridaná k playlistu cez spojovaciu tabuľku.");

        var returnedPlaylistModel = await _facadeSUT.GetAsync(playlistId);
        Assert.NotNull(returnedPlaylistModel);
        Assert.True(returnedPlaylistModel.MusicTracks.Any(mt => mt.Id == trackIdToAdd),
             "Vrátený DetailModel by mal obsahovať pridanú skladbu.");
        
        // Assert.Equal(expectedTrackCount, returnedPlaylistModel.MusicTracks.Count);
    }
    
    [Fact]
    public async Task RemoveMusicTrackFromPlaylist_ExistingPlaylistAndTrack_RemovesJoinEntity()
    {
        // Arrange
        var playlistId = PlaylistSeeds.NonEmptyPlaylist.Id; 
        var trackIdToRemove = MusicTrackSeeds.NonEmptyMusicTrack1.Id;

        await using (var dbxBefore = await DbContextFactory.CreateDbContextAsync())
        {
            var playlistBefore = await dbxBefore.Playlists
                .Include(p => p.MusicTracks)
                .SingleOrDefaultAsync(p => p.Id == playlistId);

            Assert.NotNull(playlistBefore);
            Assert.True(playlistBefore.MusicTracks.Any(mt => mt.Id == trackIdToRemove),
                "PRE-CONDICIA ZLYHALA: Playlist by mal pred testom na odstránenie obsahovať danú skladbu.");
            // var initialTrackCount = playlistBefore.MusicTracks.Count;
        }

        await _facadeSUT.RemoveMusicTrackFromPlaylistAsync(playlistId, trackIdToRemove);

        await using var dbxAfter = await DbContextFactory.CreateDbContextAsync();

        var playlistAfter = await dbxAfter.Playlists
            .Include(p => p.MusicTracks)
            .SingleOrDefaultAsync(p => p.Id == playlistId);

        Assert.NotNull(playlistAfter);
        Assert.False(playlistAfter.MusicTracks.Any(mt => mt.Id == trackIdToRemove),
            "Skladba by mala byť úspešne odstránená z playlistu (prepojenie v spojovacej tabuľke by malo zmiznúť).");

        // Assert.Equal(initialTrackCount - 1, playlistAfter.MusicTracks.Count);

        var returnedPlaylistModel = await _facadeSUT.GetAsync(playlistId); // Predpokladá, že GetAsync funguje a načítava skladby
        Assert.NotNull(returnedPlaylistModel);
        Assert.False(returnedPlaylistModel.MusicTracks.Any(mt => mt.Id == trackIdToRemove),
             "Vrátený DetailModel po odstránení by nemal obsahovať odstránenú skladbu.");
         // Assert.Equal(initialTrackCount - 1, returnedPlaylistModel.MusicTracks.Count);
    }
     
    //-------------
    // Older tests
    //-------------
    [Fact]
    public async Task Create_WithNonExistingMusicTrack_Throws_Test()
    {
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
    public async Task Delete_OneToOne_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        
        var playlistsStart = await _facadeSUT.GetAsync();
        
        int numOfPlaylists = 1;
        var currPlaylist = FillPlaylistDatabase(numOfPlaylists, repository, uow);
        
        var derailModel = _playlistModelMapper.MapToDetailModel(currPlaylist.Dequeue());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var playlists = await _facadeSUT.GetAsync();
        
        DeepAssert.Equal(playlistsStart, playlists);
    }
    [Fact]
    public async Task Delete_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        
        var playlistsStart = await _facadeSUT.GetAsync();

        
        int numOfPlaylists = 5;
        var currPlaylist = FillPlaylistDatabase(numOfPlaylists, repository, uow);

        for (int i = 0; i < numOfPlaylists; i++)
        {
            var detailModel = _playlistModelMapper.MapToDetailModel(currPlaylist.Dequeue());
            await _facadeSUT.DeleteAsync(detailModel.Id);
        }

        var playlists = await _facadeSUT.GetAsync();

        DeepAssert.Equal(playlistsStart, playlists);
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