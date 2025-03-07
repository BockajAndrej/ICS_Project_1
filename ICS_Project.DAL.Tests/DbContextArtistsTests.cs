using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ICS_Project.DAL.Tests;

public class DbContextArtistsTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task Create_And_Check_Artist()
    {
        // Arrange
        Artist artist = new()
        {
            Id = Guid.Parse("AB717789-AB1F-48E5-905F-899B79E9AD88"),
            ArtistName = "Jack something"
        };

        // Act
        MusicDbContextSUT.Artists.Add(artist);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualArtist = await dbx.Artists.SingleAsync(x => x.Id == artist.Id);
        DeepAssert.Equal(artist, actualArtist);
    }

    [Fact]
    public async Task Create_And_Delete_Artist()
    {
        // Arrange
        Artist artist = new()
        {
            Id = Guid.Parse("342C8C73-624D-4FDB-901A-CCA848EDA7B2"),
            ArtistName = "Ruby"
        };
        MusicDbContextSUT.Artists.Add(artist);
        await MusicDbContextSUT.SaveChangesAsync();

        // Act
        MusicDbContextSUT.Artists.Remove(artist);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var deleteArtist = await dbx.Artists.SingleOrDefaultAsync(a => a.Id == artist.Id);
        DeepAssert.Equal(null, deleteArtist);
    }

    [Fact]
    public async Task Find_NonExist_Artist()
    {
        // Arrange
        Artist artist = new()
        {
            Id = Guid.Parse("073BFE86-CC93-40C5-9E68-6B22F67DD324"),
            ArtistName = "Perkeo"
        };

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var nonExistArtist = await dbx.Artists.SingleOrDefaultAsync(a => a.Id == artist.Id);
        DeepAssert.Equal(null, nonExistArtist);
    }

    [Fact]
    public async Task Create_Artist_Check_Empty_MusicTrack()
    {
        // Arrange
        Artist artist = new()
        {
            Id = Guid.Parse("6B189706-3EE5-442D-BA42-EEF840BF73AE"),
            ArtistName = "NoOne"
        };

        // Act
        MusicDbContextSUT.Artists.Add(artist);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualArtist = await dbx.Artists
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == artist.Id);
        Assert.NotNull(actualArtist);
        Assert.Empty(actualArtist.MusicTracks);
    }

    [Fact]
    public async Task Create_Artist_Add_MusicTrack()
    {
        // Arrange
        Artist artist = new()
        {
            Id = Guid.Parse("6B189706-3EE5-442D-BA42-EEF840BF73AE"),
            ArtistName = "NoOne"
        };
        MusicTrack track = new()
        {
            Id = Guid.Parse("604DC92F-09E7-4145-A016-76EE3F1658C1"),
            Title = "Test Track",
            Description = "Test Description",
            Length = TimeSpan.FromMinutes(3),
            Size = 3,
            UrlAddress = "http://example.com"
        };

        // Act
        MusicDbContextSUT.Artists.Add(artist);
        MusicDbContextSUT.MusicTracks.Add(track);

        // Relationship
        artist.MusicTracks.Add(track);
        track.Artists.Add(artist);

        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualArtist = await dbx.Artists
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == artist.Id);

        Assert.NotNull(actualArtist);
        Assert.NotEmpty(actualArtist.MusicTracks);
        Assert.Contains(actualArtist.MusicTracks, t => t.Id == track.Id);
    }

    [Fact]
    public async Task Update_Artist_Name()
    {
        // Arrange
        Artist artist = new()
        {
            Id = Guid.Parse("342C8C73-624D-4FDB-901A-CCA848EDA7B2"),
            ArtistName = "Original Name"
        };

        MusicDbContextSUT.Artists.Add(artist);
        await MusicDbContextSUT.SaveChangesAsync();

        // Act
        artist.ArtistName = "Updated Name";
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var updatedArtist = await dbx.Artists.FindAsync(artist.Id);
        Assert.NotNull(updatedArtist);
        Assert.Equal("Updated Name", updatedArtist.ArtistName);
    }

    [Fact]
    public async Task Remove_MusicTrack_From_Artist()
    {
        // Arrange
        Artist artist = new()
        {
            Id = Guid.Parse("B97D0C27-4156-40D5-970A-699D50887149"),
            ArtistName = "Test Artist"
        };

        MusicTrack track = new()
        {
            Id = Guid.Parse("F37A5597-0B10-4B47-8E9E-378141C3D05A"),
            Title = "Test Track",
            Description = "Test Description",
            Length = TimeSpan.FromMinutes(3),
            Size = 3,
            UrlAddress = "http://example.com"
        };

        MusicDbContextSUT.Artists.Add(artist);
        MusicDbContextSUT.MusicTracks.Add(track);
        // Set up relationship
        artist.MusicTracks.Add(track);
        track.Artists.Add(artist);
        await MusicDbContextSUT.SaveChangesAsync();

        // Act: Remove the track from the artist, artist from track
        artist.MusicTracks.Remove(track);
        track.Artists.Remove(artist);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualArtist = await dbx.Artists
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == artist.Id);

        Assert.NotNull(actualArtist);
        Assert.Empty(actualArtist.MusicTracks);
    }

    // Seeded Tests

    [Fact]
    public async Task Get_Artist_By_Id()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var retArtist = await MusicDbContextSUT.Artists.FindAsync(ArtistSeeds.Artist.Id);

        // Assert
        Assert.NotNull(retArtist);
        Assert.Equal(ArtistSeeds.Artist.ArtistName, retArtist.ArtistName);
    }

    [Fact]
    public async Task Seeded_Artist_Has_Correct_MusicTracks()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var artist = await dbx.Artists
            .Include(a => a.MusicTracks)
            .FirstOrDefaultAsync(a => a.Id == ArtistSeeds.Artist.Id);

        // Assert
        Assert.NotNull(artist);
        Assert.Equal(2, artist.MusicTracks.Count);
        Assert.Contains(artist.MusicTracks, t => t.Id == MusicTrackSeeds.NonEmptyMusicTrack1.Id);
        Assert.Contains(artist.MusicTracks, t => t.Id == MusicTrackSeeds.NonEmptyMusicTrack2.Id);
    }

    [Fact]
    public async Task Update_Seeded_Artist_Name()
    {
        // Retrieve 
        var artist = await MusicDbContextSUT.Artists.FindAsync(ArtistSeeds.ArtistUpdate.Id);
        Assert.NotNull(artist);

        // Act
        artist.ArtistName = "Updated Artist Name";
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var updatedArtist = await dbx.Artists.FindAsync(ArtistSeeds.ArtistUpdate.Id);
        Assert.NotNull(updatedArtist);
        Assert.Equal("Updated Artist Name", updatedArtist.ArtistName);
    }

    [Fact]
    public async Task Delete_Seeded_Artist()
    {
        // Retrieve 
        var artist = await MusicDbContextSUT.Artists.FindAsync(ArtistSeeds.ArtistDelete.Id);
        Assert.NotNull(artist);

        // Act
        MusicDbContextSUT.Artists.Remove(artist);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var deletedArtist = await dbx.Artists.FindAsync(ArtistSeeds.ArtistDelete.Id);
        Assert.Null(deletedArtist);
    }

    [Fact]
    public async Task Seeded_Artist_With_Empty_Name_Has_No_Name()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var artist = await dbx.Artists.FindAsync(ArtistSeeds.EmptyArtistName.Id);

        // Assert
        Assert.NotNull(artist);
        Assert.Equal(string.Empty, artist.ArtistName);
    }
}