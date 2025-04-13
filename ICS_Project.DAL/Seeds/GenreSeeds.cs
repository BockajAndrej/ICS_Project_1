using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.DAL.Seeds;

public static class GenreSeeds
{
    public static readonly Genre Pop = new()
    {
        Id = Guid.Parse("fabde0cd-eefe-443f-baf6-3d96cc2cbf2e"),
        GenreName = "Pop",
    };

    public static readonly Genre Rap = new()
    {
        Id = Guid.Parse("0ea01c21-72dd-44bb-bb6a-e51b6bcb36c0"),
        GenreName = "Rap",
    };

    public static readonly Genre HipHop = new()
    {
        Id = Guid.Parse("a3fa7cac-9df5-4b20-babb-c9cee880a551"),
        GenreName = "Hip Hop"
    };

    public static MusicDbContext SeedArtists(this MusicDbContext db)
    {
        db.Set<Genre>().AddRange(
            Pop,
            Rap,
            HipHop
        );
        return db;
    }
}