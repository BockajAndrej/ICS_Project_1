using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging; // Needed for IMessenger and messages
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces; // Needed for your custom message


namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistDetailViewModel : ViewModelBase
    {
        private readonly IPlaylistFacade _facade;
        private readonly IMessenger _messenger; // Inject the messenger

        [ObservableProperty]
        private PlaylistDetailModel _playlistDetail;

        [ObservableProperty]
        private ObservableCollection<MusicTrackListModel> _musicTracks;

        [ObservableProperty]
        private string _searchTracks;

        public async Task InitializeAsync(Guid id)
        {
            PlaylistDetail = await _facade.GetAsync(id);
            MusicTracks = new ObservableCollection<MusicTrackListModel>(PlaylistDetail.MusicTracks);
        }

        [RelayCommand]
        private async Task SearchSongs(string inputText)
        {
            _searchTracks = inputText;
            MusicTracks.Clear();
            Filter();
        }

        private void Filter()
        {
            Debug.WriteLine($"Searching for {SearchTracks}");
            if (string.IsNullOrWhiteSpace(SearchTracks))
            {
                Debug.WriteLine("Searchbar text is empty");
                MusicTracks = new ObservableCollection<MusicTrackListModel>(PlaylistDetail.MusicTracks);
            }
            else
            {
                Debug.WriteLine("SearchbarText is NOT empty");
                var lower = SearchTracks.ToLowerInvariant();
                var filtered = PlaylistDetail.MusicTracks.ToList()
                    .Where(track =>
                        !string.IsNullOrWhiteSpace(track.Title) && track.Title.ToLowerInvariant().Contains(lower))
                    .ToList();
                MusicTracks = new ObservableCollection<MusicTrackListModel>(filtered);
            }
        }


        // Constructor matching the base class
        public PlaylistDetailViewModel(
            IPlaylistFacade playlistFacade,
            IMessengerService messengerService) // Required by ViewModelBase
            : base(messengerService) // Pass messengerService to base
        {
            _facade = playlistFacade;
            _messenger = messengerService.Messenger; // Assign the messenger
            // _messengerService is available via the base class property 'MessengerService'
            ListenToGUIDRequest();
        }

        // Command now accepts a parameter
        [RelayCommand]
        private void ShowOptions(object? parameter)
        {
            Debug.WriteLine("--- ShowOptions Command Executed ---"); // Use Debug.WriteLine
            // Ensure the parameter is a VisualElement (the Button)
            var anchor = parameter as VisualElement;

            // Send the message with the anchor and this ViewModel instance
            _messenger.Send(new PlaylistShowOptions(anchor, this));
        }

        // Dummy LoadDataAsync override (if needed by base logic)
        protected override async Task LoadDataAsync()
        {
            // TODO: Implement logic to load PlaylistDetail data
            // e.g., PlaylistDetail = await _playlistFacade.GetAsync(someId);
            await Task.CompletedTask; // Placeholder
        }

        [RelayCommand]
        public async Task ModifyTrack()
        {
            var tmp = (await _facade.GetAsync()).ToObservableCollection();

            await InitializeAsync(tmp.First().Id);
        }
        public void ListenToGUIDRequest()
        {
            WeakReferenceMessenger.Default.Register<PlaylistRequestGUID>(this, (recipient, message) =>
            {
                var id = PlaylistDetail.Id;

                Debug.WriteLine($"[ListenToGUIDRequest] Received request, sending back GUID: {id}");

                WeakReferenceMessenger.Default.Send(new PlaylistEditGUID{ID = id});
            });
        }
    }
}
