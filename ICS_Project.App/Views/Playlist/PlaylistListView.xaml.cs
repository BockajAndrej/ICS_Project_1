using System.Diagnostics;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.BL.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ICS_Project.App.Views.Playlist;

public partial class PlaylistListView : ContentView
{
    private readonly IServiceProvider _serviceProvider;
    public PlaylistListView(PlaylistListViewModel playlistListViewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        this.BindingContext = playlistListViewModel;
    }

        private async void OnNewPlaylistClicked(object sender, EventArgs e)
        { 
            var popup = _serviceProvider.GetRequiredService<PlaylistCreateNewPopup>();

            WeakReferenceMessenger.Default.Send(new PlaylistPopupContext { IsEditMode = false });

            // --- Find the parent Page ---
            // We need to traverse up the visual tree from the ContentView ('this')
            // until we find the Page that contains it.
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