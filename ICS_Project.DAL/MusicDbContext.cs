using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.DAL
{
    public class MusicDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<MusicTrack> MusicTracks => Set<MusicTrack>();
        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Genre> Genres => Set<Genre>();
    }
}