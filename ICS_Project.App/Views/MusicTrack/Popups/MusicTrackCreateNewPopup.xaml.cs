using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.ViewModels.MusicTrack;
using ICS_Project.App.Messages;

namespace ICS_Project.App.Views.MusicTrack;

public partial class MusicTrackCreateNewPopup : Popup
{
	public MusicTrackCreateNewPopup(MusicTrackCreateNewPopupModel musicTrackCreateNewPopupModel)
	{
		InitializeComponent();
        BindingContext = musicTrackCreateNewPopupModel;
        // Register the messenger to close the popup when the message is received
        WeakReferenceMessenger.Default.Register<MusicTrackNewMusicTrackClosed>(this, (r, m) =>
        {
            Close(); // closes the popup
        });
    }
    private void MusicTrackCreateNewPopup_Opened(object sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new MusicTrackNewMusicTrackOpened(true));
    }
}