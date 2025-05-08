using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Models;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging; // Needed for IMessenger and messages
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces; // Needed for your custom message
using System.Diagnostics; // For debug writeline

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistListViewModel : ViewModelBase
    {
        private readonly IPlaylistFacade _facade;
        string timestamp;

        [ObservableProperty]
        private ObservableCollection<PlaylistListModel> _playlists;

        [ObservableProperty]
        private string _searchPlaylist;

        
        [RelayCommand]
        private void PlaylistTapped(PlaylistListModel? playlist) // Parameter je vybraný playlist
        {
            if (playlist != null)
            {
                Debug.WriteLine($"Click through Command in ViewModel: Playlist '{playlist.Name}', ID: {playlist.Id}");

                MessengerService.Messenger.Send(new PlaylistSelectedMessage(playlist.Id));
            }
            else
            {
                Debug.WriteLine("PlaylistTapped was called with null param.");
            }
        }


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

        public PlaylistListViewModel(
          IPlaylistFacade playlistFacade,
          IMessengerService messengerService) // Needs IMessengerService
          : base(messengerService) // Pass to base constructor
        {
            _facade = playlistFacade;
            SearchPlaylist = string.Empty;
            LoadAllPlaylistsAsync();


            ReloadPlaylistsAfterDeletion();

        }

        private void ReloadPlaylistsAfterDeletion()
        {
            MessengerService.Messenger.Register<PlaylistDeletedMessage>(this, async (r, m) =>
            {
                Debug.WriteLine($"[PlaylistListViewModel] received Message about deleting playlist with ID: {m.Value}. Refreshing list.");
                await LoadAllPlaylistsAsync();
            });
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

        

        //------------
        public List<PlaylistListModel> PlaylistList { get; set; } = new List<PlaylistListModel>
            {
                new PlaylistListModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Music Track 1",
                    Description = "A playlist for relaxing and unwinding.",
                    NumberOfMusicTracks = 10,
                    TotalPlayTime = new TimeSpan(1, 50, 0) // 1 hour, 12 minutes
                },
                new PlaylistListModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Music Track 2",
                    Description = "A playlist for relaxing and unwinding.",
                    NumberOfMusicTracks = 15,
                    TotalPlayTime = new TimeSpan(1, 12, 0) // 1 hour, 12 minutes
                }
        };

    }
}
