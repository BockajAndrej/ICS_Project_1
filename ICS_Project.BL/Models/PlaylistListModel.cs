using CommunityToolkit.Mvvm.ComponentModel;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Models;

public partial class PlaylistListModel : ModelBase
{
    [ObservableProperty]
    public partial string Name { get; set; }
    [ObservableProperty]
    public partial string Description { get; set; }
    [ObservableProperty]
    public partial int NumberOfMusicTracks { get; set; }
    [ObservableProperty]
    public partial TimeSpan TotalPlayTime { get; set; }

    public static PlaylistListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Description = string.Empty,
        NumberOfMusicTracks = 0,
        TotalPlayTime = TimeSpan.Zero
    };
}