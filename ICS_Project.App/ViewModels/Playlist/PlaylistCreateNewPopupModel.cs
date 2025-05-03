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

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistCreateNewPopupModel : ObservableObject
    {
        private readonly IPlaylistFacade _facade;
        private readonly IMusicTrackFacade _musicTrackFacade;

        public ObservableCollection<MusicTrackListModel> MusicTracks { get; set; }

        [ObservableProperty]
        private PlaylistDetailModel _playlistDetail;

        [ObservableProperty]
        private ObservableCollection<MusicTrackListModel> _songs = [];

        public async Task InitializeAsync(Guid id)
        {
            PlaylistDetail = await _facade.GetAsync(id);
            await LoadSongsAsync();  // Now properly awaited after object creation
        }


        public PlaylistCreateNewPopupModel(IPlaylistFacade playlistFacade, IMusicTrackFacade musicTrackFacade)
        {
            _facade = playlistFacade;
            _musicTrackFacade = musicTrackFacade;
        }


        public async Task LoadSongsAsync()
        {
            // Fetch songs asynchronously
            var songs = await _musicTrackFacade.GetAsync();

            // Convert the result to an ObservableCollection
            Songs = songs.ToObservableCollection();

            // Print the contents of the Songs collection to the output for debugging
            Debug.WriteLine("Loaded Songs:");
            foreach (var song in Songs)
            {
                // Assuming each song has a 'Title' and 'Artist' or similar properties, you can adjust this to match the actual properties of your 'MusicTrackListModel'
                Debug.WriteLine($"- {song.Title}");
            }
        }


        [RelayCommand]
        public async Task CreateNewPlaylist()
        {
            var playlist = new PlaylistDetailModel()
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                Description = "Description",
                NumberOfMusicTracks = 20,
                TotalPlayTime = TimeSpan.Zero
            };

            var savedTrack = await _facade.SaveAsync(playlist);

            await InitializeAsync(PlaylistDetail.Id);
        }
    }
}