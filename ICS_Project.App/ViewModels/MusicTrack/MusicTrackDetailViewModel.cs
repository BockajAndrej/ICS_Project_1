using CommunityToolkit.Mvvm.ComponentModel;
using ICS_Project.App.Services.Interfaces;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System.Diagnostics;

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackDetailViewModel : ViewModelBase
{
    private readonly IMusicTrackFacade _facade;

    [ObservableProperty]
    private MusicTrackDetailModel? _trackDetails;

    public string Title => TrackDetails?.Title ?? "Loading...";
    public string Description => TrackDetails?.Description ?? string.Empty;
    public string FormattedLength => TrackDetails?.Length.ToString(@"mm\:ss") ?? "00:00";

    public string ArtistsText => TrackDetails?.Artists != null && TrackDetails.Artists.Any()
        ? string.Join(", ", TrackDetails.Artists.Select(a => a.ArtistName)) 
        : string.Empty;

    public string PlaylistsText => TrackDetails?.Playlists != null && TrackDetails.Playlists.Any()
        ? string.Join(", ", TrackDetails.Playlists.Select(p => p.Name)) 
        : string.Empty;

    public string GenresText => TrackDetails?.Genres != null && TrackDetails.Genres.Any()
        ? string.Join(", ", TrackDetails.Genres.Select(g => g.GenreName)) 
        : string.Empty;


    public MusicTrackDetailViewModel(
        IMusicTrackFacade musicTrackFacade,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _facade = musicTrackFacade;
    }


    public async Task LoadTrackAsync(Guid trackId)
    {
        if (trackId == Guid.Empty)
        {
            Debug.WriteLine("MusicTrackDetailViewModel: LoadTrackAsync called with empty ID.");
            TrackDetails = null;
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(FormattedLength));
            OnPropertyChanged(nameof(ArtistsText));
            OnPropertyChanged(nameof(PlaylistsText));
            OnPropertyChanged(nameof(GenresText));
            return;
        }

        Debug.WriteLine($"MusicTrackDetailViewModel: Loading track with ID: {trackId}");
        try
        {
            TrackDetails = await _facade.GetAsync(trackId);

            if (TrackDetails == null)
            {
                Debug.WriteLine($"MusicTrackDetailViewModel: Track with ID {trackId} not found by facade.");
            }
            else
            {
                Debug.WriteLine($"MusicTrackDetailViewModel: Track loaded: {TrackDetails.Title}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MusicTrackDetailViewModel: Error loading track: {ex.Message}");
            TrackDetails = null;
        }
        finally
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(FormattedLength));
            OnPropertyChanged(nameof(ArtistsText));
            OnPropertyChanged(nameof(PlaylistsText));
            OnPropertyChanged(nameof(GenresText));
        }
    }
}