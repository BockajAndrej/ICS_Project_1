using ICS_Project.App.ViewModels.MusicTrack;
using CommunityToolkit.Maui.Views;

namespace ICS_Project.App.Views.MusicTrack.Popups;

public partial class MusicTrackDetailView : Popup
{
	public MusicTrackDetailView(MusicTrackDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

    }
}