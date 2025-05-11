using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL.Facades;
using System.Diagnostics;
using System.Linq;
using ICS_Project.BL.Facades; // Assuming you might need facade
using ICS_Project.BL.Models;   // Assuming you might need models
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging; // Needed for IMessenger and messages
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces; // Needed for your custom message

namespace ICS_Project.App.ViewModels.MusicTrack;


public partial class MusicTrackEditViewModel : ViewModelBase
{
    private readonly IMusicTrackFacade _facade;
    private readonly IMessenger _messenger; // Inject the messenger

    [ObservableProperty]
    private Guid _trackId;

    [ObservableProperty]
    private string? _trackTitle;

    public MusicTrackEditViewModel(
        IMusicTrackFacade musicTrackFacade,
        IMessengerService messengerService) // Required by ViewModelBase
        : base(messengerService) // Pass messengerService to base
    {
        _facade = musicTrackFacade;
        _messenger = messengerService.Messenger; // Assign the messenger
    }

    public void Initialize(Guid trackId, string trackTitle)
    {
        _trackId = trackId;
        _trackTitle = trackTitle;
        Debug.WriteLine($"MusicTrackEditViewModel initialized for Track ID: {_trackId}, Title: {_trackTitle}");
    }

    [RelayCommand]
    private async Task AddToPlaylistAsync()
    {
        Debug.WriteLine($"Command: Add Track '{_trackTitle}' (ID: {_trackId}) to Playlist");
        // TODO: Implement logic, possibly show another popup to select playlist
        await Task.CompletedTask;
        //ClosePopup();
    }

    [RelayCommand]
    private async Task EditTrackAsync()
    {
        Debug.WriteLine($"Command: Edit Track '{_trackTitle}' (ID: {_trackId})");
        // TODO: Implement navigation to an edit page or show edit popup
        await Task.CompletedTask;
        //ClosePopup();
    }

    [RelayCommand]
    private async Task DeleteFromPlaylistAsync()
    {
        Debug.WriteLine($"Command: Delete Track '{_trackTitle}' (ID: {_trackId}) from Playlist");
        // TODO: Implement deletion logic
        await Task.CompletedTask;
    }


}