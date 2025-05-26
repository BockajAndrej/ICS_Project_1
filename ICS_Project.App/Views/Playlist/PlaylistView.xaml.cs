using ICS_Project.App.ViewModels;
using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.Views;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace ICS_Project.App.Views.Playlist;


public partial class PlaylistView : ContentPageBase
{
    private readonly PlaylistDetailViewModel _playlistDetailViewModel;

    private readonly IServiceProvider _serviceProvider;

    public PlaylistView(
        PlaylistListViewModel viewModel,
        PlaylistDetailViewModel playlistDetailViewModel,
        IServiceProvider serviceProvider
        )
        : base(viewModel)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        _playlistDetailViewModel = playlistDetailViewModel;

        Grid mainGrid = this.Content as Grid;

        if (mainGrid != null)
        {
            var playlistListView = new PlaylistListView(viewModel, serviceProvider);

            var playlistDetailView = new PlaylistDetailView(_playlistDetailViewModel);

            Grid.SetColumn(playlistListView, 0);
            Grid.SetColumn(playlistDetailView, 1);

            mainGrid.Children.Add(playlistListView);
            mainGrid.Children.Add(playlistDetailView);
        }
        else
            Debug.WriteLine("Chyba: Root element nie je Grid alebo sa nenašiel Grid.");
    }
}
