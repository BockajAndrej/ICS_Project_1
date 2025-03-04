namespace ICS_Project.DAL.Entities;

public class Genre : IEntity
{
    public required Guid Id { get; set; }
    public string GenreName { get; set; }

    public ICollection<MusicTrack> MusicTracks { get; set; } = new List<MusicTrack>();

}