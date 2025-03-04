namespace ICS_Project.DAL.Entities;

public class MusicTrack : IEntity
{
    public required Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TimeSpan Length { get; set; }
    public double Size { get; set; }
    public string UrlAddress { get; set; }

    public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Artist> Artists { get; set; } = new List<Artist>();

}