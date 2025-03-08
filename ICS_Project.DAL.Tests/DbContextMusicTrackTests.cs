using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace ICS_Project.DAL.Tests;

[Collection("Sequential")]
public class DbContextMusicTrackTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    [Fact]
    public async Task Create_And_Check_MusicTrack()
    {
        // Arrange
        MusicTrack musicTrack = new()
        {
            Id = Guid.Parse("11102F24-7BE2-4B18-A103-BFD125659AD4"),
            Title = "Always",
            Description = "Yada yada",
            Length = TimeSpan.FromMinutes(3),
            Size = 4,
            UrlAddress = "https://music.youtube.com/watch?v=nH1Sley1PtU&si=OKFa0qFr7rQZwVPY"
        };

        // Act
        MusicDbContextSUT.MusicTracks.Add(musicTrack);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualMusicTrack = await dbx.MusicTracks.SingleAsync(x => x.Id == musicTrack.Id);
        DeepAssert.Equal(musicTrack, actualMusicTrack);
    }

    [Fact]
    public async Task Create_And_Delete_MusicTrack()
    {
        // Arrange
        MusicTrack musicTrack = new()
        {
            Id = Guid.Parse("58A9041E-292F-4339-826C-C0B52911B968"),
            Title = "I Smoked Away My Brain",
            Description = "Yada yada yada",
            Length = TimeSpan.FromMinutes(2),
            Size = 3,
            UrlAddress = "https://music.youtube.com/watch?v=Fv-NdQl5HrU&si=gsDmh97E1-geceHn"
        };
        MusicDbContextSUT.MusicTracks.Add(musicTrack);
        await MusicDbContextSUT.SaveChangesAsync();

        // Act
        MusicDbContextSUT.MusicTracks.Remove(musicTrack);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var deleteMusicTrack = await dbx.MusicTracks.SingleOrDefaultAsync(a => a.Id == musicTrack.Id);
        DeepAssert.Equal(null, deleteMusicTrack);
    }

    [Fact]
    public async Task Find_NonExist_MusicTrack()
    {
        // Arrange
        MusicTrack musicTrack = new()
        {
            Id = Guid.Parse("31CA38B5-02E1-484B-B860-45D2104B29D2"),
            Title = "1000 Blunts",
            Description = "Yada yada idk",
            Length = TimeSpan.FromMinutes(2.5),
            Size = 2.5,
            UrlAddress = "https://music.youtube.com/watch?v=81uYuA1N4Qo&si=rwtXXPOzjGEqJnds"
        };

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var nonExistMusicTrack = await dbx.MusicTracks.SingleOrDefaultAsync(a => a.Id == musicTrack.Id);
        DeepAssert.Equal(null, nonExistMusicTrack);
    }

    [Fact]
    public async Task Create_MusicTrack_Has_Empty_CollectionProperties()
    {
        // Arrange
        MusicTrack musicTrack = new()
        {
            Id = Guid.Parse("66BF3504-6318-4083-AC28-8C07C4A8B3C1"),
            Title = "Immune to danger",
            Description = "Yada yada idk",
            Length = TimeSpan.FromMinutes(2.9),
            Size = 5,
            UrlAddress = "https://music.youtube.com/watch?v=u7ofPL4khCU&si=-x0Tysor2wLeGs8W"
        };

        // Act
        MusicDbContextSUT.MusicTracks.Add(musicTrack);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var actualMusicTrack = await dbx.MusicTracks
            .Include(mt => mt.Genres)
            .Include(mt => mt.Artists)
            .Include(mt => mt.Playlists)
            .FirstOrDefaultAsync(mt => mt.Id == musicTrack.Id);

        Assert.NotNull(actualMusicTrack);
        Assert.Empty(actualMusicTrack.Genres);
        Assert.Empty(actualMusicTrack.Artists);
        Assert.Empty(actualMusicTrack.Playlists);
    }

    [Fact]
    public async Task Update_MusicTrack_Title_And_Description()
    {
        // Arrange
        MusicTrack track = new()
        {
            Id = Guid.Parse("C7620380-CE01-4F2D-8D1C-108385955644"),
            Title = "Original Title",
            Description = "Original description",
            Length = TimeSpan.FromMinutes(3),
            Size = 5.0,
            UrlAddress = "http://example.com"
        };

        MusicDbContextSUT.MusicTracks.Add(track);
        await MusicDbContextSUT.SaveChangesAsync();

        var updatedTitle = "Updated Title";
        var updatedDescription = "Updated description";

        // Act
        track.Title = updatedTitle;
        track.Description = updatedDescription;
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var updatedTrack = await dbx.MusicTracks.FindAsync(track.Id);
        Assert.NotNull(updatedTrack);
        Assert.Equal(updatedTitle, updatedTrack.Title);
        Assert.Equal(updatedDescription, updatedTrack.Description);
    }

    [Fact]
    public async Task Update_MusicTrack_Length_And_Size()
    {
        // Arrange
        MusicTrack track = new()
        {
            Id = Guid.Parse("D32F21D7-FBE3-43F5-A809-FD0540BB7AA1"),
            Title = "Track for Length and Size update",
            Description = "Some description",
            Length = TimeSpan.FromMinutes(3),
            Size = 5.0,
            UrlAddress = "http://example.com"
        };

        MusicDbContextSUT.MusicTracks.Add(track);
        await MusicDbContextSUT.SaveChangesAsync();

        // Act
        track.Length = TimeSpan.FromMinutes(4);
        track.Size = 6.5;
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var updatedTrack = await dbx.MusicTracks.FindAsync(track.Id);
        Assert.NotNull(updatedTrack);
        Assert.Equal(TimeSpan.FromMinutes(4), updatedTrack.Length);
        Assert.Equal(6.5, updatedTrack.Size);
    }

    [Fact]
    public async Task Update_MusicTrack_UrlAddress()
    {
        // Arrange
        MusicTrack track = new()
        {
            Id = Guid.Parse("E4781828-2AB3-4067-8770-3FD921C50E7A"),
            Title = "Track for URL update",
            Description = "Some description",
            Length = TimeSpan.FromMinutes(3),
            Size = 5.0,
            UrlAddress = "http://example.com"
        };

        MusicDbContextSUT.MusicTracks.Add(track);
        await MusicDbContextSUT.SaveChangesAsync();

        var updatedUrl = "http://newexample.com";

        // Act
        track.UrlAddress = updatedUrl;
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var updatedTrack = await dbx.MusicTracks.FindAsync(track.Id);
        Assert.NotNull(updatedTrack);
        Assert.Equal(updatedUrl, updatedTrack.UrlAddress);
    }

    //Seeded tests

    [Fact]
    public async Task Get_MusicTrack_By_Id()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var retMusicTrack = await dbx.MusicTracks.FindAsync(MusicTrackSeeds.NonEmptyMusicTrack1.Id);

        // Assert
        Assert.NotNull(retMusicTrack);
        Assert.Equal(MusicTrackSeeds.NonEmptyMusicTrack1.Title, retMusicTrack.Title);
    }

    [Fact]
    public async Task Seeded_MusicTrack_Has_Correct_Genres()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var musicTrack = await dbx.MusicTracks
            .Include(a => a.Genres)
            .FirstOrDefaultAsync(a => a.Id == MusicTrackSeeds.NonEmptyMusicTrack1.Id);

        // Assert
        Assert.NotNull(musicTrack);
        Assert.Contains(musicTrack.Genres, t => t.Id == GenreSeeds.NonEmptyGenre.Id);
        Assert.Contains(musicTrack.Genres, t => t.Id == GenreSeeds.GenreUpdate.Id);
    }

    [Fact]
    public async Task Seeded_MusicTrack_Has_Zero_Genres()
    {
        // Act
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var musicTrack = await dbx.MusicTracks
            .Include(a => a.Genres)
            .FirstOrDefaultAsync(a => a.Id == MusicTrackSeeds.EmptyMusicTrack.Id);

        // Assert
        Assert.NotNull(musicTrack);
        Assert.Empty(musicTrack.Genres);
    }

    [Fact]
    public async Task Update_Seeded_MusicTrack_Name()
    {
        // Retrieve 
        var musicTrack = await MusicDbContextSUT.MusicTracks.FindAsync(MusicTrackSeeds.MusicTrackUpdate.Id);
        Assert.NotNull(musicTrack);

        // Act
        var updatedTitleName = "Updated Title Name";
        musicTrack.Title = updatedTitleName;
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var updatedMusicTrack = await dbx.MusicTracks.FindAsync(MusicTrackSeeds.MusicTrackUpdate.Id);
        Assert.Equal(updatedTitleName, updatedMusicTrack.Title);
    }

    [Fact]
    public async Task Delete_Seeded_MusicTrack()
    {
        // Retrieve 
        var musicTrack = await MusicDbContextSUT.MusicTracks.FindAsync(MusicTrackSeeds.MusicTrackDelete.Id);
        Assert.NotNull(musicTrack);

        // Act
        MusicDbContextSUT.MusicTracks.Remove(musicTrack);
        await MusicDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
        var deletedMusicTrack = await dbx.MusicTracks.FindAsync(MusicTrackSeeds.MusicTrackDelete.Id);
        Assert.Null(deletedMusicTrack);
    }
}