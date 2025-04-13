using ICS_Project.DAL.Entities;

namespace ICS_Project.DAL.Seeds;

public static class ArtistsSeeds
{
    public static readonly Artist Chetta = new()
    {
        Id = Guid.Parse("44c2dad0-95cc-4df6-8248-5de9f73fc2e1"),
        ArtistName = "Chetta",
    };

    public static readonly Artist Ramirez = new()
    {
        Id = Guid.Parse("65818841-0b0d-4911-b941-ddfd4dc98142"),
        ArtistName = "Ramirez",
    };

    public static readonly Artist TameImpala = new()
    {
        Id = Guid.Parse("cf658738-0c80-4f15-b5ec-55401b4d589c"),
        ArtistName = "TameImpala",
    };

    public static MusicDbContext SeedArtists(this MusicDbContext db)
    {
        db.Set<Artist>().AddRange(
            Chetta,
            Ramirez,
            TameImpala
        );
        return db;
    }
}