using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackCreateNewPopupModel : ObservableObject
{
    private readonly IMusicTrackFacade _facade;
    private readonly IArtistFacade _artistFacade;
    private readonly IGenreFacade _genreFacade;

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

    private bool _isPopupContextRegistered = false;
    private bool _isGuidListenerRegistered = false;

    private bool _isEditMode;

    public async Task InitializeAsync(Guid id)
    {
        MusicTrackDetail = await _facade.GetAsync(id);
        if (MusicTrackDetail != null)
        {
            Title = MusicTrackDetail.Title;
            Description = MusicTrackDetail.Description;
            URL = MusicTrackDetail.UrlAddress;

            SongDuration = MusicTrackDetail.Length;
            UpdatePickersFromSongDuration();

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
        IGenreFacade genreFacade)
    {
        _facade = musicTrackFacade;
        _artistFacade = artistFacade;
        _genreFacade = genreFacade;

        MinuteOptions = new ObservableCollection<int>(Enumerable.Range(0, 60));
        SecondOptions = new ObservableCollection<int>(Enumerable.Range(0, 60));
        UpdatePickersFromSongDuration();

        MusicTrackDetail = new MusicTrackDetailModel
        {
            Title = "",
            Description = "",
            Length = TimeSpan.Zero,
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

                SongDuration = TimeSpan.Zero;

                if (_isEditMode)
                {
                    Debug.WriteLine("Edit mode - preparing to listen for GUID.");
                    ListenForGUID();
                }
                else
                {
                    Debug.WriteLine("Create mode - initializing new music track.");
                    // Reset MusicTrackDetail for new entry
                    MusicTrackDetail = new MusicTrackDetailModel
                    {
                        Length = TimeSpan.Zero,
                        Artists = new ObservableCollection<ArtistListModel>(),
                        Genres = new ObservableCollection<GenreListModel>()
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
        ParseAndCorrectFileSize();

        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(URL) || SongDuration == TimeSpan.Zero ||
            string.IsNullOrWhiteSpace(Description) || FileSizeMB == 0.0)
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

        MusicTrackDetail.Title = Title;
        MusicTrackDetail.Description = Description;
        MusicTrackDetail.UrlAddress = URL;
        MusicTrackDetail.Length = SongDuration;
        MusicTrackDetail.Size = FileSizeMB;


        bool scalarValuesChanged = true;

        if (_isEditMode)
        {
            var currentArtistsIds = MusicTrackDetail.Artists.Select(t => t.Id).ToHashSet();
            var artistsToAdd = _selectedArtists.Where(artist => !currentArtistsIds.Contains(artist.Id)).ToList();
            var artistsToRemove = MusicTrackDetail.Artists.Where(artist => !_selectedArtists.Any(t => t.Id == artist.Id)).ToList();

            var currentGenresIds = MusicTrackDetail.Genres.Select(t => t.Id).ToHashSet();
            var genresToAdd = _selectedGenres.Where(genre => !currentGenresIds.Contains(genre.Id)).ToList();
            var genresToRemove = MusicTrackDetail.Genres.Where(genre => !_selectedGenres.Any(t => t.Id == genre.Id)).ToList();

            foreach (var artist in artistsToRemove) await _facade.RemoveArtistFromMusicTrackAsync(MusicTrackDetail.Id, artist.Id);
            foreach (var genre in genresToRemove) await _facade.RemoveGenreFromMusicTrackAsync(MusicTrackDetail.Id, genre.Id);
            foreach (var artist in artistsToAdd) await _facade.AddArtistToMusicTrackAsync(MusicTrackDetail.Id, artist.Id);
            foreach (var genre in genresToAdd) await _facade.AddGenreToMusicTrackAsync(MusicTrackDetail.Id, genre.Id);

            Debug.WriteLine("Music track updated (git logic).");
        }
        else
        {
            MusicTrackDetail.Artists.Clear();
            MusicTrackDetail.Genres.Clear();
            var savedTrack = await _facade.SaveAsync(MusicTrackDetail);

            if (savedTrack == null || savedTrack.Id == Guid.Empty)
            {
                Debug.WriteLine("Failed to save new music track or ID missing.");
                await Application.Current.MainPage.DisplayAlert("Chyba", "Nepodařilo se uložit novou skladbu.", "OK");
                return;
            }
            MusicTrackDetail.Id = savedTrack.Id; // Update the main models ID

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
                FileSizeString = FileSizeMB.ToString("0.0", CultureInfo.InvariantCulture);
                OnPropertyChanged(nameof(FileSizeString));

                SongDuration = MusicTrackDetail.Length;
                UpdatePickersFromSongDuration();

                await LoadAndPreselectArtistsAndGenres();
            });
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

    private void UpdatePickersFromSongDuration()
    {
        SelectedMinutes = SongDuration.Minutes;
        SelectedSeconds = SongDuration.Seconds;
    }

    private void UpdateSongDurationFromPickers()
    {
        SongDuration = new TimeSpan(0, SelectedMinutes, SelectedSeconds);
        Debug.WriteLine($"SongDuration updated from pickers: {SongDuration}");
    }

    [RelayCommand]
    private void ParseAndCorrectFileSize()
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
                megabytes = FileSizeMB;
                correctedValue = megabytes.ToString("0.0###############", CultureInfo.InvariantCulture);
                Debug.WriteLine($"Invalid file size input '{newValue}'. Reverted/kept: {correctedValue}");
            }
        }
        if (FileSizeString != correctedValue) { FileSizeString = correctedValue; }
        FileSizeMB = megabytes;
    }
}