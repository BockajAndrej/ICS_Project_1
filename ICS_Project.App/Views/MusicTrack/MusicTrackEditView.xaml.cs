using ICS_Project.App.ViewModels.MusicTrack;

namespace ICS_Project.App.Views.MusicTrack;

public partial class MusicTrackEditView : ContentView
{
	public MusicTrackEditView()
	{
		InitializeComponent();
        BindingContext = new MusicTrackEditViewModel();
    }
}