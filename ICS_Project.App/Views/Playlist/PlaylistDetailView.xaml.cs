using ICS_Project.App.ViewModels.Playlist;

namespace ICS_Project.App.Views.Playlist;

public partial class PlaylistDetailView : ContentView
{
    public PlaylistDetailView(PlaylistDetailViewModel playlistViewModel)
    {
        InitializeComponent();

        this.BindingContext = playlistViewModel;
    }
}