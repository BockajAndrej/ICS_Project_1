using ICS_Project.App.ViewModels.MusicTrack;
using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.Views.Playlist;

namespace ICS_Project.App.Views.MusicTrack;

public partial class MusicTrackView : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    public MusicTrackView(PlaylistListViewModel playlist, MusicTrackListViewModel musicTrack, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;

        Grid mainGrid = this.Content as Grid;

        if (mainGrid != null)
        {
            // 1. Created instances of UI elements
            var playlistListView = new PlaylistListView(playlist, serviceProvider);
            var musicTrackListView = new MusicTrackListView(musicTrack);

            // 2. Created connection with Grid.Column
            Grid.SetColumn(playlistListView, 0);
            Grid.SetColumn(musicTrackListView, 1);

            // 3. Add elements into collection of grid childs
            mainGrid.Children.Add(playlistListView);
            mainGrid.Children.Add(musicTrackListView);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Chyba: Root element nie je Grid alebo sa nenašiel Grid.");
        }

        _serviceProvider = serviceProvider;
    }
}