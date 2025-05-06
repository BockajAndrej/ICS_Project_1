using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Models;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging; // Needed for IMessenger and messages
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces; // Needed for your custom message

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistListViewModel : ViewModelBase
    {
        private readonly IPlaylistFacade _facade;
        string timestamp;

        [ObservableProperty]
        private ObservableCollection<PlaylistListModel> _playlists;

        private List<PlaylistListModel> _allPlaylists;

        [ObservableProperty]
        private string _searchPlaylist;


        [RelayCommand]
        private async Task SearchPlaylists(string inputText)
        {
            _searchPlaylist = inputText;
            Playlists.Clear();
            Filter();
        }

        private void Filter()
        {
            Debug.WriteLine($"Searching for {SearchPlaylist}");
            if (string.IsNullOrWhiteSpace(SearchPlaylist))
            {
                Debug.WriteLine("Searchbar text is empty");
                Playlists = new ObservableCollection<PlaylistListModel>(_allPlaylists);
            }
            else
            {
                Debug.WriteLine("SearchbarText is NOT empty");
                var lower = SearchPlaylist.ToLowerInvariant();
                var filtered = _allPlaylists
                    .Where(playlist =>
                        !string.IsNullOrWhiteSpace(playlist.Name) && playlist.Name.ToLowerInvariant().Contains(lower))
                    .ToList();
                Playlists = new ObservableCollection<PlaylistListModel>(filtered);
            }
        }

        [RelayCommand]
        public async Task LoadAllPlaylistsAsync()
        {
            _allPlaylists = (await _facade.GetAsync()).ToList();
            Playlists = new ObservableCollection<PlaylistListModel>(_allPlaylists);
        }

        public PlaylistListViewModel(
          IPlaylistFacade playlistFacade,
          IMessengerService messengerService) // Needs IMessengerService
          : base(messengerService) // Pass to base constructor
        {
            _facade = playlistFacade;
            SearchPlaylist = string.Empty;
            LoadAllPlaylistsAsync();
            ListenForCreation();
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

        //TODO: TEMPORARY SOLUTION, reflashing allPlaylists memory when new playlist is created

        private async void ListenForCreation()
        {
            WeakReferenceMessenger.Default.Register<PlaylistNewCreation>(this, async (r, m) =>
            {
                await LoadAllPlaylistsAsync();
            });
        }

    }
}
