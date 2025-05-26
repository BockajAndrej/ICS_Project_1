using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.Common.Tests.Seeds;

public static class GenreSeeds
{
    public static readonly Genre EmptyGenre = new()
    {
        Id = default,
        GenreName = "Unknown Genre" // Prevents Null issue in SQLite, instead of "default"
    };

    public static readonly Genre NonEmptyGenre = new()
    {
        Id = Guid.Parse("88725A37-F6BE-4A21-9142-0A556BA23B75"),
        GenreName = "Pop"
    };

    public static readonly Genre EmptyGenreName =
        Clone( "777A6748-59F0-42DD-8860-567D1A7E49DA", string.Empty);

    public static readonly Genre GenreEmptyMusicTracks =
        Clone( "7C00C163-B12F-4DD6-8268-718D4243DAD7", "Rock");

    public static readonly Genre GenreUpdate =
        Clone( "68F8EE00-F3AE-4BB3-8D0C-3695BC1B839C", "Rap");

    public static readonly Genre GenreDelete =
        Clone("0E18CDB4-619B-4835-8602-3321A8A82AFA", "Country");

    public static Genre GenreClone(string id, string name)
    {
        return Clone(id, name);
    }   
    
    static GenreSeeds()
    {
        NonEmptyGenre.MusicTracks.Add(MusicTrackSeeds.NonEmptyMusicTrack1);
        NonEmptyGenre.MusicTracks.Add(MusicTrackSeeds.NonEmptyMusicTrack2);

        GenreDelete.MusicTracks.Add(MusicTrackSeeds.NonEmptyMusicTrack1); 
    }

    public static DbContext SeedGenres(this DbContext dbx)
    {
        dbx.Set<Genre>().AddRange(
            EmptyGenre,
            NonEmptyGenre,
            EmptyGenreName,
            GenreEmptyMusicTracks,
            GenreUpdate,
            GenreDelete);
        return dbx;
    }

    // Shallow Clone Method for Genres
    private static Genre Clone(string newId, string newName)
    {
        return new Genre
        {
            Id = Guid.Parse(newId),
            GenreName = newName,
            MusicTracks = new List<MusicTrack>()
        };
    }
}