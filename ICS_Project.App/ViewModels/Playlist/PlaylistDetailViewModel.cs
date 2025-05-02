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

        [ObservableProperty]
        private PlaylistDetailModel _playlistDetail;

        public async Task InitializeAsync(Guid id)
        {
            PlaylistDetail = await _facade.GetAsync(id);
            //PlaylistDetail = new PlaylistDetailModel
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Album 1",
            //    Description = "Opis playlistu",
            //    NumberOfMusicTracks = 5,
            //    TotalPlayTime = new TimeSpan(1, 11, 0),
            //    MusicTracks = {
            //     new MusicTrackListModel { Title = "Musictrack 1", Description = "Je fajn", Length = new TimeSpan(0, 5, 0), Size = 50, UrlAddress = "https" },
            //     new MusicTrackListModel { Title = "Musictrack 2", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" },
            //     new MusicTrackListModel { Title = "Musictrack 3", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" },
            //     new MusicTrackListModel { Title = "Musictrack 4", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" },
            //     new MusicTrackListModel { Title = "Musictrack 5", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" },
            //     new MusicTrackListModel { Title = "Musictrack 6", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" }
            //    }
            //};
        }


        public PlaylistDetailViewModel(IPlaylistFacade playlistFacade)
        {
            _facade = playlistFacade;
        }

        //------------------
        //[ObservableProperty]
        //public partial PlaylistDetailModel playlistDetail { get; set; } = new PlaylistDetailModel
        //{
        //    Id = Guid.NewGuid(),
        //    Name = "Album 1",
        //    Description = "Opis playlistu",
        //    NumberOfMusicTracks = 5,
        //    TotalPlayTime = new TimeSpan(1, 11, 0),
        //    MusicTracks = {
        //         new MusicTrackListModel { Title = "Musictrack 1", Description = "Je fajn", Length = new TimeSpan(0, 5, 0), Size = 50, UrlAddress = "https" },
        //         new MusicTrackListModel { Title = "Musictrack 2", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" },
        //         new MusicTrackListModel { Title = "Musictrack 3", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" },
        //         new MusicTrackListModel { Title = "Musictrack 4", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" },
        //         new MusicTrackListModel { Title = "Musictrack 5", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" },
        //         new MusicTrackListModel { Title = "Musictrack 6", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" }
        //     }
        //};

        [RelayCommand]
        public async Task ModifyTrack()
        {
            //PlaylistDetail.MusicTracks.Add(new MusicTrackListModel { Title = "New track 6", Description = "Super track", Length = new TimeSpan(0, 6, 0), Size = 70, UrlAddress = "https" });
            var tmp = (await _facade.GetAsync()).ToObservableCollection();

            InitializeAsync(tmp.First().Id);
        }


    }
}
