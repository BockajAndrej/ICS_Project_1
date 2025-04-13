using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.Common.Tests.Seeds;

public static class ArtistSeeds
{
    public static readonly Artist EmptyArtist = new()
    {
        Id = default,
        ArtistName = "Unknown Artist" // Prevents Null issue in SQLite, instead of "default"
    };

    public static readonly Artist Artist = new()
    {
        Id = Guid.Parse(input: "84E2430E-E8EB-4982-A1F2-19C2AAE4FBE9"),
        ArtistName = "Scrim"
    };

    public static readonly Artist EmptyArtistName =
        Clone(Artist, "60E299A2-BDC6-45F3-9391-38CE09CC665B", string.Empty);

    public static readonly Artist ArtistUpdate =
        Clone(Artist, "C43A05ED-20D0-4E68-B086-B0998A26914F", "Update me");

    public static readonly Artist ArtistDelete =
        Clone(Artist, "58D0C03C-C539-4A16-96B1-9A9527B1BA0F", "Delete me");
    
    public static readonly Artist ArtistWOutTracks =
        Clone(Artist, "1c2cb57c-3f7f-485d-8e4e-b78228644581", "I cannot Sing");

    public static Artist ArtistClone(string id, string artistName)
    {
        return Clone(Artist, id, artistName);
    }


    static ArtistSeeds()
    {
        Artist.MusicTracks.Add(MusicTrackSeeds.NonEmptyMusicTrack1);
        Artist.MusicTracks.Add(MusicTrackSeeds.NonEmptyMusicTrack2);

        ArtistUpdate.MusicTracks.Add(MusicTrackSeeds.NonEmptyMusicTrack1);

        MusicTrackSeeds.NonEmptyMusicTrack1.Artists.Add(Artist);
        MusicTrackSeeds.NonEmptyMusicTrack2.Artists.Add(Artist);
    }

    public static DbContext SeedArtists(this DbContext context)
    {
        context.Set<Artist>().AddRange(
            EmptyArtist,
            Artist,
            EmptyArtistName,
            ArtistUpdate,
            ArtistDelete,
            ArtistWOutTracks);
        return context;
    }

    // Manual Clone Method for Artists
    private static Artist Clone(Artist original, string newId, string newName)
    {
        return new Artist
        {
            Id = Guid.Parse(newId),
            ArtistName = newName,
            MusicTracks = new List<MusicTrack>()
        };
    }
}