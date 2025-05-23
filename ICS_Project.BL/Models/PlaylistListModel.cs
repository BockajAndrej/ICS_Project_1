using CommunityToolkit.Mvvm.ComponentModel;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Models;

public partial class PlaylistListModel : ModelBase
{
    
    public string Name { get; set; }
    public string Description { get; set; }
    
    public int NumberOfMusicTracks { get; set; }
    public TimeSpan TotalPlayTime { get; set; }

    public static PlaylistListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        Name = string.Empty,
        Description = string.Empty,
        NumberOfMusicTracks = 0,
        TotalPlayTime = TimeSpan.Zero
    };
}