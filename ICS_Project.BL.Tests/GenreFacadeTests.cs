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
using System.Collections.ObjectModel;

namespace ICS_Project.BL.Tests;

public class GenreFacadeTests : FacadeTestsBase
{
    private readonly IGenreFacade _facadeSUT;
    private readonly IGenreModelMapper _genreModelMapper;
    private readonly IMusicTrackModelMapper _musicTrackModelMapper;

    public GenreFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = ServiceProvider.GetRequiredService<IGenreFacade>();
        _genreModelMapper = ServiceProvider.GetRequiredService<IGenreModelMapper>();
        _musicTrackModelMapper = ServiceProvider.GetRequiredService<IMusicTrackModelMapper>();
    }
    
    
    [Fact]
    public async Task Create_WithNonExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = new GenreDetailModel()
        {
            Id = Guid.NewGuid(),
            GenreName = "powermetal",
            
            MusicTracks = new ObservableCollection<MusicTrackListModel>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = string.Empty,
                    Length = TimeSpan.FromMinutes(4),
                    Size = 0,
                    Title = "40:1",
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
        var detailModel = _genreModelMapper.MapToDetailModel(GenreSeeds.GenreEmptyMusicTracks);

        var returnedModel = await _facadeSUT.SaveAsync(detailModel);

        FixIds(detailModel, returnedModel); // What is this good for? <Vitan_, AKA xkolosv00>
        DeepAssert.Equal(detailModel, returnedModel);
    }
    
    [Fact]
    public async Task Create_WithExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = _genreModelMapper.MapToDetailModel(GenreSeeds.NonEmptyGenre);
        
        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(detailModel));
    }
    
    [Fact]
    public async Task Create_WithAndWoutExistingMusicTrack_Throws_Test()
    {
        //As is noted in FacadeBase.GuardCollectionsAreNotSet: Inserting or updating models with adjacent collections is baned
        
        //Arrange
        var detailModel = new GenreDetailModel()
        {
            Id = Guid.NewGuid(),
            GenreName = "Pop Rock",
            
            MusicTracks =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Description = string.Empty,
                    Length = TimeSpan.Zero,
                    Size = 0,
                    Title = "Fire",
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
        IRepository<Genre> repository = uow.GetRepository<Genre, GenreEntityMapper>();
        
        repository.Insert(GenreSeeds.NonEmptyGenre);
        uow.CommitAsync();
        
        var detailModel = _genreModelMapper.MapToDetailModel(GenreSeeds.NonEmptyGenre);

        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        DeepAssert.Equal(detailModel, returnedModel);
    }
    [Fact]
    public async Task GetID_OneToMany_Test()
    {
        //We need to populate the database without a facade SaveAsync method
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Genre> repository = uow.GetRepository<Genre, GenreEntityMapper>();
        
        int numOfArtists = 5;
        var lastGenre = FillArtistDatabase(numOfArtists, repository, uow);
        
        var detailModel = _genreModelMapper.MapToDetailModel(lastGenre.Dequeue());

        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task Get_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Genre> repository = uow.GetRepository<Genre, GenreEntityMapper>();
        
        int numOfGenres = 5;
        var firstGenre = FillArtistDatabase(numOfGenres, repository, uow);
        
        var GenreList = await _facadeSUT.GetAsync();
        foreach (var artist in GenreList)
        {
            numOfGenres--;
            var currentModel = _genreModelMapper.MapToListModel(firstGenre.Dequeue());
            DeepAssert.Equal(currentModel, artist);
        }
        //Check that number of inserted artist is correct 
        Assert.Equal(0, numOfGenres);
    }

    [Fact]
    public async Task Delete_OneToOne_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Genre> repository = uow.GetRepository<Genre, GenreEntityMapper>();
        
        int numOfGenres = 1;
        var currArtist = FillArtistDatabase(numOfGenres, repository, uow);
        
        var derailModel = _genreModelMapper.MapToDetailModel(currArtist.Dequeue());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var genres = await _facadeSUT.GetAsync();
        
        Assert.True(!genres.Any());
    }
    [Fact]
    public async Task Delete_ManyToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Genre> repository = uow.GetRepository<Genre, GenreEntityMapper>();
        
        int numOfGenres = 5;
        var currArtist = FillArtistDatabase(numOfGenres, repository, uow);

        for (int i = 0; i < numOfGenres; i++)
        {
            var detailModel = _genreModelMapper.MapToDetailModel(currArtist.Dequeue());
            await _facadeSUT.DeleteAsync(detailModel.Id);
        }

        var genres = await _facadeSUT.GetAsync();
        
        Assert.True(!genres.Any());
    }
    [Fact]
    public async Task Delete_OneToMany_Test()
    {
        IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<Genre> repository = uow.GetRepository<Genre, GenreEntityMapper>();
        
        int numOfGenres = 5;
        var lastGenre = FillArtistDatabase(numOfGenres, repository, uow);
        
        var derailModel = _genreModelMapper.MapToDetailModel(lastGenre.Peek());

        await _facadeSUT.DeleteAsync(derailModel.Id);
        
        var artist = await _facadeSUT.GetAsync(derailModel.Id);
        
        Assert.Null(artist);
    }
    
    
    private static Queue<Genre> FillArtistDatabase(int numOfGenres, IRepository<Genre> repository, IUnitOfWork uow)
    {
        Queue<Genre> genres = new();
        for (int i = 0; i < numOfGenres; i++)
        {
            var genreToSave = GenreSeeds.GenreClone($"58D0C03C-C539-4A16-96B1-9A95AAAAAAA{i}", $"Name= {i}{i}");
            genres.Enqueue(genreToSave);
            repository.Insert(genreToSave);
            uow.CommitAsync();
        }
        return genres;
    }
    
    //Check if is 
    private static void FixIds(GenreDetailModel expectedModel, GenreDetailModel returnedModel) //TODO: Delete this, as it has no usage? <Vitan_, AKA xkolosv00>
    {
        returnedModel.Id = expectedModel.Id;
        foreach (var musicTrackModel in returnedModel.MusicTracks)
        {
            var musicTrackDetailModel = expectedModel.MusicTracks.FirstOrDefault(i =>
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