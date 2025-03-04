namespace ICS_Project.DAL.Entities;

public class Playlist : IEntity
{
    public required Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int NumberOfMusicTracks { get; set; }
    public TimeSpan TotalPlayTime { get; set; }

    public ICollection<MusicTrack> MusicTracks { get; set; } = new List<MusicTrack>();

}