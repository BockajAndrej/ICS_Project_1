using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace ICS_Project.DAL.Tests;

public class DbContextGenreTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task Create_And_Check_Genre()
    {
        // Arrange
        Genre genre = new()
        {
            Id = Guid.Parse("6936A8F1-6CAC-4674-A7DC-DF78D8395BB4"),
            GenreName = "Phonk"
        };

        // Act
        MusicDbContextSUT.Genres.Add(genre);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualGenre = await dbx.Genres.SingleAsync(x => x.Id == genre.Id);
        DeepAssert.Equal(genre, actualGenre);
    }

    [Fact]
    public async Task Create_And_Delete_Genre()
    {
        // Arrange
        Genre genre = new()
        {
            Id = Guid.Parse("0CEAB58D-B9E3-4F68-B6A2-F73372C06E88"),
            GenreName = "House"
        };
        MusicDbContextSUT.Genres.Add(genre);
        await MusicDbContextSUT.SaveChangesAsync();

        // Act
        MusicDbContextSUT.Genres.Remove(genre);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var deleteGenre = await dbx.Genres.SingleOrDefaultAsync(a => a.Id == genre.Id);
        DeepAssert.Equal(null, deleteGenre);
    }

    [Fact]
    public async Task Find_NonExist_Genre()
    {
        // Arrange
        Genre genre = new()
        {
            Id = Guid.Parse("98EFC38B-26D1-4919-94C4-4F10BFFB7626"),
            GenreName = "Electro"
        };

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var nonExistGenre = await dbx.Genres.SingleOrDefaultAsync(a => a.Id == genre.Id);
        DeepAssert.Equal(null, nonExistGenre);
    }

    [Fact]
    public async Task Create_Genre_Check_Empty_MusicTrack()
    {
        // Arrange
        Genre genre = new()
        {
            Id = Guid.Parse("513870FD-5195-44DC-9E38-0C3A1A956F71"),
            GenreName = "Rap"
        };

        // Act
        MusicDbContextSUT.Genres.Add(genre);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualGenre = await dbx.Genres
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == genre.Id);
        Assert.NotNull(actualGenre);
        Assert.Empty(actualGenre.MusicTracks);
    }

    [Fact]
    public async Task Create_Genre_Add_MusicTrack()
    {
        // Arrange
        Genre genre = new()
        {
            Id = Guid.Parse("9BD9BF4D-B819-4349-A1F0-8B477A1CD954"),
            GenreName = "Bass"
        };
        MusicTrack track = new()
        {
            Id = Guid.Parse("E4F86283-7B30-41C0-A7E4-4EA4BEB03AE9"),
            Title = "Test Track",
            Description = "Test Description",
            Length = TimeSpan.FromMinutes(3),
            Size = 3,
            UrlAddress = "http://example.com"
        };

        // Act
        MusicDbContextSUT.Genres.Add(genre);
        MusicDbContextSUT.MusicTracks.Add(track);

        // Relationship
        genre.MusicTracks.Add(track);
        track.Genres.Add(genre);

        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualGenre = await dbx.Genres
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == genre.Id);

        Assert.NotNull(actualGenre);
        Assert.NotEmpty(actualGenre.MusicTracks);
        Assert.Contains(actualGenre.MusicTracks, t => t.Id == track.Id);
    }

    [Fact]
    public async Task Update_Genre_Name()
    {
        // Arrange
        Genre genre = new()
        {
            Id = Guid.Parse("0B0260D6-EDBD-4395-B6B8-FA9B129F6727"),
            GenreName = "Original Name"
        };

        MusicDbContextSUT.Genres.Add(genre);
        await MusicDbContextSUT.SaveChangesAsync();

        // Act
        genre.GenreName = "Updated Name";
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var updatedGenre = await dbx.Genres.FindAsync(genre.Id);
        Assert.NotNull(updatedGenre);
        Assert.Equal("Updated Name", updatedGenre.GenreName);
    }

    [Fact]
    public async Task Remove_MusicTrack_From_Genre()
    {
        // Arrange
        Genre genre = new()
        {
            Id = Guid.Parse("353210BF-058C-49FA-AAC6-71292707ED0D"),
            GenreName = "Test Genre"
        };

        MusicTrack track = new()
        {
            Id = Guid.Parse("E39A005F-470C-4825-92E1-8F5E9AFFE503"),
            Title = "Test Genre",
            Description = "Test Description",
            Length = TimeSpan.FromMinutes(3),
            Size = 3,
            UrlAddress = "http://example.com"
        };

        MusicDbContextSUT.Genres.Add(genre);
        MusicDbContextSUT.MusicTracks.Add(track);
        // Set up relationship
        genre.MusicTracks.Add(track);
        track.Genres.Add(genre);
        await MusicDbContextSUT.SaveChangesAsync();

        // Act: Remove the track from the artist, artist from track
        genre.MusicTracks.Remove(track);
        track.Genres.Remove(genre);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualGenre = await dbx.Genres
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == genre.Id);

        Assert.NotNull(actualGenre);
        Assert.Empty(actualGenre.MusicTracks);
    }

    // Seeded tests

    [Fact]
    public async Task Get_Genre_By_Id()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var retGenre = await dbx.Genres.FindAsync(GenreSeeds.NonEmptyGenre.Id);

        // Assert
        Assert.NotNull(retGenre);
        Assert.Equal(GenreSeeds.NonEmptyGenre.GenreName, retGenre.GenreName);
    }

    [Fact]
    public async Task Seeded_Genre_Has_Correct_MusicTracks()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var genre = await dbx.Genres
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == GenreSeeds.NonEmptyGenre.Id);

        // Assert
        Assert.NotNull(genre);
        Assert.Equal(2, genre.MusicTracks.Count);
        Assert.Contains(genre.MusicTracks, t => t.Id == MusicTrackSeeds.NonEmptyMusicTrack1.Id);
        Assert.Contains(genre.MusicTracks, t => t.Id == MusicTrackSeeds.NonEmptyMusicTrack2.Id);
    }

    [Fact]
    public async Task Seeded_Genre_Has_Zero_MusicTracks()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var genre = await dbx.Genres
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == GenreSeeds.GenreEmptyMusicTracks.Id);

        // Assert
        Assert.NotNull(genre);
        Assert.Empty(genre.MusicTracks);
    }

    [Fact]
    public async Task Update_Seeded_Genre_Name()
    {
        // Retrieve 
        var genre = await MusicDbContextSUT.Genres.FindAsync(GenreSeeds.GenreUpdate.Id);
        Assert.NotNull(genre);

        // Act
        genre.GenreName = "Updated Genre Name";
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var updatedGenre = await dbx.Genres.FindAsync(GenreSeeds.GenreUpdate.Id);
        Assert.NotNull(updatedGenre);
        Assert.Equal("Updated Genre Name", updatedGenre.GenreName);
    }

    [Fact]
    public async Task Get_Seeded_EmptyGenreName()
    {
        // Arrange & Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var genre = await dbx.Genres.FindAsync(GenreSeeds.EmptyGenreName.Id);

        // Assert
        Assert.NotNull(genre);
        DeepAssert.Equal(string.Empty, genre.GenreName);
    }

    [Fact]
    public async Task Delete_Seeded_Genre()
    {
        // Retrieve 
        var genre = await MusicDbContextSUT.Genres.FindAsync(GenreSeeds.GenreDelete.Id);
        Assert.NotNull(genre);

        // Act
        MusicDbContextSUT.Genres.Remove(genre);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var deletedGenre = await dbx.Genres.FindAsync(GenreSeeds.GenreDelete.Id);
        Assert.Null(deletedGenre);
    }
}