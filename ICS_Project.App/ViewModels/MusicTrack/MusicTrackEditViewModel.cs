using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.App.Services.Interfaces;
using ICS_Project.BL.Facades;
using System.Diagnostics;

namespace ICS_Project.App.ViewModels.MusicTrack;


public partial class MusicTrackEditViewModel : ViewModelBase
{
    private readonly IMusicTrackFacade _facade;
    private readonly IMessenger _messenger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPopupService _popupService;

    [ObservableProperty]
    private Guid _trackId;

    [ObservableProperty]
    private string? _trackTitle;

    public MusicTrackEditViewModel(
        IMusicTrackFacade musicTrackFacade,
        IMessengerService messengerService,
        IServiceProvider serviceProvider,
        IPopupService popupService) 
        : base(messengerService) 
    {
        _facade = musicTrackFacade;
        _messenger = messengerService.Messenger; 
        _serviceProvider = serviceProvider; 
        _popupService = popupService;

        ListenToGUIDRequest();
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
        await Task.CompletedTask;
    }

    public void ListenToGUIDRequest()
    {
        WeakReferenceMessenger.Default.Register<MusicTrackRequestGUID>(this, (recipient, message) =>
        {
            Debug.WriteLine($"[ListenToGUIDRequest] Received request, sending back GUID: {TrackId}");

            WeakReferenceMessenger.Default.Send(new MusicTrackEditGUID { ID = TrackId });
        });
    }

    [RelayCommand]
    private async Task DeleteMusicTrackAsync()
    {       
        if (_trackId == Guid.Empty)
        {
            Debug.WriteLine("[DeleteMusicTrackAsync] No track to delete.");
            return;
        }

        Debug.WriteLine($"[DeleteMusicTrackAsync] Trying to delete track with ID: {_trackId}");

        try
        {
            await _facade.DeleteAsync(_trackId);
            Debug.WriteLine($"[DeleteMusicTrackAsync] Track with ID: {_trackId} was successfully removed.");
            _messenger.Send(new MusicTrackDeletedMessage(_trackId));
            TrackId = Guid.Empty;
            TrackTitle = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[DeleteMusicTrackAsync] Error when trying to delete track: {ex.Message}");
        }

    }


}