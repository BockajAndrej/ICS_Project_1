using ICS_Project.App.ViewModels.Playlist;

namespace ICS_Project.App.Views.Playlist;

public partial class PlaylistView : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    public PlaylistView(
        PlaylistListViewModel playlistListViewModel,
        PlaylistDetailViewModel playlistDetailViewModel,
        IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        // Attempt to find the Grid element in the page content
        Grid mainGrid = this.Content as Grid;

        if (mainGrid != null)
        {
            // 1. Vytvorenie inštancie UI elementu
            // Použite správny názov triedy, napr. PlaylistListViewControl
            var playlistListView = new PlaylistListView(playlistListViewModel, _serviceProvider);
            var playlistDetailView = new PlaylistDetailView(playlistDetailViewModel);

            // 2. Nastavenie pripojenej vlastnosti Grid.Column
            // Metóda SetColumn je statická metóda triedy Grid
            Grid.SetColumn(playlistListView, 0);
            Grid.SetColumn(playlistDetailView, 1);

            // 3. Pridanie elementu do kolekcie detí Gridu
            mainGrid.Children.Add(playlistListView);
            mainGrid.Children.Add(playlistDetailView);
        }
        else
        {
            // Ak sa sem dostanete, znamená to, že sa nepodarilo získať referenciu na Grid
            // Skontrolujte XAML a ako získavate referenciu na Grid.
            System.Diagnostics.Debug.WriteLine("Chyba: Root element nie je Grid alebo sa nenašiel Grid.");
        }
    }
}