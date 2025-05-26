using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using ICS_Project.App.Messages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
// using ICS_Project.DAL.Entities; // Usually present
// using System.ComponentModel.DataAnnotations; // Usually present
using Microsoft.IdentityModel.Tokens; // Usually for other parts of app
using System.Globalization; // For FileSize parsing

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackCreateNewPopupModel : ObservableObject
{
    private readonly IMusicTrackFacade _facade;
    private readonly IArtistFacade _artistFacade;
    private readonly IGenreFacade _genreFacade;
    private readonly IPlaylistFacade _playlistFacade;

    [ObservableProperty]
    private MusicTrackDetailModel _musicTrackDetail;

    [ObservableProperty]
    private ObservableCollection<ArtistListModel> _artists = [];

    private List<ArtistListModel> _allArtists = [];

    [ObservableProperty]
    private ObservableCollection<GenreListModel> _genres = [];

    private List<GenreListModel> _allGenres = [];

    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string _URL = "";

    // --- REMOVED OLD DURATION PROPERTIES ---
    // [ObservableProperty]
    // private string _hoursString = "00";
    // [ObservableProperty]
    // private string _minutesString = "00";
    // [ObservableProperty]
    // private string _secondsString = "00";
    // private TimeSpan TotalDuration = TimeSpan.Zero;
    // --- END REMOVED OLD DURATION PROPERTIES ---

    // +++ ADDED PICKER DURATION PROPERTIES +++
    [ObservableProperty]
    private TimeSpan _songDuration = TimeSpan.Zero;

    public ObservableCollection<int> MinuteOptions { get; }
    public ObservableCollection<int> SecondOptions { get; }

    private int _selectedMinutes;
    public int SelectedMinutes
    {
        get => _selectedMinutes;
        set
        {
            if (SetProperty(ref _selectedMinutes, value))
            {
                UpdateSongDurationFromPickers();
            }
        }
    }

    private int _selectedSeconds;
    public int SelectedSeconds
    {
        get => _selectedSeconds;
        set
        {
            if (SetProperty(ref _selectedSeconds, value))
            {
                UpdateSongDurationFromPickers();
            }
        }
    }
    // +++ END ADDED PICKER DURATION PROPERTIES +++

    [ObservableProperty]
    private string _fileSizeString = "";

    private double FileSizeMB = 0.0;

    [ObservableProperty]
    private string _searchbarAuthorsText = "";

    [ObservableProperty]
    private string _searchbarGenresText = "";

    private readonly List<ArtistListModel> _selectedArtists = new();
    private readonly List<GenreListModel> _selectedGenres = new();

    private HashSet<Guid> _originalArtistsIds = new();
    private HashSet<Guid> _selectedArtistsIds = new();

    private HashSet<Guid> _originalGenresIds = new();
    private HashSet<Guid> _selectedGenresIds = new();

    // Using combined flags for clarity from previous iteration
    private bool _isPopupContextRegistered = false;
    private bool _isGuidListenerRegistered = false;

    private bool _isEditMode;

    public async Task InitializeAsync(Guid id) // If directly navigating to edit
    {
        MusicTrackDetail = await _facade.GetAsync(id);
        if (MusicTrackDetail != null)
        {
            Title = MusicTrackDetail.Title;
            Description = MusicTrackDetail.Description;
            URL = MusicTrackDetail.UrlAddress;

            // +++ MODIFIED FOR PICKER DURATION +++
            SongDuration = MusicTrackDetail.Length;
            UpdatePickersFromSongDuration();
            // +++ END MODIFIED FOR PICKER DURATION +++

            FileSizeMB = MusicTrackDetail.Size;
            FileSizeString = FileSizeMB.ToString(CultureInfo.InvariantCulture);

            await LoadAndPreselectArtistsAndGenres();
        }
    }

    private async Task LoadAndPreselectArtistsAndGenres() // Helper from previous iteration
    {
        await LoadArtistsAsync();
        _originalArtistsIds.Clear(); _selectedArtistsIds.Clear(); _selectedArtists.Clear();
        if (MusicTrackDetail?.Artists != null)
            foreach (var artist in MusicTrackDetail.Artists)
            { _originalArtistsIds.Add(artist.Id); var match = Artists.FirstOrDefault(a => a.Id == artist.Id); if (match != null) match.IsSelected = true; }

        await LoadGenresAsync();
        _originalGenresIds.Clear(); _selectedGenresIds.Clear(); _selectedGenres.Clear();
        if (MusicTrackDetail?.Genres != null)
            foreach (var genre in MusicTrackDetail.Genres)
            { _originalGenresIds.Add(genre.Id); var match = Genres.FirstOrDefault(g => g.Id == genre.Id); if (match != null) match.IsSelected = true; }
    }

    public MusicTrackCreateNewPopupModel(
        IMusicTrackFacade musicTrackFacade,
        IArtistFacade artistFacade,
        IGenreFacade genreFacade,
        IPlaylistFacade playlistFacade)
    {
        _facade = musicTrackFacade;
        _artistFacade = artistFacade;
        _genreFacade = genreFacade;
        _playlistFacade = playlistFacade;

        // +++ ADDED PICKER INITIALIZATION +++
        MinuteOptions = new ObservableCollection<int>(Enumerable.Range(0, 60));
        SecondOptions = new ObservableCollection<int>(Enumerable.Range(0, 60));
        // SongDuration is already TimeSpan.Zero by default from its declaration.
        UpdatePickersFromSongDuration(); // Initialize pickers based on default SongDuration
        // +++ END ADDED PICKER INITIALIZATION +++

        MusicTrackDetail = new MusicTrackDetailModel
        {
            Title = "",
            Description = "",
            Length = TimeSpan.Zero, // Initial length for the model
            Size = 0,
            UrlAddress = "",
            Artists = new ObservableCollection<ArtistListModel>(),
            Genres = new ObservableCollection<GenreListModel>()
        };

        WeakReferenceMessenger.Default.UnregisterAll(this);
        if (!_isPopupContextRegistered) // Using combined flag
        {
            _isPopupContextRegistered = true;
            WeakReferenceMessenger.Default.Register<MusicTrackPopupContext>(this, async (r, m) =>
            {
                Debug.WriteLine("MusicTrackPopupContextMessage received.");
                _isEditMode = m.IsEditMode;

                // Reset common fields
                Title = ""; Description = ""; URL = "";
                FileSizeString = "0.0"; FileSizeMB = 0.0;
                SearchbarAuthorsText = ""; SearchbarGenresText = "";
                _selectedArtists.Clear(); _selectedGenres.Clear();
                _selectedArtistsIds.Clear(); _selectedGenresIds.Clear();
                _originalArtistsIds.Clear(); _originalGenresIds.Clear();
                if (Artists != null) foreach (var artist in Artists) artist.IsSelected = false;
                if (Genres != null) foreach (var genre in Genres) genre.IsSelected = false;

                // +++ MODIFIED FOR PICKER DURATION +++
                SongDuration = TimeSpan.Zero; // This will also update SelectedMinutes/Seconds via its setter chain
                // +++ END MODIFIED FOR PICKER DURATION +++

                if (_isEditMode)
                {
                    Debug.WriteLine("Edit mode - preparing to listen for GUID.");
                    ListenForGUID();
                }
                else
                {
                    Debug.WriteLine("Create mode - initializing new music track.");
                    // Reset MusicTrackDetail for new entry (ensure Length is TimeSpan.Zero)
                    MusicTrackDetail = new MusicTrackDetailModel
                    {
                        Length = TimeSpan.Zero, // Reset length
                        Artists = new ObservableCollection<ArtistListModel>(), // Or null if BL requires
                        Genres = new ObservableCollection<GenreListModel>()   // Or null if BL requires
                    };
                    await LoadArtistsAsync();
                    await LoadGenresAsync();
                }
            });
        }
    }

    public async Task LoadArtistsAsync()
    {
        if (Artists != null) foreach (var artist in Artists.ToList()) artist.PropertyChanged -= Artist_PropertyChanged;
        _allArtists = (await _artistFacade.GetAsync()).ToList();
        Artists = new ObservableCollection<ArtistListModel>(_allArtists);
        foreach (var artist in Artists) { artist.IsSelected = _selectedArtistsIds.Contains(artist.Id); artist.PropertyChanged += Artist_PropertyChanged; }
    }

    public async Task LoadGenresAsync()
    {
        if (Genres != null) foreach (var genre in Genres.ToList()) genre.PropertyChanged -= Genre_PropertyChanged;
        _allGenres = (await _genreFacade.GetAsync()).ToList();
        Genres = new ObservableCollection<GenreListModel>(_allGenres);
        foreach (var genre in Genres) { genre.IsSelected = _selectedGenresIds.Contains(genre.Id); genre.PropertyChanged += Genre_PropertyChanged; }
    }

    [RelayCommand]
    public async void SaveChanges()
    {
        ParseAndCorrectFileSize(); // Ensure FileSizeMB is current

        // +++ MODIFIED VALIDATION FOR PICKER DURATION +++
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(URL) || SongDuration == TimeSpan.Zero ||
            string.IsNullOrWhiteSpace(Description) || FileSizeMB == 0.0)
        // +++ END MODIFIED VALIDATION FOR PICKER DURATION +++
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Všechna pole musí být vyplněná", "OK");
            return;
        }
        if (!_selectedArtists.Any()) // Simpler check
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Musí být uveden alespoň jeden autor", "OK");
            return;
        }
        if (!_selectedGenres.Any()) // Simpler check
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Musí být uveden alespoň jeden žánr", "OK");
            return;
        }

        // Prepare model to save - original git approach modified Model directly
        MusicTrackDetail.Title = Title;
        MusicTrackDetail.Description = Description;
        MusicTrackDetail.UrlAddress = URL;
        // +++ MODIFIED FOR PICKER DURATION +++
        MusicTrackDetail.Length = SongDuration;
        // +++ END MODIFIED FOR PICKER DURATION +++
        MusicTrackDetail.Size = FileSizeMB;

        // Simplified change detection from git version for brevity.
        // A robust solution would compare original values if _isEditMode.
        bool scalarValuesChanged = true; // Assume changes for this simplified merge.

        if (_isEditMode)
        {
            
            // Original git logic for edit mode
            var currentArtistsIds = MusicTrackDetail.Artists.Select(t => t.Id).ToHashSet();
            var artistsToAdd = _selectedArtists.Where(artist => !currentArtistsIds.Contains(artist.Id)).ToList();
            var artistsToRemove = MusicTrackDetail.Artists.Where(artist => !_selectedArtists.Any(t => t.Id == artist.Id)).ToList();
            MusicTrackDetail.Artists.Clear(); // Clear before adding new ones


            var currentGenresIds = MusicTrackDetail.Genres.Select(t => t.Id).ToHashSet();
            var genresToAdd = _selectedGenres.Where(genre => !currentGenresIds.Contains(genre.Id)).ToList();
            var genresToRemove = MusicTrackDetail.Genres.Where(genre => !_selectedGenres.Any(t => t.Id == genre.Id)).ToList();
            MusicTrackDetail.Genres.Clear(); // Clear before adding new ones

            var playlistIds = MusicTrackDetail.Playlists.Select(t => t.Id).ToHashSet();
            MusicTrackDetail.Playlists.Clear();

            // Save scalar properties. The original git logic implies _facade.SaveAsync handles this.
            // If _facade.SaveAsync *only* updates scalar for existing IDs OR your BL model properties are not init-only, this is fine.
            // Otherwise, you might need a separate DTO or method for scalar updates.
            await _facade.SaveAsync(MusicTrackDetail); // Assumes this updates scalar if ID exists

            foreach (var artist in artistsToRemove) await _facade.RemoveArtistFromMusicTrackAsync(MusicTrackDetail.Id, artist.Id);
            foreach (var genre in genresToRemove) await _facade.RemoveGenreFromMusicTrackAsync(MusicTrackDetail.Id, genre.Id);
            foreach (var artist in artistsToAdd) await _facade.AddArtistToMusicTrackAsync(MusicTrackDetail.Id, artist.Id);
            foreach (var genre in genresToAdd) await _facade.AddGenreToMusicTrackAsync(MusicTrackDetail.Id, genre.Id);

            Debug.WriteLine("Music track updated (git logic).");
        }
        else // Creation mode
        {
            // Original git logic for creation mode.
            // IMPORTANT: If MusicTrackDetailModel.Artists/Genres are init-only AND your facade guard
            // requires them to be NULL for INSERT, this will fail.
            // In that case, you'd `new MusicTrackDetailModel { Artists = null, Genres = null, ... }` here.
            MusicTrackDetail.Artists.Clear();
            MusicTrackDetail.Genres.Clear();
            var savedTrack = await _facade.SaveAsync(MusicTrackDetail);

            if (savedTrack == null || savedTrack.Id == Guid.Empty)
            {
                Debug.WriteLine("Failed to save new music track or ID missing.");
                await Application.Current.MainPage.DisplayAlert("Chyba", "Nepodařilo se uložit novou skladbu.", "OK");
                return;
            }
            MusicTrackDetail.Id = savedTrack.Id; // Update the main model's ID

            foreach (var artist in _selectedArtists) await _facade.AddArtistToMusicTrackAsync(savedTrack.Id, artist.Id);
            foreach (var genre in _selectedGenres) await _facade.AddGenreToMusicTrackAsync(savedTrack.Id, genre.Id);
            Debug.WriteLine("Music track created (git logic).");
        }

        Debug.WriteLine($"Title: {MusicTrackDetail.Title}, Length: {MusicTrackDetail.Length}, Size: {MusicTrackDetail.Size}MB");
        if (scalarValuesChanged || !_isEditMode) // Simplified condition from git
        {
            WeakReferenceMessenger.Default.Send(new MusicTrackListViewUpdate());
        }
        WeakReferenceMessenger.Default.Send(new MusicTrackSelectedMessage(MusicTrackDetail.Id));
        WeakReferenceMessenger.Default.Send(new MusicTrackNewMusicTrackClosed());
    }

    [RelayCommand]
    public void RevertChanges()
    {
        WeakReferenceMessenger.Default.Send(new MusicTrackNewMusicTrackClosed());
    }

    private void Artist_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ArtistListModel.IsSelected) && sender is ArtistListModel artist)
        {
            if (artist.IsSelected) { if (_selectedArtistsIds.Add(artist.Id)) _selectedArtists.Add(artist); }
            else { if (_selectedArtistsIds.Remove(artist.Id)) _selectedArtists.RemoveAll(t => t.Id == artist.Id); }
        }
    }

    private void Genre_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GenreListModel.IsSelected) && sender is GenreListModel genre)
        {
            if (genre.IsSelected) { if (_selectedGenresIds.Add(genre.Id)) _selectedGenres.Add(genre); }
            else { if (_selectedGenresIds.Remove(genre.Id)) _selectedGenres.RemoveAll(t => t.Id == genre.Id); }
        }
    }

    private void ListenForGUID()
    {
        if (!_isGuidListenerRegistered) // Using combined flag
        {
            _isGuidListenerRegistered = true;
            WeakReferenceMessenger.Default.Register<MusicTrackEditGUID>(this, async (recipient, message) =>
            {
                Debug.WriteLine($"Received MusicTrackEditGUID with ID: {message.ID}");
                var loadedTrack = await _facade.GetAsync(message.ID);
                if (loadedTrack == null)
                {
                    Debug.WriteLine($"Track {message.ID} not found.");
                    WeakReferenceMessenger.Default.Send(new MusicTrackNewMusicTrackClosed());
                    return;
                }
                MusicTrackDetail = loadedTrack;

                Title = MusicTrackDetail.Title;
                Description = MusicTrackDetail.Description;
                URL = MusicTrackDetail.UrlAddress;
                FileSizeMB = MusicTrackDetail.Size;
                FileSizeString = FileSizeMB.ToString("0.0", CultureInfo.InvariantCulture); // Format consistently
                OnPropertyChanged(nameof(FileSizeString));

                // +++ MODIFIED FOR PICKER DURATION +++
                SongDuration = MusicTrackDetail.Length;
                UpdatePickersFromSongDuration();
                // --- REMOVE OLD STRING ASSIGNMENTS ---
                // HoursString = TotalDuration.Hours.ToString("D2");
                // MinutesString = TotalDuration.Minutes.ToString("D2");
                // SecondsString = TotalDuration.Seconds.ToString("D2");
                // +++ END MODIFIED FOR PICKER DURATION +++

                await LoadAndPreselectArtistsAndGenres();
            });
            // _isNotRegistered = false; // This was part of original git's flag logic
        }
    }

    [RelayCommand]
    public void SearchAuthors(string newText)
    {
        SearchbarAuthorsText = newText ?? ""; FilterAuthors();
    }

    private void FilterAuthors()
    {
        if (Artists == null) return;
        foreach (var artist in Artists.ToList()) artist.PropertyChanged -= Artist_PropertyChanged;
        var filtered = string.IsNullOrWhiteSpace(SearchbarAuthorsText) ? _allArtists : _allArtists.Where(artist => !string.IsNullOrWhiteSpace(artist.ArtistName) && artist.ArtistName.ToLowerInvariant().Contains(SearchbarAuthorsText.ToLowerInvariant())).ToList();
        Artists = new ObservableCollection<ArtistListModel>(filtered);
        foreach (var artist in Artists) { artist.IsSelected = _selectedArtistsIds.Contains(artist.Id); artist.PropertyChanged += Artist_PropertyChanged; }
    }

    [RelayCommand]
    public void SearchGenres(string newText)
    {
        SearchbarGenresText = newText ?? ""; FilterGenres();
    }

    private void FilterGenres()
    {
        if (Genres == null) return;
        foreach (var genre in Genres.ToList()) genre.PropertyChanged -= Genre_PropertyChanged;
        var filtered = string.IsNullOrWhiteSpace(SearchbarGenresText) ? _allGenres : _allGenres.Where(genre => !string.IsNullOrWhiteSpace(genre.GenreName) && genre.GenreName.ToLowerInvariant().Contains(SearchbarGenresText.ToLowerInvariant())).ToList();
        Genres = new ObservableCollection<GenreListModel>(filtered);
        foreach (var genre in Genres) { genre.IsSelected = _selectedGenresIds.Contains(genre.Id); genre.PropertyChanged += Genre_PropertyChanged; }
    }

    // --- REMOVED OLD DURATION VALIDATION METHODS ---
    // [RelayCommand] private void ValidateAndCorrectHoursString() { /* ... */ }
    // [RelayCommand] private void ValidateAndCorrectMinutesString() { /* ... */ }
    // [RelayCommand] private void ValidateAndCorrectSecondsString() { /* ... */ }
    // private void UpdateTotalDuration() { /* ... */ }
    // --- END REMOVED OLD DURATION VALIDATION METHODS ---

    // +++ ADDED PICKER HELPER METHODS +++
    private void UpdatePickersFromSongDuration()
    {
        SelectedMinutes = SongDuration.Minutes;
        SelectedSeconds = SongDuration.Seconds;
    }

    private void UpdateSongDurationFromPickers()
    {
        // Assuming hours are not handled by these pickers and remain 0
        SongDuration = new TimeSpan(0, SelectedMinutes, SelectedSeconds);
        Debug.WriteLine($"SongDuration updated from pickers: {SongDuration}");
    }
    // +++ END ADDED PICKER HELPER METHODS +++

    [RelayCommand]
    private void ParseAndCorrectFileSize() // From git, improved
    {
        string? newValue = FileSizeString;
        string correctedValue;
        double megabytes = 0.0;

        if (string.IsNullOrWhiteSpace(newValue)) { correctedValue = "0.0"; }
        else
        {
            string parseValue = newValue.Replace(',', '.');
            if (double.TryParse(parseValue, NumberStyles.Any, CultureInfo.InvariantCulture, out megabytes))
            {
                if (megabytes < 0) megabytes = 0.0;
                correctedValue = megabytes.ToString("0.0###############", CultureInfo.InvariantCulture);
            }
            else
            {
                megabytes = FileSizeMB; // Revert to last valid or default
                correctedValue = megabytes.ToString("0.0###############", CultureInfo.InvariantCulture);
                Debug.WriteLine($"Invalid file size input '{newValue}'. Reverted/kept: {correctedValue}");
            }
        }
        if (FileSizeString != correctedValue) { FileSizeString = correctedValue; }
        FileSizeMB = megabytes;
    }
}