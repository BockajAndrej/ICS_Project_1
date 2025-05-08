using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ICS_Project.App.Views.MusicTrack.Popups; // For typeof(MusicTrackEditView)
using ICS_Project.App.ViewModels.MusicTrack;   // For MusicTrackEditViewModel
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; // For IServiceProvider (if still needed for VM resolution)
using CommunityToolkit.Mvvm.Messaging; // Needed for IMessenger and messages
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces; // Needed for your custom message


namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistDetailViewModel : ViewModelBase
    {
        private readonly IPlaylistFacade _playlistFacade;
        private readonly IPopupService _popupService; // <<-- ADD THIS
        private readonly IServiceProvider _serviceProvider; // <<-- ADD THIS (to resolve VM for popup)

        [ObservableProperty]
        private PlaylistDetailModel? _playlistHeaderDetails; // For playlist name, description etc.

        [ObservableProperty]
        private ObservableCollection<PlaylistTrackViewModel> _musicTrackVMs = new();

        public Guid CurrentPlaylistId { get; private set; }

        public async Task InitializeAsync(Guid id)
        {
            await SetCurrentPlaylistAndLoadAsync(id);
            //CurrentPlaylistId = id;
            //_playlistHeaderDetails = await _playlistFacade.GetAsync(id);

        }


        // Constructor matching the base class
        public PlaylistDetailViewModel(
                IPlaylistFacade playlistFacade,
                IMessengerService messengerService,
                IPopupService popupService,          // <<-- ADD THIS
                IServiceProvider serviceProvider)    // <<-- ADD THIS
                : base(messengerService)
        {
            _playlistFacade = playlistFacade;
            _popupService = popupService;        // <<-- ADD THIS
            _serviceProvider = serviceProvider;  // <<-- ADD THIS

            // Register message handlers
            // The base class (ObservableRecipient) handles IsActive,
            // so messages are received when IsActive is true.
            Messenger.Register<MusicTrackShowOptions>(this, HandleMusicTrackShowOptions);
            Messenger.Register<MusicTrackShowDetail>(this, HandleMusicTrackDetailOptionsAsync);
            ListenToGUIDRequest();
        }

        // Call this when you know which playlist ID to load
        public async Task SetCurrentPlaylistAndLoadAsync(Guid playlistId)
        {
            if (playlistId == Guid.Empty)
            {
                Debug.WriteLine("SetCurrentPlaylistAndLoadAsync: Playlist ID is empty. Clearing data.");
                CurrentPlaylistId = Guid.Empty;
                PlaylistHeaderDetails = null; // Use the generated public property
                MusicTrackVMs.Clear();        // Use the generated public property
                return;
            }

            CurrentPlaylistId = playlistId;
            Debug.WriteLine($"SetCurrentPlaylistAndLoadAsync: Set CurrentPlaylistId to {CurrentPlaylistId}. Forcing data refresh.");
            // ForceDataRefreshOnNextAppearing(); // Use this if loading happens via OnAppearing
            await LoadDataAsync(); // Or call LoadDataAsync directly if you want immediate load
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
                musicTrackEditViewModel, // Pass the resolved and initialized ViewModel
                message.Anchor           // Pass the anchor element (button)
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

            // Initialize the ViewModel with data from the track that was clicked
            //musicTrackDetailViewModel.Initialize(message.ViewModel.ID, message.ViewModel.Title);

            Debug.WriteLine($"PlaylistDetailViewModel: Calling LoadTrackAsync on MusicTrackDetailViewModel for ID: {trackIdToShow}");
            await musicTrackDetailViewModel.LoadTrackAsync(trackIdToShow); // <--- CALL THE LOAD METHOD


            // Now show the popup with the ViewModel that *has* loaded the data
            Debug.WriteLine($"PlaylistDetailViewModel: Requesting PopupService to show MusicTrackDetailView for track: {message.ViewModel.Title}"); // Using original title for log
            _popupService.ShowPopup(
                typeof(MusicTrackDetailView), // Use the correct Type
                musicTrackDetailViewModel, // Pass the resolved and *loaded* ViewModel
                message.Anchor             // Pass the anchor element
            );
        }

        // Command now accepts a parameter
        [RelayCommand]
        private void ShowOptions(object? parameter)
        {
            Debug.WriteLine("--- ShowOptions Command Executed ---"); // Use Debug.WriteLine
            // Ensure the parameter is a VisualElement (the Button)
            var anchor = parameter as VisualElement;

            // Send the message with the anchor and this ViewModel instance
            MessengerService.Send(new PlaylistShowOptions(anchor, this));
        }


        // This is your primary data loading method
        protected override async Task LoadDataAsync()
        {
            Debug.WriteLine($"LoadDataAsync started for CurrentPlaylistId: {CurrentPlaylistId}");
            if (CurrentPlaylistId == Guid.Empty)
            {
                Debug.WriteLine("LoadDataAsync: CurrentPlaylistId is empty. Clearing data.");
                // Ensure properties are cleared if ID is invalid
                PlaylistHeaderDetails = null;
                MusicTrackVMs.Clear();
                return;
            }

            try
            {
                // Use the generated public property for assignment
                PlaylistHeaderDetails = await _playlistFacade.GetAsync(CurrentPlaylistId);
                Debug.WriteLine($"LoadDataAsync: Fetched PlaylistHeaderDetails. Name: {PlaylistHeaderDetails?.Name ?? "null"}");

                // Use the generated public property
                MusicTrackVMs.Clear();
                if (PlaylistHeaderDetails?.MusicTracks != null)
                {
                    Debug.WriteLine($"LoadDataAsync: Found {PlaylistHeaderDetails.MusicTracks.Count} tracks.");
                    foreach (var trackModel in PlaylistHeaderDetails.MusicTracks)
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
                // Handle error appropriately, maybe clear data or show a message
                PlaylistHeaderDetails = null;
                MusicTrackVMs.Clear();
            }
            // await base.LoadDataAsync(); // Call if base class has meaningful implementation
        }

        [RelayCommand]
        public async Task ModifyTrack()
        {
            Debug.WriteLine("ModifyTrack command started.");
            try
            {
                var playlists = await _playlistFacade.GetAsync(); // This gets ListModels
                if (playlists != null && playlists.Any())
                {
                    var firstPlaylistId = playlists.First().Id;
                    Debug.WriteLine($"ModifyTrack: Found first playlist with ID: {firstPlaylistId}. Calling SetCurrentPlaylistAndLoadAsync.");
                    await SetCurrentPlaylistAndLoadAsync(firstPlaylistId);
                }
                else
                {
                    Debug.WriteLine("ModifyTrack: No playlists found to load.");
                    // Optionally clear the current view if no playlists exist
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
                var id = _playlistHeaderDetails.Id;

                Debug.WriteLine($"[ListenToGUIDRequest] Received request, sending back GUID: {id}");

                WeakReferenceMessenger.Default.Send(new PlaylistEditGUID{ID = id});
            });
        }
    }
}
