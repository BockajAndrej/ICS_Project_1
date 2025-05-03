using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Models;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.ObjectModel;

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistListViewModel : ObservableObject
    {
        private readonly IPlaylistFacade _facade;
        string timestamp;

        [ObservableProperty]
        private ObservableCollection<PlaylistListModel> _playlists;

        [ObservableProperty]
        private string _searchPlaylist;


        [RelayCommand]
        private async Task SearchPlaylists()
        {
            Playlists.Clear();
            Playlists = (await _facade.GetAsync(SearchPlaylist)).ToObservableCollection();
        }

        [RelayCommand]
        public async Task LoadAllPlaylistsAsync()
        {
            Playlists = (await _facade.GetAsync()).ToObservableCollection();
        }

        public PlaylistListViewModel(IPlaylistFacade playlistFacade)
        {
            _facade = playlistFacade;
            SearchPlaylist = string.Empty;
            LoadAllPlaylistsAsync();
        }

        [RelayCommand]
        public async Task AddAlbum()
        {
            timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");

            var detailModelToCreate = new PlaylistDetailModel
            {
                Id = Guid.NewGuid(),
                Name = $"Name: {timestamp}",
                Description = "Description for new playlist",
                NumberOfMusicTracks = 0,
                TotalPlayTime = TimeSpan.Zero,
            };

            try
            {
                await _facade.SaveAsync(detailModelToCreate);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
