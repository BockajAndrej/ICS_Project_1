using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ICS_Project.BL.Models;

public partial class PlaylistDetailModel : ModelBase
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public int NumberOfMusicTracks { get; set; }
    public TimeSpan TotalPlayTime { get; set; }

    public ObservableCollection<MusicTrackListModel> MusicTracks { get; init; } = new();

    public static PlaylistDetailModel Empty = new()
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Description = string.Empty,
        NumberOfMusicTracks = 0,
        TotalPlayTime = TimeSpan.Zero
    };

    public static PlaylistDetailModel CreateEmpty()
    {
        return new PlaylistDetailModel
        {
            Id = Guid.Empty,
            Name = string.Empty,
            Description = string.Empty,
            NumberOfMusicTracks = 0,
            TotalPlayTime = TimeSpan.Zero
        };
    }
}