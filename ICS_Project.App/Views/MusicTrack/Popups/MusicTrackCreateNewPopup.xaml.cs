using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.ViewModels.MusicTrack;
using ICS_Project.App.Messages;
using ICS_Project.App.Views.Artist.Popups;
using System.Diagnostics;

namespace ICS_Project.App.Views.MusicTrack;

public partial class MusicTrackCreateNewPopup : Popup
{

    private readonly IServiceProvider _serviceProvider;

    public MusicTrackCreateNewPopup(MusicTrackCreateNewPopupModel musicTrackCreateNewPopupModel, IServiceProvider serviceProvider)
	{
		InitializeComponent();
        _serviceProvider = serviceProvider;
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

    private async void OnNewArtistClicked(object sender, EventArgs e)
    {
        var popup = _serviceProvider.GetRequiredService<ArtistEditView>();

        WeakReferenceMessenger.Default.Send(new PlaylistPopupContext { IsEditMode = false });

        // --- Find the parent Page ---
        // We need to traverse up the visual tree from the ContentView ('this')
        // until we find the Page that contains it.

        // Idealy we would like to display second popup over the first one,
        // but since maui does not support popup stacking, we can only display it over original page
        // -> Therefore it looks kinda weird (mainly the fact, that the og page gets even darker but the first popup remains the same)
        Element parent = this;
        while (parent != null && !(parent is Page))
        {
            parent = parent.Parent; // Go up one level
        }

        // Cast the found parent element to a Page
        Page currentPage = parent as Page;
        // --- End Finding Page ---


        // --- Show the Popup using the Page context ---
        if (currentPage != null)
        {

            // Now call ShowPopup on the Page instance.
            // The toolkit will use the Anchor (if set) and the Page context
            // to display the popup correctly.
            currentPage.ShowPopup(popup);
        }
        else
        {
            // Should not happen in a normal MAUI app structure, but good to handle.
            Debug.WriteLine("Error: Could not find the parent Page to display the popup.");
            // Maybe show an alert or log this error properly.
        }
        // --- End Showing Popup ---
    }
}