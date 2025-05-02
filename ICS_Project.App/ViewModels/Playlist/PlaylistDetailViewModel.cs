using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistDetailViewModel : ObservableObject
    {
        private readonly IPlaylistFacade _facade;
        private readonly IMusicTrackFacade _musicTrackFacade;

        [ObservableProperty]
        private PlaylistDetailModel _playlistDetail;

        public async Task InitializeAsync(Guid id)
        {
            PlaylistDetail = await _facade.GetAsync(id);
        }


        public PlaylistDetailViewModel(IPlaylistFacade playlistFacade, IMusicTrackFacade musicTrackFacade)
        {
            _facade = playlistFacade;
            _musicTrackFacade = musicTrackFacade;
        }

        [RelayCommand]
        public async Task ModifyTrack()
        {
            var tmp = (await _facade.GetAsync()).ToObservableCollection();

            await InitializeAsync(tmp.First().Id);
        }

        [RelayCommand]
        public async Task AddMusicTrackToPlaylist()
        {
            var musictrack = new MusicTrackDetailModel()
            {
                Id = Guid.NewGuid(),
                Title = "Titulok",
                Description = "Opis",
                Length = TimeSpan.FromSeconds(200),
                Size = 20,
                UrlAddress = "https://www.google.com",
            };

            var savedTrack = await _musicTrackFacade.SaveAsync(musictrack);

            await _facade.AddMusicTrackToPlaylistAsync(PlaylistDetail.Id, savedTrack.Id);

            await InitializeAsync(PlaylistDetail.Id);
        }
    }
}
