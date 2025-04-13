using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Models;

public class PlaylistListModel : ModelBase
{
    public required string Name { get; set; }
    public string Description  { get; set; }
    public required int NumberOfMusicTracks { get; set; }
    public required TimeSpan TotalPlayTime { get; set; }
    
    public static PlaylistListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Description = string.Empty,
        NumberOfMusicTracks = 0,
        TotalPlayTime = TimeSpan.Zero
    }; 
}