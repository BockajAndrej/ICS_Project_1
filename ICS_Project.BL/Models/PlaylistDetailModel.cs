using System.Collections.ObjectModel;

namespace ICS_Project.BL.Models;

public class PlaylistDetailModel : ModelBase
{
    public required string Name { get; set; }
    public string Description  { get; set; }
    public required int NumberOfMusicTracks { get; set; }
    public required TimeSpan TotalPlayTime { get; set; }
    
    public ObservableCollection<MusicTrackListModel> MusicTracks { get; init; } = new();
    
    public static PlaylistDetailModel Empty = new()
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Description = string.Empty,
        NumberOfMusicTracks = 0,
        TotalPlayTime = TimeSpan.Zero
    }; 
}