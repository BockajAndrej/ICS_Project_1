namespace ICS_Project.DAL.Entities;

public class Artist : IEntity
{
    public required Guid Id { get; set; }
    public string ArtistName { get; set; }

    public ICollection<MusicTrack> MusicTracks { get; set; } = new List<MusicTrack>();

}