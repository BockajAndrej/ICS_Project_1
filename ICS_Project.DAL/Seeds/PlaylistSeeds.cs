using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.DAL.Seeds;

public static class PlaylistSeeds
{
    public static readonly Playlist ChillVibes = new()
    {
        Id = Guid.Parse("d4e5f6a1-b2c3-4567-8901-abcdef012345"),
        Name = "Chill Vibes",
        Description = "Relaxing tunes for unwinding.",
        NumberOfMusicTracks = 0,
        TotalPlayTime = TimeSpan.Zero
    };

    public static readonly Playlist WorkoutMix = new()
    {
        Id = Guid.Parse("e5f6a1b2-c3d4-5678-9012-bcdef0123456"),
        Name = "Workout Mix",
        Description = "High-energy tracks for the gym.",
        NumberOfMusicTracks = 0,
        TotalPlayTime = TimeSpan.Zero
    };

    public static readonly Playlist RoadTripAnthems = new()
    {
        Id = Guid.Parse("f6a1b2c3-d4e5-6789-0123-cdef01234567"),
        Name = "Road Trip Anthems",
        Description = "Sing-along hits for the open road.",
        NumberOfMusicTracks = 0,
        TotalPlayTime = TimeSpan.Zero
    };

    public static MusicDbContext SeedPlaylists(this MusicDbContext db)
    {
        db.Set<Playlist>().AddRange(
            ChillVibes,
            WorkoutMix,
            RoadTripAnthems
        );
        return db;
    }
}