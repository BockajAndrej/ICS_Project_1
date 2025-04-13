using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.DAL.Seeds;

public static class PlaylistSeeds
{
    public static readonly Playlist LudovkyPlaylist = new()
    {
        Id = Guid.Parse("fabde0cd-eefe-443f-baf6-3d96cc2cbf2e"),
        Name = "Ludovky",
        Description = "Ludovky playlist",
        NumberOfMusicTracks = 1,
        TotalPlayTime = TimeSpan.FromMinutes(2)
    };

    static PlaylistSeeds()
    {
        LudovkyPlaylist.MusicTracks.Add(MusisTrackSeeds.MidnightDrive);
    }

    public static void Seed(this ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Genre>().HasData(
            LudovkyPlaylist
        );
}