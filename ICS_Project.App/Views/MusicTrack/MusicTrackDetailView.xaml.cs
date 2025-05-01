using ICS_Project.App.ViewModels.MusicTrack;

namespace ICS_Project.App.Views.MusicTrack;

public partial class MusicTrackDetailView : ContentView
{
	public MusicTrackDetailView()
	{
		InitializeComponent();
        BindingContext = new MusicTrackDetailViewModel();

    }
}