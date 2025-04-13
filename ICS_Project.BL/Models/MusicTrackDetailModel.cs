using System.Collections.ObjectModel;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Models;

public class MusicTrackDetailModel : ModelBase
{
    public required string Title { get; set; }
    public string Description { get; set; }
    public required TimeSpan Length { get; set; }
    public required double Size { get; set; }
    public required string UrlAddress { get; set; }
    
    public ObservableCollection<PlaylistListModel> Playlists { get; init; } = new();
    public ObservableCollection<GenreListModel> Genres { get; init; } = new();
    public ObservableCollection<ArtistListModel> Artists { get; init; } = new();

    public static MusicTrackDetailModel Empty = new()
    {
        Id = Guid.NewGuid(),
        Title = string.Empty,
        Description = string.Empty,
        Length = TimeSpan.Zero,
        Size = 0,
        UrlAddress = string.Empty
    };
}