using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.App.ViewModels.Playlist;

namespace ICS_Project.App.Views.Playlist
{
    public partial class PlaylistCreateNewPopup : Popup
    {   
        public PlaylistCreateNewPopup(PlaylistCreateNewPopupModel playlistCreateNewPopupModel)
        {
            InitializeComponent();
            BindingContext = playlistCreateNewPopupModel;
        }

        private void PlaylistCreateNewPopup_Opened(object sender, EventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new PopupOpenedMessage(true));
        }
    }
}