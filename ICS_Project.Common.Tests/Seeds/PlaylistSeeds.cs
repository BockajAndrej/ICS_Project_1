using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.Common.Tests.Seeds;

public static class PlaylistSeeds
{
    public static readonly Playlist EmptyPlaylist = new()
    {
        Id = default,
        Name = "Unknown Name", // Prevents Null issue in SQLite, instead of "default"
        Description = "Unknown Description", 
        NumberOfMusicTracks = default,
        TotalPlayTime = default
    };

    public static readonly Playlist NonEmptyPlaylist = new()
    {
        Id = Guid.Parse("50A41C82-D39B-4ED4-B867-F3EF536916AF"),
        Name = "Sleep",
        Description = "Fall asleep in 3 seconds",
        NumberOfMusicTracks = 1,
        TotalPlayTime = TimeSpan.FromHours(1)
    };

    public static readonly Playlist PlaylistUpdate = Clone(NonEmptyPlaylist, "B7ECBCF6-647F-4708-A906-6B0C1DFE4FDC",
        "Updated Playlsit", "Updated description");

    public static readonly Playlist PlaylistDelete = Clone(NonEmptyPlaylist, "183D822D-7E48-4DC7-8F06-0DC50816D07B",
        "To Be Deleted", "Playlist meant for deletion tests");

    static PlaylistSeeds()
    {
        PlaylistDelete.MusicTracks.Add(MusicTrackSeeds.NonEmptyMusicTrack1);

        NonEmptyPlaylist.MusicTracks.Add(MusicTrackSeeds.NonEmptyMusicTrack1);

    }

    public static DbContext SeedPlaylists(this DbContext dbx)
    {
        dbx.Set<Playlist>().AddRange(
            EmptyPlaylist,
            NonEmptyPlaylist,
            PlaylistUpdate,
            PlaylistDelete
        );
        return dbx;
    }

    public static Playlist Clone(Playlist original, string newId, string newName, string newDescription)
    {
        return new Playlist
        {
            Id = Guid.Parse(newId),
            Name = newName,
            Description = newDescription,
            NumberOfMusicTracks = original.NumberOfMusicTracks,
            TotalPlayTime = original.TotalPlayTime,
            MusicTracks = new List<MusicTrack>() // Prevents shared state issues
        };
    }
}