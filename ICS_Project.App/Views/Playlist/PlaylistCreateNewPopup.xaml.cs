using CommunityToolkit.Maui.Views;
using ICS_Project.App.ViewModels.Playlist;

namespace ICS_Project.App.Views.Playlist
{
    public partial class PlaylistCreateNewPopup : Popup
    {
        public PlaylistCreateNewPopup(PlaylistCreateNewPopupModel playlistCreateNewPopupModel)
        {
            InitializeComponent();
            BindingContext = playlistCreateNewPopupModel;

            // Subscribe to the OnOpened event
            this.Opened += PlaylistCreateNewPopup_Opened;
        }

        // Handle the OnOpened event to load playlists
        private async void PlaylistCreateNewPopup_Opened(object sender, EventArgs e)
        {
            if (BindingContext is PlaylistCreateNewPopupModel viewModel)
            {
                await viewModel.LoadSongsAsync(); // Load songs when the popup opens
            }
        }
    }
}