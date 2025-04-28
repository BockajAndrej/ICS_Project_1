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

public class ArtistFacadeTests : FacadeTestsBase
{
    private readonly IArtistFacade _facadeSut;
    private readonly IArtistModelMapper _artistModelMapper;
    private readonly IMusicTrackModelMapper _musicTrackModelMapper;

    public ArtistFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSut = ServiceProvider.GetRequiredService<IArtistFacade>();
        _artistModelMapper = ServiceProvider.GetRequiredService<IArtistModelMapper>();
        _musicTrackModelMapper = ServiceProvider.GetRequiredService<IMusicTrackModelMapper>();
    }
    
    //-------------
    // Newer tests
    //-------------
    [Fact]
    public async Task Create_Artist_Test()
    {
        // Arrange
        var detailModelToCreate = new ArtistDetailModel()
        {
            Id = Guid.NewGuid(),
            ArtistName = "New Artist From Facade Test",
            // MusicTrack collection must be null or empty for correct SaveAsync
            MusicTrack = new ObservableCollection<MusicTrackListModel>()
        };

        var returnedModel = await _facadeSut.SaveAsync(detailModelToCreate);

        // Check result right from database with new DbContextu
        await using var dbx = await DbContextFactory.CreateDbContextAsync();

        var createdEntity = await dbx.Artists
            .Include(a => a.MusicTracks)
            .SingleOrDefaultAsync(a => a.Id == returnedModel.Id);
        
        // Exist?
        Assert.NotNull(createdEntity);

        // Mapping
        var actualDetailModel = _artistModelMapper.MapToDetailModel(createdEntity);

        detailModelToCreate.Id = returnedModel.Id;
        
        Assert.Empty(actualDetailModel.MusicTrack);
        //Check properties
        DeepAssert.Equal(detailModelToCreate, actualDetailModel); 
    }
    
    [Fact]
    public async Task Update_Artist_Test()
    {
        // Arrange
        var artistId = Guid.Parse("1A2B3C4D-5E6F-7890-ABCD-EF1234567890");
        var originalArtist = new Artist()
        {
            Id = artistId,
            ArtistName = "Original Artist Name",
        };
        await using (var dbx = await DbContextFactory.CreateDbContextAsync())
        {
            dbx.Artists.Add(originalArtist);
            await dbx.SaveChangesAsync();
        }

        var detailModelToUpdate = new ArtistDetailModel
        {
            Id = artistId,
            ArtistName = "Updated Artist Name",
            // Init can not be there
            MusicTrack = new ObservableCollection<MusicTrackListModel>() 
        };

        // Act
        var returnedModel = await _facadeSut.SaveAsync(detailModelToUpdate);

        // Assert
        await using var dbxAfterUpdate = await DbContextFactory.CreateDbContextAsync();
        var updatedEntity = await dbxAfterUpdate.Artists
                                        .Include(a => a.MusicTracks)
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync(a => a.Id == detailModelToUpdate.Id);

        Assert.NotNull(updatedEntity);
        Assert.Equal("Updated Artist Name", updatedEntity.ArtistName);
        Assert.Empty(updatedEntity.MusicTracks);

        var actualDetailModel = _artistModelMapper.MapToDetailModel(updatedEntity);

        Assert.Equal(detailModelToUpdate.Id, actualDetailModel.Id);
        Assert.Equal(detailModelToUpdate.ArtistName, actualDetailModel.ArtistName);
        Assert.Empty(actualDetailModel.MusicTrack);
    }
    
    [Fact]
    public async Task Save_Artist_With_No_Tracks_Creates_Or_Updates_Correctly_Test()
    {
        // Arrange
        var artistWithoutTracksSeed = ArtistSeeds.ArtistWOutTracks;

        await using (var dbx = await DbContextFactory.CreateDbContextAsync())
        {
             var seededArtist = await dbx.Artists
                .Include(a => a.MusicTracks)
                .SingleOrDefaultAsync(a => a.Id == artistWithoutTracksSeed.Id);
             Assert.NotNull(seededArtist);
             Assert.Empty(seededArtist.MusicTracks);
        }

        // MusicTrack must be empty or null because of GuardCollectionsAreNotSet
        var detailModelToSave = new ArtistDetailModel
        {
            Id = artistWithoutTracksSeed.Id,
            ArtistName = "Updated Artist Name (No Tracks)",
            MusicTrack = new ObservableCollection<MusicTrackListModel>()
        };

        // Act
        var returnedModel = await _facadeSut.SaveAsync(detailModelToSave);

        // Assert
        // Check right from new DB
        await using var dbxAfterSave = await DbContextFactory.CreateDbContextAsync();
        var updatedEntity = await dbxAfterSave.Artists
                                    .Include(a => a.MusicTracks)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(a => a.Id == detailModelToSave.Id);

        Assert.NotNull(updatedEntity);
        Assert.Equal("Updated Artist Name (No Tracks)", updatedEntity.ArtistName);
        Assert.Empty(updatedEntity.MusicTracks); // Kolekcia skladieb by mala zostať prázdna

        // Expected model
        var expectedDetailModel = _artistModelMapper.MapToDetailModel(updatedEntity);

        DeepAssert.Equal(expectedDetailModel, returnedModel);
    }
    
    [Fact]
    public async Task GetID_OneToOne_Test_Refactored()
    {
        // Arrange
        var artistWOutTracksSeed = ArtistSeeds.ArtistWOutTracks;

        await using (var dbx = await DbContextFactory.CreateDbContextAsync())
        {
            var seededArtist = await dbx.Artists
                .Include(a => a.MusicTracks)
                .SingleOrDefaultAsync(a => a.Id == artistWOutTracksSeed.Id);
            Assert.NotNull(seededArtist);
            Assert.Empty(seededArtist.MusicTracks);
        }

        // Act
        var returnedModel = await _facadeSut.GetAsync(artistWOutTracksSeed.Id);

        // Assert
        var expectedDetailModel = _artistModelMapper.MapToDetailModel(artistWOutTracksSeed);

        Assert.NotNull(returnedModel);
        DeepAssert.Equal(expectedDetailModel, returnedModel);
        Assert.Empty(returnedModel.MusicTrack);
    }
    
    [Fact]
    public async Task GetID_ArtistWithMusicTracks_ReturnsDetailModelWithCorrectTrackIds_Test()
    {
        // Arrange
        var artistWithTracksSeed = ArtistSeeds.Artist;

        await using (var dbx = await DbContextFactory.CreateDbContextAsync())
        {
             var seededArtist = await dbx.Artists
                .Include(a => a.MusicTracks)
                .SingleOrDefaultAsync(a => a.Id == artistWithTracksSeed.Id);

             Assert.NotNull(seededArtist);
             Assert.NotEmpty(seededArtist.MusicTracks);
             Assert.Equal(artistWithTracksSeed.MusicTracks.Count, seededArtist.MusicTracks.Count);
             // Check if loaded right musicTracks
             Assert.Contains(seededArtist.MusicTracks, t => t.Id == MusicTrackSeeds.NonEmptyMusicTrack1.Id);
             Assert.Contains(seededArtist.MusicTracks, t => t.Id == MusicTrackSeeds.NonEmptyMusicTrack2.Id);
        }

        // Act
        var returnedModel = await _facadeSut.GetAsync(artistWithTracksSeed.Id);

        var expectedDetailModel = _artistModelMapper.MapToDetailModel(artistWithTracksSeed);
        
        Assert.NotNull(returnedModel);
        DeepAssert.Equal(expectedDetailModel, returnedModel);
        
        Assert.NotNull(returnedModel.MusicTrack);
        Assert.NotEmpty(returnedModel.MusicTrack);
        Assert.Equal(expectedDetailModel.MusicTrack.Count, returnedModel.MusicTrack.Count);

        // Check returned collection  
        var returnedTrackIds = returnedModel.MusicTrack.Select(mt => mt.Id).ToList();
        var expectedTrackIds = expectedDetailModel.MusicTrack.Select(mt => mt.Id).ToList();

        Assert.Equal(expectedTrackIds.Count, returnedTrackIds.Count);
        Assert.True(expectedTrackIds.OrderBy(id => id).SequenceEqual(returnedTrackIds.OrderBy(id => id)));
    }
    
    [Fact]
    public async Task GetAsync_WithSearchTerm_ReturnsMatchingArtistsByName()
    {
        // Arrange
        var artist1 = new Artist { Id = Guid.Parse("A1A1A1A1-1111-2222-3333-000000000001"), ArtistName = "Andrej Kiska" };
        var artist2 = new Artist { Id = Guid.Parse("A1A1A1A1-1111-2222-3333-000000000002"), ArtistName = "Peter Sagan" };
        var artist3 = new Artist { Id = Guid.Parse("A1A1A1A1-1111-2222-3333-000000000003"), ArtistName = "Andrej Babiš" };
        var artist4 = new Artist { Id = Guid.Parse("A1A1A1A1-1111-2222-3333-000000000004"), ArtistName = "Anorak Band" };
        var artist5 = new Artist { Id = Guid.Parse("A1A1A1A1-1111-2222-3333-000000000005"), ArtistName = "Zuzana Čaputová" };

        await using (var dbx = await DbContextFactory.CreateDbContextAsync())
        {
            dbx.Artists.AddRange(artist1, artist2, artist3, artist4, artist5);
            await dbx.SaveChangesAsync();
        }

        var searchTerm = "And";
        
        var expectedEntities = new List<Artist> { artist1, artist3, artist4 };
        var expectedModels = _artistModelMapper.MapToListModel(expectedEntities);

        // Act
        var returnedModels = await _facadeSut.GetAsync(searchTerm);

        // Assert
        Assert.NotNull(returnedModels);

        Assert.Equal(expectedModels.Count(), returnedModels.Count());

        // List have to be sorted 
        DeepAssert.Equal(expectedModels.OrderBy(m => m.Id), returnedModels.OrderBy(m => m.Id));

        foreach (var model in returnedModels)
        {
            Assert.Contains(searchTerm, model.ArtistName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
    
    //-------------
    // Older tests
    //-------------
    [Fact]
    public async Task Create_WithNonExistingMusicTrack_Throws_Test()
    {
        var detailModel = new ArtistDetailModel()
        {
            Id = Guid.NewGuid(), // Facade.SaveAsync by mal generovať nové ID, ak je predvolené prázdne,
            // ale pre test, kde model obsahuje kolekciu, je ID irelevantné, lebo GuardCollectionsAreNotSet vyhodí chybu hneď.
            ArtistName = "Brumbex",
            MusicTrack = new ObservableCollection<MusicTrackListModel>()
            {
                new() // Táto skladba neexistuje v DB (irelevantné pre test)
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

        // Prítomnost kolekcie v SaveAsync modeli
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSut.SaveAsync(detailModel));
    }
    
    [Fact]
    public async Task Create_WithExistingMusicTrack_Throws_Test()
    {
        //Arrange
        var detailModel = _artistModelMapper.MapToDetailModel(ArtistSeeds.Artist);
        
        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSut.SaveAsync(detailModel));
    }
    
    [Fact]
    public async Task Create_WithAndWoutExistingMusicTrack_Throws_Test()
    {
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
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSut.SaveAsync(detailModel));
    }
    
    
    [Fact]
    public async Task Save_Test()
    {
        var detailModel = _artistModelMapper.MapToDetailModel(ArtistSeeds.ArtistWOutTracks);

        var returnedModel = await _facadeSut.SaveAsync(detailModel);
        
        detailModel.Id = returnedModel.Id;
        DeepAssert.Equal(detailModel, returnedModel);
    }
    
    [Fact]
    public async Task GetID_OneToOne_Test()
    {
        var detailModel = _artistModelMapper.MapToDetailModel(ArtistSeeds.Artist);

        var returnedModel = await _facadeSut.GetAsync(detailModel.Id);

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

        var returnedModel = await _facadeSut.GetAsync(detailModel.Id);

        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task Delete_OneToOne_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        var artistsStart = await _facadeSut.GetAsync();
        
        int numOfArtists = 1;
        var currArtist = FillArtistDatabase(numOfArtists, repository, uow);
        
        var derailModel = _artistModelMapper.MapToDetailModel(currArtist.Dequeue());

        await _facadeSut.DeleteAsync(derailModel.Id);
        
        var artists = await _facadeSut.GetAsync();
        
        DeepAssert.Equal(artistsStart, artists);
    }
    [Fact]
    public async Task Delete_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Artist> repository = uow.GetRepository<Artist, ArtistEntityMapper>();
        
        var artistsStart = await _facadeSut.GetAsync();
        
        int numOfArtists = 5;
        var currArtist = FillArtistDatabase(numOfArtists, repository, uow);

        for (int i = 0; i < numOfArtists; i++)
        {
            var detailModel = _artistModelMapper.MapToDetailModel(currArtist.Dequeue());
            await _facadeSut.DeleteAsync(detailModel.Id);
        }

        var artists = await _facadeSut.GetAsync();
        
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

        await _facadeSut.DeleteAsync(derailModel.Id);
        
        var artist = await _facadeSut.GetAsync(derailModel.Id);
        
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
}