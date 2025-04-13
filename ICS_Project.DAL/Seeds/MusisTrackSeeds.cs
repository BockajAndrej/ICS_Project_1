using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.DAL.Seeds;

public static class MusisTrackSeeds
{
    public static readonly MusicTrack MidnightDrive = new()
    {
        Id = Guid.Parse("e1f2a3b4-c5d6-7890-1234-abcdef123456"),
        Title = "Midnight Drive",
        Description = "A late-night driving track.",
        Length = TimeSpan.FromMinutes(4) + TimeSpan.FromSeconds(15),
        Size = 8.5, // Example size in MB
        UrlAddress = "http://example.audio/midnight_drive.mp3"
    };

    public static readonly MusicTrack SunriseGroove = new()
    {
        Id = Guid.Parse("f2a3b4c5-d6e7-8901-2345-bcdef1234567"),
        Title = "Sunrise Groove",
        Description = "An upbeat morning vibe.",
        Length = TimeSpan.Parse("00:03:30"),
        Size = 6.2,
        UrlAddress = "http://example.audio/sunrise_groove.mp3"
    };

    public static readonly MusicTrack DesertEchoes = new()
    {
        Id = Guid.Parse("a3b4c5d6-e7f8-9012-3456-cdef12345678"),
        Title = "Desert Echoes",
        Description = "Ambient soundscape.",
        Length = new TimeSpan(0, 5, 55),
        Size = 10.1,
        UrlAddress = "http://example.audio/desert_echoes.mp3"
    };

    public static MusicDbContext SeedMusicTracks(this MusicDbContext db)
    {
        db.Set<MusicTrack>().AddRange(
            MidnightDrive,
            SunriseGroove,
            DesertEchoes
        );

        return db; // Return the context for chaining
    }
}