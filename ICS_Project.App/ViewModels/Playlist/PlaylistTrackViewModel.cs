using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces;
using ICS_Project.BL.Models;

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistTrackViewModel : ViewModelBase
    {
        private readonly MusicTrackListModel _musicTrackModel;

        public string Title => _musicTrackModel.Title;
        public string Length => _musicTrackModel.Length.ToString(@"mm\:ss");
        public double Size => _musicTrackModel.Size;
        public Guid ID => _musicTrackModel.Id;

        public PlaylistTrackViewModel(MusicTrackListModel musicTrackModel, IMessengerService messengerService)
          : base(messengerService)
        {
            _musicTrackModel = musicTrackModel ?? throw new ArgumentNullException(nameof(musicTrackModel));
        }

        [RelayCommand]
        private void MusicTrackShowOptionsMessage(object? parameter) 
        {
            var anchor = parameter as VisualElement;

            // Send a SPECIFIC message for track options, including THIS ViewModel instance
            this.Messenger.Send(new MusicTrackShowOptions(anchor, this));
        }

        [RelayCommand]
        private void MusicTrackShowDetailMessage(object? parameter)
        {
            var anchor = parameter as VisualElement;

            // Send a SPECIFIC message for track options, including THIS ViewModel instance
            this.Messenger.Send(new MusicTrackShowDetail(anchor, this));
        }


    }
}