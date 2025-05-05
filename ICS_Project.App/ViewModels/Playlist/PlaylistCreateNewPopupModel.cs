using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
using ICS_Project.App.Messages;
using System.ComponentModel;

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistCreateNewPopupModel : ObservableObject
    {
        private readonly IPlaylistFacade _facade;
        private readonly IMusicTrackFacade _musicTrackFacade;


        [ObservableProperty]
        private PlaylistDetailModel _playlistDetail;

        [ObservableProperty]
        private ObservableCollection<MusicTrackListModel> _musicTracks = [];

        [ObservableProperty] 
        private string _name;

        [ObservableProperty] 
        private string _description;

        [ObservableProperty] 
        private int _numberOfTracks;

        [ObservableProperty] 
        private TimeSpan _totalTrackTime;

        private readonly List<MusicTrackListModel> _selectedTracks = new();

        private HashSet<Guid> _originalTrackIds = new();
        private HashSet<Guid> _selectedTrackIds = new();

        private bool _isRegistered = false;
        private bool _isNotRegistered = true;



        public async Task InitializeAsync(Guid id)
        {
            PlaylistDetail = await _facade.GetAsync(id);
            await LoadSongsAsync();  // Now properly awaited after object creation
        }


        public PlaylistCreateNewPopupModel(IPlaylistFacade playlistFacade, IMusicTrackFacade musicTrackFacade)
        {
            _facade = playlistFacade;
            _musicTrackFacade = musicTrackFacade;
            NumberOfTracks = 0;
            TotalTrackTime = TimeSpan.Zero;

            PlaylistDetail = new PlaylistDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "",
                Description = "",
                TotalPlayTime = TimeSpan.Zero,
                NumberOfMusicTracks = 0,
                MusicTracks = new ObservableCollection<MusicTrackListModel>(),
            };

            WeakReferenceMessenger.Default.UnregisterAll(this);
            // Register to listen for PlaylistPopupContextMessage
            if (!_isRegistered)
            {
                _isRegistered = true;
                WeakReferenceMessenger.Default.Register<PlaylistPopupContext>(this, async (r, m) =>
                {
                    Debug.WriteLine("PlaylistPopupContextMessage received in PlaylistCreateNewPopupModel.");

                    var context = m.IsEditMode;

                    if (context)
                    {
                        Debug.WriteLine("Edit mode — loading existing playlist.");
                        ListenForGUID();
                    }
                    else
                    {
                        Debug.WriteLine("Create mode — initializing new playlist.");
                        await LoadSongsAsync();
                    }
                });
            }

        }


        public async Task LoadSongsAsync()
        {
            // Fetch songs asynchronously
            var tracks = await _musicTrackFacade.GetAsync();

            // Convert the result to an ObservableCollection
            MusicTracks = tracks.ToObservableCollection();

            // Print the contents of the Songs collection to the output for debugging
            // Debug.WriteLine("Loaded Songs:");
            // foreach (var track in MusicTracks)
            // {
            //     // Assuming each song has a 'Title' and 'Artist' or similar properties, you can adjust this to match the actual properties of your 'MusicTrackListModel'
            //     Debug.WriteLine($"- {track.Title}");
            // }

            foreach (var track in MusicTracks)
            {
                track.PropertyChanged += Track_PropertyChanged;
            }
        }

        // Command for saving the playlist
        [RelayCommand]
        public async void SaveChanges()
        {
            if (Name != null)
            {
                PlaylistDetail.Name = Name;
                PlaylistDetail.Description = Description;

                // Get the current track IDs in the playlist
                var currentTrackIds = PlaylistDetail.MusicTracks.Select(t => t.Id).ToHashSet();

                // Tracks that were selected but are not in the current playlist
                var tracksToAdd = _selectedTracks.Where(track => !currentTrackIds.Contains(track.Id)).ToList();

                // Tracks that are in the playlist but were deselected
                var tracksToRemove = PlaylistDetail.MusicTracks.Where(track => !_selectedTracks.Any(t => t.Id == track.Id)).ToList();

                // Remove tracks that are not selected
                foreach (var track in tracksToRemove)
                {
                    await _facade.RemoveMusicTrackFromPlaylistAsync(PlaylistDetail.Id, track.Id);
                }

                // Add tracks that are selected but not in the playlist
                foreach (var track in tracksToAdd)
                {
                    await _facade.AddMusicTrackToPlaylistAsync(PlaylistDetail.Id, track.Id);
                }
            }

            WeakReferenceMessenger.Default.Send(new PlaylistNewPlaylistClosed());
        }

        // Command for reverting the changes
        [RelayCommand]
        public void RevertChanges()
        {
            Debug.WriteLine("Revert button pressed");
        }

        private void Track_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MusicTrackListModel.IsSelected) && sender is MusicTrackListModel track)
            {
                if (track.IsSelected)
                {
                    if (_selectedTrackIds.Add(track.Id))
                    {
                        _selectedTracks.Add(track);
                        NumberOfTracks++;
                        TotalTrackTime += track.Length;
                    }
                }
                else
                {
                    if (_selectedTrackIds.Remove(track.Id))
                    {
                        _selectedTracks.RemoveAll(t => t.Id == track.Id);
                        NumberOfTracks--;
                        TotalTrackTime -= track.Length;
                    }
                }
            }
        }

        private void ListenForGUID()
        {
            if (_isNotRegistered)
            {
                WeakReferenceMessenger.Default.Register<PlaylistEditGUID>(this, async (recipient, message) =>
                {
                    Debug.WriteLine($"Received PlaylistEditGUID with ID: {message.ID}");

                    // Fetch the playlist detail using the provided GUID
                    PlaylistDetail = await _facade.GetAsync(message.ID);

                    // Optional: Refresh the music tracks and their selection
                    await LoadSongsAsync();

                    // Pre-select tracks already in the playlist
                    foreach (var track in PlaylistDetail.MusicTracks)
                    {
                        _originalTrackIds.Add(track.Id);

                        var match = MusicTracks.FirstOrDefault(t => t.Id == track.Id);
                        if (match != null)
                        {
                            match.IsSelected = true;
                        }
                    }

                    Name = PlaylistDetail.Name;
                    Description = PlaylistDetail.Description;
                });
                _isNotRegistered = false;
            }
        }
    }
}