using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces;
using ICS_Project.App.ViewModels.MusicTrack;
using ICS_Project.App.Views.MusicTrack.Popups;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistDetailViewModel : ViewModelBase
    {
        private readonly IPlaylistFacade _playlistFacade;
        private readonly IPopupService _popupService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private PlaylistDetailModel? _playlistDetail;

        [ObservableProperty]
        private string _searchTracks;

        public bool IsPlaylistAvailable => PlaylistDetail != null && PlaylistDetail.Id != Guid.Empty;


        partial void OnPlaylistDetailChanged(PlaylistDetailModel value)
        {
            OnPropertyChanged(nameof(IsPlaylistAvailable));
        }

        [ObservableProperty]
        private ObservableCollection<PlaylistTrackViewModel> _musicTrackVMs = new();

        public Guid CurrentPlaylistId { get; private set; }

        public async Task InitializeAsync(Guid id)
        {
            await SetCurrentPlaylistAndLoadAsync(id);
        }

        [RelayCommand]
        private async Task SearchSongs(string inputText)
        {

            _searchTracks = inputText;
            Filter();
        }

        private void Filter()
        {
            Debug.WriteLine($"Filtering tracks with search text: '{SearchTracks}'");

            if (PlaylistDetail == null || PlaylistDetail.MusicTracks == null)
            {
                Debug.WriteLine("Filter: PlaylistDetail or PlaylistDetail.MusicTracks is null. Clearing VMs and exiting.");
                MusicTrackVMs.Clear();
                return;
            }

            MusicTrackVMs.Clear();

            if (string.IsNullOrWhiteSpace(SearchTracks))
            {
                Debug.WriteLine("Searchbar text is empty. Reloading all tracks.");
                foreach (var trackModel in PlaylistDetail.MusicTracks)
                {
                    if (trackModel != null)
                    {
                        MusicTrackVMs.Add(new PlaylistTrackViewModel(trackModel, MessengerService));
                    }
                }
            }
            else
            {
                Debug.WriteLine("SearchbarText is NOT empty. Filtering tracks.");
                var lower = SearchTracks.ToLowerInvariant();
                var filtered = PlaylistDetail.MusicTracks
                    .Where(track =>
                        track != null &&
                        !string.IsNullOrWhiteSpace(track.Title) &&
                        track.Title.ToLowerInvariant().Contains(lower))
                    .ToList();

                foreach (var trackModel in filtered)
                {
                    MusicTrackVMs.Add(new PlaylistTrackViewModel(trackModel, MessengerService));
                }
            }
        }



        public PlaylistDetailViewModel(
                IPlaylistFacade playlistFacade,
                IMessengerService messengerService,
                IPopupService popupService,
                IServiceProvider serviceProvider)
                : base(messengerService)
        {
            _playlistFacade = playlistFacade;
            _popupService = popupService;
            _serviceProvider = serviceProvider;

            // Register message handlers
            Messenger.Register<MusicTrackShowOptions>(this, HandleMusicTrackShowOptions);
            Messenger.Register<MusicTrackShowDetail>(this, HandleMusicTrackDetailOptionsAsync);
            ListenToGUIDRequest();

            ListenToPlaylistSelect();

            ListenToMusicTracksDelete();
            ListenToMusicTracksUpdate();
        }

        public async Task SetCurrentPlaylistAndLoadAsync(Guid playlistId)
        {
            if (playlistId == Guid.Empty)
            {
                Debug.WriteLine("SetCurrentPlaylistAndLoadAsync: Playlist ID is empty. Clearing data.");
                CurrentPlaylistId = Guid.Empty;
                PlaylistDetail = null;
                MusicTrackVMs.Clear();
                return;
            }

            CurrentPlaylistId = playlistId;
            Debug.WriteLine($"SetCurrentPlaylistAndLoadAsync: Set CurrentPlaylistId to {CurrentPlaylistId}. Forcing data refresh.");
            await LoadDataAsync();
        }


        private void HandleMusicTrackShowOptions(object recipient, MusicTrackShowOptions message)
        {
            if (message.ViewModel == null)
            {
                Debug.WriteLine("PlaylistDetailViewModel: MusicTrackShowOptions message received with null ViewModel.");
                return;
            }

            // Resolve the ViewModel for the popup
            // This ensures the popup's VM gets its dependencies injected correctly.
            var musicTrackEditViewModel = _serviceProvider.GetService<MusicTrackEditViewModel>();
            if (musicTrackEditViewModel == null)
            {
                Debug.WriteLine("PlaylistDetailViewModel: Failed to resolve MusicTrackEditViewModel.");
                return;
            }

            // Initialize the ViewModel with data from the track that was clicked
            musicTrackEditViewModel.Initialize(message.ViewModel.ID, message.ViewModel.Title);


            Debug.WriteLine($"PlaylistDetailViewModel: Requesting PopupService to show MusicTrackEditView for track: {message.ViewModel.Title}");
            _popupService.ShowPopup(
                typeof(MusicTrackEditView),
                musicTrackEditViewModel,
                message.Anchor
            );
        }        
        private async void HandleMusicTrackDetailOptionsAsync(object recipient, MusicTrackShowDetail message)
        {
            if (message.ViewModel == null)
            {
                Debug.WriteLine("PlaylistDetailViewModel: MusicTrackShowDetail message received with null ViewModel."); 
                return;
            }

            Guid trackIdToShow = message.ViewModel.ID;
            // Resolve the ViewModel for the popup
            // This ensures the popup's VM gets its dependencies injected correctly.
            var musicTrackDetailViewModel = _serviceProvider.GetService<MusicTrackDetailViewModel>();
            if (musicTrackDetailViewModel == null)
            {
                Debug.WriteLine("PlaylistDetailViewModel: Failed to resolve MusicTrackEditViewModel.");
                return;
            }

            Debug.WriteLine($"PlaylistDetailViewModel: Calling LoadTrackAsync on MusicTrackDetailViewModel for ID: {trackIdToShow}");
            await musicTrackDetailViewModel.LoadTrackAsync(trackIdToShow);


            // Now show the popup with the ViewModel that *has* loaded the data
            Debug.WriteLine($"PlaylistDetailViewModel: Requesting PopupService to show MusicTrackDetailView for track: {message.ViewModel.Title}");
            _popupService.ShowPopup(
                typeof(MusicTrackDetailView),
                musicTrackDetailViewModel,
                message.Anchor
            );
        }

        [RelayCommand]
        private void ShowOptions(object? parameter)
        {
            Debug.WriteLine("--- ShowOptions Command Executed ---");
            var anchor = parameter as VisualElement;

            // Send the message with the anchor and this ViewModel instance
            MessengerService.Send(new PlaylistShowOptions(anchor, this));
        }

        protected override async Task LoadDataAsync()
        {
            Debug.WriteLine($"LoadDataAsync started for CurrentPlaylistId: {CurrentPlaylistId}");
            if (CurrentPlaylistId == Guid.Empty)
            {
                Debug.WriteLine("LoadDataAsync: CurrentPlaylistId is empty. Clearing data.");
                // Ensure properties are cleared if ID is invalid
                PlaylistDetail = null;
                MusicTrackVMs.Clear();
                return;
            }

            try
            {
                // Use the generated public property for assignment
                PlaylistDetail = await _playlistFacade.GetAsync(CurrentPlaylistId);
                Debug.WriteLine($"LoadDataAsync: Fetched PlaylistHeaderDetails. Name: {PlaylistDetail?.Name ?? "null"}");

                // Use the generated public property
                MusicTrackVMs.Clear();
                if (PlaylistDetail?.MusicTracks != null)
                {
                    Debug.WriteLine($"LoadDataAsync: Found {PlaylistDetail.MusicTracks.Count} tracks.");
                    foreach (var trackModel in PlaylistDetail.MusicTracks)
                    {
                        MusicTrackVMs.Add(new PlaylistTrackViewModel(trackModel, MessengerService));
                    }
                }
                else
                {
                    Debug.WriteLine("LoadDataAsync: PlaylistHeaderDetails.MusicTracks is null or empty.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDataAsync: Exception occurred: {ex.Message}");
                PlaylistDetail = null;
                MusicTrackVMs.Clear();
            }
        }

        [RelayCommand]
        public async Task ModifyTrack()
        {
            Debug.WriteLine("ModifyTrack command started.");
            try
            {
                var playlists = await _playlistFacade.GetAsync();
                if (playlists != null && playlists.Any())
                {
                    var firstPlaylistId = playlists.First().Id;
                    Debug.WriteLine($"ModifyTrack: Found first playlist with ID: {firstPlaylistId}. Calling SetCurrentPlaylistAndLoadAsync.");
                    await SetCurrentPlaylistAndLoadAsync(firstPlaylistId);
                }
                else
                {
                    Debug.WriteLine("ModifyTrack: No playlists found to load.");
                    await SetCurrentPlaylistAndLoadAsync(Guid.Empty);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ModifyTrack: Exception occurred: {ex.Message}");
            }
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
        public void ListenToPlaylistSelect()
        {
            WeakReferenceMessenger.Default.Register<PlaylistSelectedMessage>(this, async (r, m) =>
            {
                Debug.WriteLine($"[PlaylistDetailViewModel] Received Playlist ID: {m.Value}");
                await InitializeAsync(m.Value);
            });
        }


        [RelayCommand]
        private void AddMusicTrackToPlaylist()
        {
            Debug.WriteLine("AddMusicTrackToPlaylistCommand executed.");
        }

        [RelayCommand]
        private async Task DeletePlaylistAsync()
        {
            if (PlaylistDetail == null || PlaylistDetail.Id == Guid.Empty)
            {
                Debug.WriteLine("[DeletePlaylistAsync] No playlist to delete.");
                return;
            }

            Debug.WriteLine($"[DeletePlaylistAsync] Trying to delete playlist with ID: {PlaylistDetail.Id}");

            try
            {
                await _playlistFacade.DeleteAsync(PlaylistDetail.Id);
                Debug.WriteLine($"[DeletePlaylistAsync] Playlist with ID: {PlaylistDetail.Id} was succsefully removed.");

                WeakReferenceMessenger.Default.Send(new PlaylistDeletedMessage(PlaylistDetail.Id));

                PlaylistDetail = PlaylistDetailModel.CreateEmpty();
                MusicTrackVMs.Clear();


            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DeletePlaylistAsync] Error when trying to delete playlist: {ex.Message}");
            }
        }

        private void ListenToMusicTracksDelete()
        {
            MessengerService.Messenger.Register<MusicTrackDeletedMessage>(this, async (r, m) =>
            {
                Debug.WriteLine($"[PlaylistDetailViewModel] received Message about deleting musicTrack with ID: {m.Value}. Refreshing list.");
                await SetCurrentPlaylistAndLoadAsync(CurrentPlaylistId);
            });
        }

        private void ListenToMusicTracksUpdate()
        {
            MessengerService.Messenger.Register<MusicTrackUpdatedMessage>(this, async (r, m) =>
            {
                Debug.WriteLine($"[PlaylistDetailViewModel] received Message about updating musicTrack with ID: {m.Value}. Refreshing list.");
                await SetCurrentPlaylistAndLoadAsync(CurrentPlaylistId);
            });
        }
    }
}
