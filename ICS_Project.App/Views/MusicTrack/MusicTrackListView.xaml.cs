using ICS_Project.App.ViewModels.MusicTrack;
using ICS_Project.BL.Models;

namespace ICS_Project.App.Views.MusicTrack;

public partial class MusicTrackListView : ContentView
{
    public MusicTrackListView(MusicTrackListViewModel playlistViewModel)
    {
        InitializeComponent();

        BindingContext = playlistViewModel;
    }
}