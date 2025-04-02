using System.Collections.ObjectModel;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Models;

public class GenreDetailModel : ModelBase
{
    public required string GenreName { get; set; }
    
    public ObservableCollection<MusicTrackListModel> MusicTracks { get; init; } = new();
    
    public static GenreDetailModel Empty = new()
    {
        Id = Guid.NewGuid(),
        GenreName = string.Empty
    }; 
    
}