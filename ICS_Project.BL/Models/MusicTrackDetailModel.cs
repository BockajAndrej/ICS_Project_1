using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Models;

public partial class MusicTrackDetailModel : ModelBase
{
    [ObservableProperty]
    public partial string Title { get; set; }
    [ObservableProperty]
    public partial string Description { get; set; }
    [ObservableProperty]
    public partial TimeSpan Length { get; set; }
    [ObservableProperty]
    public partial double Size { get; set; }
    [ObservableProperty]
    public partial string UrlAddress { get; set; }

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