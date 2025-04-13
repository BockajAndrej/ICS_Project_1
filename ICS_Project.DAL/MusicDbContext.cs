using ICS_Project.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using ICS_Project.DAL.Seeds;

namespace ICS_Project.DAL
{
    public class MusicDbContext(DbContextOptions options, bool seedDemoData) : DbContext(options)
    {
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<MusicTrack> MusicTracks => Set<MusicTrack>();
        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Genre> Genres => Set<Genre>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Artist>()
                .HasMany(a => a.MusicTracks)
                .WithMany(m => m.Artists)
                .UsingEntity(j => j.ToTable("ArtistMusicTrack"));

            modelBuilder.Entity<Genre>()
                .HasMany(g => g.MusicTracks)
                .WithMany(m => m.Genres)
                .UsingEntity(j => j.ToTable("GenreMusicTrack"));

            modelBuilder.Entity<Playlist>()
                .HasMany(p => p.MusicTracks)
                .WithMany(m => m.Playlists)
                .UsingEntity(j => j.ToTable("PlaylistMusicTrack"));
            
        }
    }
}