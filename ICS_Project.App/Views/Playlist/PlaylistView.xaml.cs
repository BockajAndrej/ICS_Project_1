using ICS_Project.App.ViewModels; 
using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.Views; 
using System.Diagnostics; 
using Microsoft.Maui.Controls; 

namespace ICS_Project.App.Views.Playlist;


public partial class PlaylistView : ContentPageBase 
{
    // Store the Detail ViewModel needed for the child view
    private readonly PlaylistDetailViewModel _playlistDetailViewModel;

    // 2. Constructor now takes the PAGE's ViewModel and other needed ViewModels, matching base
    public PlaylistView(
        PlaylistListViewModel viewModel, // This is the ViewModel for THIS page
        PlaylistDetailViewModel playlistDetailViewModel // Inject the other needed VM
        )
        : base(viewModel) // Pass the page's ViewModel to ContentPageBase constructor
    private readonly IServiceProvider _serviceProvider;

    public PlaylistView(
        PlaylistListViewModel playlistListViewModel,
        PlaylistDetailViewModel playlistDetailViewModel,
        IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

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
            // Metóda SetColumn je statická metóda triedy Grid
            Grid.SetColumn(playlistListView, 0);
            Grid.SetColumn(playlistDetailView, 1);
            // Add children (same as before)
            // 3. Pridanie elementu do kolekcie detí Gridu
            mainGrid.Children.Add(playlistListView);
            mainGrid.Children.Add(playlistDetailView);
        }
        else
            Debug.WriteLine("Chyba: Root element nie je Grid alebo sa nenašiel Grid.");
        // Ak sa sem dostanete, znamená to, že sa nepodarilo získať referenciu na Grid
        // Skontrolujte XAML a ako získavate referenciu na Grid.
        System.Diagnostics.Debug.WriteLine("Chyba: Root element nie je Grid alebo sa nenašiel Grid.");
    }
}
