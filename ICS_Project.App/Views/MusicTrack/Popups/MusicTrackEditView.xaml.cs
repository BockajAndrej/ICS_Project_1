using ICS_Project.App.ViewModels.MusicTrack;
using CommunityToolkit.Maui.Views;
using ICS_Project.App.ViewModels.MusicTrack;
using System.Diagnostics; // Ensure this using is correct

namespace ICS_Project.App.Views.MusicTrack.Popups;

public partial class MusicTrackEditView : Popup
{
	public MusicTrackEditView(MusicTrackDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private void ClosePopup(object sender, EventArgs e)
    {
        Debug.WriteLine("ClosePopup: Closing popup after button click.");
        this.Close();
    }
}