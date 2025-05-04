// Add necessary using statements
using ICS_Project.App.ViewModels; // For ViewModelBase
using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.Views; // For ContentPageBase (if needed, check namespace)
using System.Diagnostics; // Keep for Debug.WriteLine if needed
using Microsoft.Maui.Controls; // Make sure this using exists

namespace ICS_Project.App.Views.Playlist;

// 1. Inherit from ContentPageBase
public partial class PlaylistView : ContentPageBase // <<< CORRECT INHERITANCE
{
    // Store the Detail ViewModel needed for the child view
    private readonly PlaylistDetailViewModel _playlistDetailViewModel;

    // 2. Constructor now takes the PAGE's ViewModel and other needed ViewModels, matching base
    public PlaylistView(
        PlaylistListViewModel viewModel, // This is the ViewModel for THIS page
        PlaylistDetailViewModel playlistDetailViewModel // Inject the other needed VM
        )
        : base(viewModel) // Pass the page's ViewModel to ContentPageBase constructor
    {
        InitializeComponent();

        // Store the injected Detail ViewModel to pass to the child view
        _playlistDetailViewModel = playlistDetailViewModel;

        // --- Child View Setup ---
        Grid mainGrid = this.Content as Grid;

        if (mainGrid != null)
        {
            // Create child views using the correct ViewModels
            // PlaylistListViewModel is available via the 'ViewModel' property from base class (or directly)
            var playlistListView = new PlaylistListView(viewModel);

            // PlaylistDetailViewModel was injected and stored
            var playlistDetailView = new PlaylistDetailView(_playlistDetailViewModel);

            // Set columns (same as before)
            Grid.SetColumn(playlistListView, 0);
            Grid.SetColumn(playlistDetailView, 1);

            // Add children (same as before)
            mainGrid.Children.Add(playlistListView);
            mainGrid.Children.Add(playlistDetailView);
        }
        else
        {
            Debug.WriteLine("Chyba: Root element nie je Grid alebo sa nenašiel Grid.");
        }
        // --- End Child View Setup ---
    }

    // Remove message handling logic if it was moved to AppShell
}