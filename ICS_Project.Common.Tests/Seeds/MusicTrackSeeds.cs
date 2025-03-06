using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.Common.Tests.Seeds;

public static class MusicTrackSeeds
{
    public static readonly MusicTrack EmptyMusicTrack = new()
    {
        Id = default,
        Title = default,
        Description = default,
        Length = default,
        Size = default,
        UrlAddress = default
    };

    public static readonly MusicTrack NonEmptyMusicTrack1 = new()
    {
        Id = Guid.Parse("3EECF1A0-EFEC-4D2B-9485-EFDCE77DAA9E"),
        Title = "IKnowImNotAHero",
        Description = "Lorem ipsum",
        Length = TimeSpan.FromMinutes(3),
        Size = 3,
        UrlAddress = "https://music.youtube.com/watch?v=wWcPE1obgu0&si=0cjE8n0D_H9oF3w7"
    };

    public static readonly MusicTrack NonEmptyMusicTrack2 = new()
    {
        Id = Guid.Parse("4E3569E2-6E84-4FAE-A8E7-43E96AE6B949"),
        Title = "Scargazer",
        Description = "Lorem ipsum dolor sit amet",
        Length = TimeSpan.FromMinutes(2.5),
        Size = 4,
        UrlAddress = "https://music.youtube.com/watch?v=rfzC6cLT7Q0&si=Ug_nZCpmiGdeH2RM"
    };

    public static readonly MusicTrack MusicTrackUpdate = Clone(NonEmptyMusicTrack1,
        "1B5E72D2-8C36-41A8-AD8A-9878F8E265D3", "Updated Track", "Updated Description");

    public static readonly MusicTrack MusicTrackDelete = Clone(NonEmptyMusicTrack2,
        "8F2C0D1D-7E60-4A1C-ACF9-6AFA19C8411D", "Track to Delete", "For deletion test");

    static MusicTrackSeeds()
    {
        NonEmptyMusicTrack1.Genres.Add(GenreSeeds.NonEmptyGenre);
        NonEmptyMusicTrack1.Artists.Add(ArtistSeeds.Artist);

        NonEmptyMusicTrack2.Artists.Add(ArtistSeeds.Artist);
        
    }

    public static DbContext SeedMusicTracks(this DbContext dbx)
    {
        dbx.Set<MusicTrack>().AddRange(
            EmptyMusicTrack,
            NonEmptyMusicTrack1,
            NonEmptyMusicTrack2,
            MusicTrackUpdate,
            MusicTrackDelete);
        return dbx;
    }

    public static MusicTrack Clone(MusicTrack original, string newId, string newTitle, string newDescription)
    {
        return new MusicTrack
        {
            Id = Guid.Parse(newId),
            Title = newTitle,
            Description = newDescription,
            Length = original.Length,
            Size = original.Size,
            UrlAddress = original.UrlAddress,
            Playlists = new List<Playlist>(),
            Genres = new List<Genre>(),
            Artists = new List<Artist>()
        };
    }
}