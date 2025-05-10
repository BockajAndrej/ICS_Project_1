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
using System.Collections.ObjectModel; 

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackDetailViewModel : ViewModelBase
{
    private readonly IMusicTrackFacade _facade;

    [ObservableProperty]
    private MusicTrackDetailModel? _trackDetails;

    public string Title => TrackDetails?.Title ?? "Loading...";
    public string Description => TrackDetails?.Description ?? string.Empty;
    // Add properties for formatted Autor, Album, Genres, Length etc. if needed
    public string FormattedLength => TrackDetails?.Length.ToString(@"mm\:ss") ?? "00:00";

    public MusicTrackDetailViewModel(
    IMusicTrackFacade musicTrackFacade,
    IMessengerService messengerService) // Required by ViewModelBase
    : base(messengerService) // Pass messengerService to base
    {
        _facade = musicTrackFacade;
    }

    public void Initialize(Guid trackId, string trackTitle)
    {

    }

    public async Task LoadTrackAsync(Guid trackId)
    {
        if (trackId == Guid.Empty)
        {
            Debug.WriteLine("MusicTrackDetailViewModel: LoadTrackAsync called with empty ID.");
            TrackDetails = null; // Clear the observable property
            OnPropertyChanged(nameof(Title)); // Example
            OnPropertyChanged(nameof(FormattedLength));
            //OnPropertyChanged(nameof(ImageUrl));
            //OnPropertyChanged(nameof(ArtistsText));
            //OnPropertyChanged(nameof(GenresText));
            // OnPropertyChanged(nameof(AlbumText));
            return;
        }

        Debug.WriteLine($"MusicTrackDetailViewModel: Loading track with ID: {trackId}");
        try
        {
            // IsBusy = true;

            // *** THIS IS THE CORRECT ASSIGNMENT TO THE OBSERVABLE PROPERTY ***
            TrackDetails = await _facade.GetAsync(trackId); // GetAsync(Guid id) returns Task<MusicTrackDetailModel?>

            if (TrackDetails == null)
            {
                Debug.WriteLine($"MusicTrackDetailViewModel: Track with ID {trackId} not found by facade.");
                // TrackDetails is already null if not found, but explicitly set for clarity? No, GetAsync handles it.
                // You might set a flag indicating 'NotFound' state if needed.
            }
            else
            {
                Debug.WriteLine($"MusicTrackDetailViewModel: Track loaded: {TrackDetails.Title}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MusicTrackDetailViewModel: Error loading track: {ex.Message}");
            TrackDetails = null; // Clear the property on error
            // TODO: Handle error - show message to user
        }
        finally
        {
            // IsBusy = false;
            // Manually raise PropertyChanged for derived properties IF they don't
            // automatically notify when TrackDetails changes. CT.Mvvm might do this, but explicit is safe.
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(FormattedLength));
            //OnPropertyChanged(nameof(ImageUrl));
            //OnPropertyChanged(nameof(ArtistsText));
            //OnPropertyChanged(nameof(GenresText));
            // OnPropertyChanged(nameof(AlbumText));
        }
    }
}
