using System.Collections.ObjectModel;

namespace ICS_Project.BL.Models;

public class ArtistDetailModel : ModelBase
{
    public required string ArtistName { get; set; }
    
    public ObservableCollection<MusicTrackListModel> MusicTrack { get; init; } = new();
    
    public static ArtistDetailModel Empty = new()
    {
        Id = Guid.NewGuid(),
        ArtistName = string.Empty
    }; 
}