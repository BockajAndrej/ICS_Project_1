using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace ICS_Project.DAL.Tests
{
    [Collection("Sequential")]
    public class DbContextPlaylistTests(ITestOutputHelper output) : DbContextTestsBase(output)
    {
        [Fact]
        public async Task Create_And_Check_Playlist()
        {
            // Arrange
            Playlist playlist = new()
            {
                Id = Guid.Parse("566BCEDE-1D8A-4B3B-94F8-39C47237ED24"),
                Name = "Chill Vibes",
                Description = "Relaxing music",
                NumberOfMusicTracks = 0,
                TotalPlayTime = TimeSpan.Zero
            };

            // Act
            MusicDbContextSUT.Playlists.Add(playlist);
            await MusicDbContextSUT.SaveChangesAsync();

            // Assert
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var actualPlaylist = await dbx.Playlists.SingleAsync(x => x.Id == playlist.Id);
            DeepAssert.Equal(playlist, actualPlaylist);
        }

        [Fact]
        public async Task Create_And_Delete_Playlist()
        {
            // Arrange
            Playlist playlist = new()
            {
                Id = Guid.Parse("769FC701-D52F-4B27-8B32-7D44047A2E92"),
                Name = "Party Hits",
                Description = "Upbeat music for parties",
                NumberOfMusicTracks = 2,
                TotalPlayTime = TimeSpan.FromMinutes(60)
            };
            MusicDbContextSUT.Playlists.Add(playlist);
            await MusicDbContextSUT.SaveChangesAsync();

            // Act
            MusicDbContextSUT.Playlists.Remove(playlist);
            await MusicDbContextSUT.SaveChangesAsync();

            // Assert
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var deletedPlaylist = await dbx.Playlists.SingleOrDefaultAsync(a => a.Id == playlist.Id);
            Assert.Null(deletedPlaylist);
        }

        [Fact]
        public async Task Find_NonExist_Playlist()
        {
            // Arrange
            var nonExistId = Guid.Parse("52A3BEF8-2850-47C1-858E-6836AB81F4C8");

            // Assert
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var nonExistPlaylist = await dbx.Playlists.SingleOrDefaultAsync(a => a.Id == nonExistId);
            Assert.Null(nonExistPlaylist);
        }

        [Fact]
        public async Task Create_Playlist_Has_Empty_MusicTracks()
        {
            // Arrange
            Playlist playlist = new()
            {
                Id = Guid.Parse("025063A0-966D-4372-A4C0-06729829C53A"),
                Name = "Solo Playlist",
                Description = "No tracks yet",
                NumberOfMusicTracks = 0,
                TotalPlayTime = TimeSpan.Zero
            };

            // Act
            MusicDbContextSUT.Playlists.Add(playlist);
            await MusicDbContextSUT.SaveChangesAsync();

            // Assert
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var actualPlaylist = await dbx.Playlists
                .Include(p => p.MusicTracks)
                .FirstOrDefaultAsync(p => p.Id == playlist.Id);
            Assert.NotNull(actualPlaylist);
            Assert.Empty(actualPlaylist.MusicTracks);
        }

        [Fact]
        public async Task Update_Playlist_Name_And_Description()
        {
            // Arrange
            Playlist playlist = new()
            {
                Id = Guid.Parse("68C00928-BD8B-4B43-86CA-B1F7A1541003"),
                Name = "Original Playlist Name",
                Description = "Original description",
                NumberOfMusicTracks = 3,
                TotalPlayTime = TimeSpan.FromMinutes(90)
            };

            MusicDbContextSUT.Playlists.Add(playlist);
            await MusicDbContextSUT.SaveChangesAsync();

            var updatedName = "Updated Playlist Name";
            var updatedDescription = "Updated description";

            // Act
            playlist.Name = updatedName;
            playlist.Description = updatedDescription;
            await MusicDbContextSUT.SaveChangesAsync();

            // Assert
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var updatedPlaylist = await dbx.Playlists.FindAsync(playlist.Id);
            Assert.NotNull(updatedPlaylist);
            Assert.Equal(updatedName, updatedPlaylist.Name);
            Assert.Equal(updatedDescription, updatedPlaylist.Description);
        }

        [Fact]
        public async Task Update_Playlist_NumberOfMusicTracks_And_TotalPlayTime()
        {
            // Arrange
            Playlist playlist = new()
            {
                Id = Guid.Parse("EA867C1B-44AF-40E3-B961-6EBCFB571CD9"),
                Name = "Energy Booster",
                Description = "Get pumped up",
                NumberOfMusicTracks = 4,
                TotalPlayTime = TimeSpan.FromMinutes(80)
            };

            MusicDbContextSUT.Playlists.Add(playlist);
            await MusicDbContextSUT.SaveChangesAsync();

            var updatedNumberOfTracks = 5;
            var updatedTotalPlayTime = TimeSpan.FromHours(2);

            // Act
            playlist.NumberOfMusicTracks = updatedNumberOfTracks;
            playlist.TotalPlayTime = updatedTotalPlayTime;
            await MusicDbContextSUT.SaveChangesAsync();

            // Assert
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var updatedPlaylist = await dbx.Playlists.FindAsync(playlist.Id);
            Assert.NotNull(updatedPlaylist);
            Assert.Equal(updatedNumberOfTracks, updatedPlaylist.NumberOfMusicTracks);
            Assert.Equal(updatedTotalPlayTime, updatedPlaylist.TotalPlayTime);
        }

        // Seeded tests

        [Fact]
        public async Task Get_Playlist_By_Id()
        {
            // Act
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var retPlaylist = await dbx.Playlists.FindAsync(PlaylistSeeds.NonEmptyPlaylist.Id);

            // Assert
            Assert.NotNull(retPlaylist);
            Assert.Equal(PlaylistSeeds.NonEmptyPlaylist.Name, retPlaylist.Name);
        }

        [Fact]
        public async Task Seeded_Playlist_Has_Correct_MusicTracks()
        {
            // Act
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var playlist = await dbx.Playlists
                .Include(p => p.MusicTracks)
                .FirstOrDefaultAsync(p => p.Id == PlaylistSeeds.NonEmptyPlaylist.Id);

            // Assert
            Assert.NotNull(playlist);
            Assert.Contains(playlist.MusicTracks, mt => mt.Id == MusicTrackSeeds.NonEmptyMusicTrack1.Id);
        }

        [Fact]
        public async Task Seeded_Playlist_Has_Zero_MusicTracks()
        {
            // Act
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var playlist = await dbx.Playlists
                .Include(p => p.MusicTracks)
                .FirstOrDefaultAsync(p => p.Id == PlaylistSeeds.EmptyPlaylist.Id);

            // Assert
            Assert.NotNull(playlist);
            Assert.Empty(playlist.MusicTracks);
        }

        [Fact]
        public async Task Update_Seeded_Playlist_Name()
        {
            // Retrieve
            var playlist = await MusicDbContextSUT.Playlists.FindAsync(PlaylistSeeds.PlaylistUpdate.Id);
            Assert.NotNull(playlist);

            // Act
            var updatedName = "New Seeded Playlist Name";
            playlist.Name = updatedName;
            await MusicDbContextSUT.SaveChangesAsync();

            // Assert
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var updatedPlaylist = await dbx.Playlists.FindAsync(PlaylistSeeds.PlaylistUpdate.Id);
            Assert.NotNull(updatedPlaylist);
            Assert.Equal(updatedName, updatedPlaylist.Name);
        }

        [Fact]
        public async Task Delete_Seeded_Playlist()
        {
            // Retrieve
            var playlist = await MusicDbContextSUT.Playlists.FindAsync(PlaylistSeeds.PlaylistDelete.Id);
            Assert.NotNull(playlist);

            // Act
            MusicDbContextSUT.Playlists.Remove(playlist);
            await MusicDbContextSUT.SaveChangesAsync();

            // Assert
            await using var dbx = DbContextFactory.CreateDbContext(Array.Empty<string>());
            var deletedPlaylist = await dbx.Playlists.FindAsync(PlaylistSeeds.PlaylistDelete.Id);
            Assert.Null(deletedPlaylist);
        }
    }
}