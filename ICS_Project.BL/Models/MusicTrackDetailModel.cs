using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Models;

public partial class MusicTrackDetailModel : ModelBase
{
    public string Title { get; set; }
    public string Description { get; set; }
    public TimeSpan Length { get; set; }
    public double Size { get; set; }
    public string UrlAddress { get; set; }

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