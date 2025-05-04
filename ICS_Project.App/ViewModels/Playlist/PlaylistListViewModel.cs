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

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistListViewModel : ViewModelBase
    {
        private readonly IPlaylistFacade _facade;

        [ObservableProperty]
        private ObservableCollection<PlaylistListModel> _values = [];

        public async Task InitializeAsync()
        {
            Values = (await _facade.GetAsync()).ToObservableCollection();
        }

        public PlaylistListViewModel(
          IPlaylistFacade playlistFacade,
          IMessengerService messengerService) // Needs IMessengerService
          : base(messengerService) // Pass to base constructor
        {
            _facade = playlistFacade;
            // Remove InitializeAsync() call from constructor if base.OnAppearingAsync handles loading
        }

        [RelayCommand]
        public async Task AddAlbum()
        {
            var detailModelToCreate = new PlaylistDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "New Playlist From Facade Test",
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
