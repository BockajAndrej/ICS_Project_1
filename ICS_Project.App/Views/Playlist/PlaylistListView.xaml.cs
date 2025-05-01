using ICS_Project.App.ViewModels.Playlist;

namespace ICS_Project.App.Views.Playlist;

public partial class PlaylistListView : ContentView
{
    public PlaylistListView(PlaylistListViewModel playlistListViewModel)
    {
        InitializeComponent();

        this.BindingContext = playlistListViewModel;
    }
}