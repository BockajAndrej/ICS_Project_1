using ICS_Project.DAL.Entities;

namespace ICS_Project.DAL.Seeds;

public static class MusisTrackSeeds
{
    // Track associated with Ramirez
    public static readonly MusicTrack TheMysticalWarlock= new()
    {
        Id = Guid.Parse("53430515-7252-44ae-90af-c8940dd88118"),
        Title = "The Mystical Warlock",
        Description = "Classic track by Ramirez.",
        Length = TimeSpan.FromMinutes(2) + TimeSpan.FromSeconds(28), 
        Size = 5.5, 
        UrlAddress = "http://example.audio/ramirez_grey_gorilla.mp3"
    };

    public static readonly MusicTrack KnockKnock = new()
    {
        Id = Guid.Parse("9cc0d994-aa0d-4c7c-8ed2-3eb4f8c17a25"),
        Title = "KnockKnock",
        Description = "Popular track by Chetta.",
        Length = TimeSpan.FromMinutes(3) + TimeSpan.FromSeconds(11),
        Size = 6.8,
        UrlAddress = "http://example.audio/chetta_bleach.mp3"
    };

    public static readonly MusicTrack IKnow = new()
    {
        Id = Guid.Parse("a4c692a0-6091-4cc7-a0a9-a039dd330221"),
        Title = "The Less I Know The Better",
        Description = "Iconic hit by Tame Impala.",
        Length = TimeSpan.FromMinutes(3) + TimeSpan.FromSeconds(36),
        Size = 7.5,
        UrlAddress = "http://example.audio/tameimpala_lessiknow.mp3"
    };

    public static MusicDbContext SeedMusicTracks(this MusicDbContext db)
    {
        db.Set<MusicTrack>().AddRange(
            TheMysticalWarlock,
            KnockKnock,
            IKnow
        );

        return db;
    }
}