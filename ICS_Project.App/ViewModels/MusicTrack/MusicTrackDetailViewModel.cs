using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Services.Interfaces;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel; // Keep this

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackDetailViewModel : ViewModelBase
{
    private readonly IMusicTrackFacade _facade;

    [ObservableProperty]
    private MusicTrackDetailModel? _trackDetails;

    public string Title => TrackDetails?.Title ?? "Loading...";
    public string Description => TrackDetails?.Description ?? string.Empty;
    public string FormattedLength => TrackDetails?.Length.ToString(@"mm\:ss") ?? "00:00";

    // NEW PROPERTIES FOR COMMA-SEPARATED LISTS
    public string ArtistsText => TrackDetails?.Artists != null && TrackDetails.Artists.Any()
        ? string.Join(", ", TrackDetails.Artists.Select(a => a.ArtistName)) // Assuming ArtistListModel has ArtistName
        : string.Empty; // Or "None", "N/A"

    public string PlaylistsText => TrackDetails?.Playlists != null && TrackDetails.Playlists.Any()
        ? string.Join(", ", TrackDetails.Playlists.Select(p => p.Name)) // Assuming PlaylistListModel has Name
        : string.Empty;

    public string GenresText => TrackDetails?.Genres != null && TrackDetails.Genres.Any()
        ? string.Join(", ", TrackDetails.Genres.Select(g => g.GenreName)) // Assuming GenreListModel has GenreName
        : string.Empty;


    public MusicTrackDetailViewModel(
        IMusicTrackFacade musicTrackFacade,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _facade = musicTrackFacade;
    }

    // No changes to Initialize needed

    public async Task LoadTrackAsync(Guid trackId)
    {
        if (trackId == Guid.Empty)
        {
            Debug.WriteLine("MusicTrackDetailViewModel: LoadTrackAsync called with empty ID.");
            TrackDetails = null;
            // OnPropertyChanged will be triggered for TrackDetails, which should
            // trigger updates for dependent properties like Title, ArtistsText etc.
            // if they are correctly observing TrackDetails or if you manually call OnPropertyChanged for them.
            // With CT.Mvvm, when _trackDetails changes, derived properties should re-evaluate.
            // However, it's safer to explicitly notify for these string-formatted properties.
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
            // Explicitly raise PropertyChanged for the new string properties
            // This ensures UI updates when TrackDetails (and thus its collections) are loaded/changed.
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(FormattedLength));
            OnPropertyChanged(nameof(ArtistsText));
            OnPropertyChanged(nameof(PlaylistsText));
            OnPropertyChanged(nameof(GenresText));
        }
    }
}