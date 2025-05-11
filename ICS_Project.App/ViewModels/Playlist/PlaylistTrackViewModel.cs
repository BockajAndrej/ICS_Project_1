using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.BL.Models; // This will now primarily use MusicTrackListModel
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces;
using Microsoft.Maui.Controls; // For VisualElement
using System;

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistTrackViewModel : ViewModelBase
    {
        // Change the type of the model to MusicTrackListModel
        private readonly MusicTrackListModel _musicTrackModel;

        public string Title => _musicTrackModel.Title;
        // Format TimeSpan to string as desired, e.g., "mm:ss"
        public string Length => _musicTrackModel.Length.ToString(@"mm\:ss");
        public double Size => _musicTrackModel.Size;
        public Guid ID => _musicTrackModel.Id; // Id comes from ModelBase

        // Change constructor to accept MusicTrackListModel
        public PlaylistTrackViewModel(MusicTrackListModel musicTrackModel, IMessengerService messengerService)
          : base(messengerService)
        {
            _musicTrackModel = musicTrackModel ?? throw new ArgumentNullException(nameof(musicTrackModel));
        }

        [RelayCommand]
        private void MusicTrackShowOptionsMessage(object? parameter) // Parameter can be the button itself
        {
            var anchor = parameter as VisualElement;

            // Send a SPECIFIC message for track options, including THIS ViewModel instance
            // Use the Messenger property provided by ViewModelBase (via ObservableRecipient)
            this.Messenger.Send(new MusicTrackShowOptions(anchor, this));
        }

        [RelayCommand]
        private void MusicTrackShowDetailMessage(object? parameter) // Parameter can be the button itself
        {
            var anchor = parameter as VisualElement;

            // Send a SPECIFIC message for track options, including THIS ViewModel instance
            // Use the Messenger property provided by ViewModelBase (via ObservableRecipient)
            this.Messenger.Send(new MusicTrackShowDetail(anchor, this));
        }


    }
}