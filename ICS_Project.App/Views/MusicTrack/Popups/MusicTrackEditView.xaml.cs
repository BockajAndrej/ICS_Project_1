using ICS_Project.App.ViewModels.MusicTrack;
using CommunityToolkit.Maui.Views;
using ICS_Project.App.ViewModels.MusicTrack; // Ensure this using is correct

namespace ICS_Project.App.Views.MusicTrack.Popups;

public partial class MusicTrackEditView : Popup
{
	public MusicTrackEditView(MusicTrackDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}