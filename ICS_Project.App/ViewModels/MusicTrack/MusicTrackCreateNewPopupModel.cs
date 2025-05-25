using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using ICS_Project.App.Messages;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackCreateNewPopupModel : ObservableObject
{
    private readonly IMusicTrackFacade _facade;
    private readonly IArtistFacade _artistFacade;
    private readonly IGenreFacade _genreFacade;

    [ObservableProperty]
    private MusicTrackDetailModel _musicTrackDetail;

    [ObservableProperty]
    private ObservableCollection<ArtistListModel> _artists = []; // I think this is wrong and is meant to be used as a list of artists for the track, but I just need to choose one from the list of all artists 

    private List<ArtistListModel> _allArtists = []; // What is this good for?

    [ObservableProperty]
    private ObservableCollection<GenreListModel> _genres = []; // +- As said above, but maybe one music track can have multiple genres

    private List<GenreListModel> _allGenres = []; // What is this good for?

    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string _URL = "";

    [ObservableProperty]
    private string _hoursString = "00";

    [ObservableProperty]
    private string _minutesString = "00";

    [ObservableProperty]
    private string _secondsString = "00";

    private TimeSpan TotalDuration = TimeSpan.Zero;

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

    private bool _isRegistered = false;
    private bool _isNotRegistered = true; // What is this good for?

    private bool _isEditMode;

    public async Task InitializeAsync(Guid id)
    {
        MusicTrackDetail = await _facade.GetAsync(id);
        await LoadArtistsAsync();
        await LoadGenresAsync();
    }



    public MusicTrackCreateNewPopupModel(
        IMusicTrackFacade musicTrackFacade, 
        IArtistFacade artistFacade,
        IGenreFacade genreFacade)
	{
		_facade = musicTrackFacade;
        _artistFacade = artistFacade;
        _genreFacade = genreFacade;

        // there were number of tracks and something in OG. Are we missing something here aswell?

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
        // Register to receive PlaylistPopupContext message
        if (!_isRegistered)
        {
            _isRegistered = true;
            WeakReferenceMessenger.Default.Register<MusicTrackPopupContext>(this, async (r, m) =>
            {
                Debug.WriteLine("MusicTrackPopupContextMessage received in MusicTrackCreateNewPopupModel.");

                _isEditMode = m.IsEditMode;

                if (_isEditMode)
                {
                    Debug.WriteLine("Edit mode - loading existing music track.");
                    ListenForGUID();
                }
                else
                {
                    Debug.WriteLine("Create mode - initializing new music track.");
                    await LoadGenresAsync();
                    await LoadArtistsAsync();
                }
            });

        }
    }

    public async Task LoadArtistsAsync()
    {
        _allArtists = (await _artistFacade.GetAsync()).ToList();
        Artists = new ObservableCollection<ArtistListModel>(_allArtists);

        foreach (var artist in Artists)
        {
            artist.PropertyChanged += Artist_PropertyChanged;
        }
    }


    public async Task LoadGenresAsync()
    {
        _allGenres = (await _genreFacade.GetAsync()).ToList();
        Genres = new ObservableCollection<GenreListModel>(_allGenres);


        foreach (var genre in Genres)
        {
            genre.PropertyChanged += Genre_PropertyChanged;
        }
    }


    // Save music track command
    [RelayCommand]
    public async void SaveChanges()
    {
        // Constraints
        if (Title == "" || URL == "" || TotalDuration == TimeSpan.Zero || Description == "" || FileSizeMB == 0.0)
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Všechna pole musí být vyplněná", "OK");
        }
        else if (_selectedArtists.IsNullOrEmpty<ArtistListModel>())
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Musí být uveden alespoň jeden autor", "OK");
        }
        else if (_selectedGenres.IsNullOrEmpty<GenreListModel>())
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Musí být uveden alespoň jeden žánr", "OK");
        }
        else
        {
            // Check for changes
            bool scalarValuesChanged = MusicTrackDetail.Title != Title ||
                MusicTrackDetail.Description != Description ||
                MusicTrackDetail.Length != TotalDuration ||
                MusicTrackDetail.UrlAddress != URL ||
                MusicTrackDetail.Size != FileSizeMB;


            // Check if artists or genres have changed
            var currentArtistsIds = MusicTrackDetail.Artists.Select(t => t.Id).ToHashSet();
            var selectedArtistsIds = _selectedArtists.Select(t => t.Id).ToHashSet();
            var artistsChanged = !currentArtistsIds.SetEquals(selectedArtistsIds);


            var currentGenresIds = MusicTrackDetail.Genres.Select(t => t.Id).ToHashSet();
            var selectedGenresIds = _selectedGenres.Select(t => t.Id).ToHashSet();
            var genresChanged = !currentGenresIds.SetEquals(selectedGenresIds);


            // No changes -> skip saving 
            if (!scalarValuesChanged && !artistsChanged && !genresChanged)
            {
                WeakReferenceMessenger.Default.Send(new MusicTrackNewMusicTrackClosed());
                Debug.WriteLine("No changes detected, skipping save.");
                return;
            }


            // Changes detected -> save
            MusicTrackDetail.Title = Title;
            MusicTrackDetail.Description = Description;
            MusicTrackDetail.UrlAddress = URL;
            MusicTrackDetail.Length = TotalDuration;
            MusicTrackDetail.Size = FileSizeMB;



            if (_isEditMode) // Edit mode -> update existings lists
            {
                // Artists and genres to add (are selected, but not in the current track)
                var artistsToAdd = _selectedArtists
                    .Where(artist => !currentArtistsIds.Contains(artist.Id))
                    .ToList();
                var genresToAdd = _selectedGenres
                    .Where(genre => !currentGenresIds.Contains(genre.Id))
                    .ToList();


                // Artists and genres to remove (are in the current track, but not selected)
                var artistsToRemove = MusicTrackDetail.Artists
                    .Where(artist => !_selectedArtists.Any(t => t.Id == artist.Id))
                    .ToList();
                var genresToRemove = MusicTrackDetail.Genres
                    .Where(genre => !_selectedGenres.Any(t => t.Id == genre.Id))
                    .ToList();


                // Remove unselected artists and genres
                foreach (var artist in artistsToRemove)
                {
                    await _facade.RemoveArtistFromMusicTrackAsync(MusicTrackDetail.Id, artist.Id);
                }
                foreach (var genre in genresToRemove)
                {
                    await _facade.RemoveGenreFromMusicTrackAsync(MusicTrackDetail.Id, genre.Id);
                }


                // Add selected artists and genres
                foreach (var artist in artistsToAdd)
                {
                    await _facade.AddArtistToMusicTrackAsync(MusicTrackDetail.Id, artist.Id);
                }
                foreach (var genre in genresToAdd)
                {
                    await _facade.AddGenreToMusicTrackAsync(MusicTrackDetail.Id, genre.Id);
                }


                Debug.WriteLine("Music track updated:");
            }
            else // Creation mode -> fill list
            {
                MusicTrackDetail.Artists.Clear();
                MusicTrackDetail.Genres.Clear();
                var saveMusicTrack = await _facade.SaveAsync(MusicTrackDetail);
                foreach (var artist in _selectedArtists)
                {
                    await _facade.AddArtistToMusicTrackAsync(saveMusicTrack.Id, artist.Id);
                    Debug.WriteLine($"Added artist: {artist.ArtistName}");
                }
                foreach (var genre in _selectedGenres)
                {
                    await _facade.AddGenreToMusicTrackAsync(saveMusicTrack.Id, genre.Id);
                    Debug.WriteLine($"Added genre: {genre.GenreName}");
                }


                Debug.WriteLine("Music track created:");
            }


            Debug.WriteLine($"Title: {MusicTrackDetail.Title}");
            Debug.WriteLine($"Description: {MusicTrackDetail.Description}");
            Debug.WriteLine($"Length: {MusicTrackDetail.Length}");
            Debug.WriteLine($"URL: {MusicTrackDetail.UrlAddress}");
            Debug.WriteLine($"Size: {MusicTrackDetail.Size} MB");
            // Artists and genres cannot be read from MusicTrackDetail, because they are updated asynchronously - they might not be in the list yet (you cannot display it's Guid for the same reason)
            Debug.WriteLine($"Artists: {string.Join(", ", _selectedArtists.Select(g => g.ArtistName))}");
            Debug.WriteLine($"Genres: {string.Join(", ", _selectedGenres.Select(t => t.GenreName))}");


            if (_isEditMode)
            {
                // TODO: THIS IS WRONG, NEEDS TO BE DONE BETTER
                WeakReferenceMessenger.Default.Send(new MusicTrackSelectedMessage(MusicTrackDetail.Id));
            }
            if (scalarValuesChanged)
            {
                WeakReferenceMessenger.Default.Send(new MusicTrackListViewUpdate());
            }
            WeakReferenceMessenger.Default.Send(new MusicTrackNewMusicTrackClosed());
        }
    }


    // Revert changes command
    [RelayCommand]
    public void RevertChanges()
    {
        WeakReferenceMessenger.Default.Send(new MusicTrackNewMusicTrackClosed());
    }


    private void Artist_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ArtistListModel.IsSelected) &&
            sender is ArtistListModel artist)
        {
            if (artist.IsSelected)
            {
                if (_selectedArtistsIds.Add(artist.Id))
                {
                    _selectedArtists.Add(artist);
                }
            }
            else
            {
                if (_selectedArtistsIds.Remove(artist.Id))
                {
                    // Removing artist with given Id - just safer way
                    _selectedArtists.RemoveAll(t => t.Id == artist.Id);
                }
            }
        }
    }


    private void Genre_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GenreListModel.IsSelected) &&
            sender is GenreListModel genre)
        {
            if (genre.IsSelected)
            {
                if (_selectedGenresIds.Add(genre.Id))
                {
                    _selectedGenres.Add(genre);
                }
            }
            else
            {
                if (_selectedGenresIds.Remove(genre.Id))
                {
                    // Removing genre with given Id - just safer way
                    _selectedGenres.RemoveAll(t => t.Id == genre.Id);
                }
            }
        }
    }


    private void ListenForGUID()
    {
        if (_isNotRegistered)
        {
            WeakReferenceMessenger.Default.Register<MusicTrackEditGUID>(this, async (recipient, message) =>
            {
                Debug.WriteLine($"Received MusicTrackEditGUID with ID: {message.ID}");

                // Fetch the musicTrack detail using the provided GUID
                MusicTrackDetail = await _facade.GetAsync(message.ID);

                // Optional: Refresh the artists and their selection
                await LoadArtistsAsync();

                // Pre-select artists already in the musicTrack
                foreach (var artist in MusicTrackDetail.Artists)
                {
                    _originalArtistsIds.Add(artist.Id);

                    var match = Artists.FirstOrDefault(a => a.Id == artist.Id);
                    if (match != null)
                    {
                        match.IsSelected = true;
                    }
                }

                // Optional: Refresh the genres and their selection
                await LoadGenresAsync();

                // Pre-select genres already in the musicTrack
                foreach (var genre in MusicTrackDetail.Genres)
                {
                    _originalGenresIds.Add(genre.Id);

                    var match = Genres.FirstOrDefault(g => g.Id == genre.Id);
                    if (match != null)
                    {
                        match.IsSelected = true;
                    }
                }

                Title = MusicTrackDetail.Title;
                Description = MusicTrackDetail.Description;
                URL = MusicTrackDetail.UrlAddress;
                FileSizeMB = MusicTrackDetail.Size;
                FileSizeString = FileSizeMB.ToString();
                TotalDuration = MusicTrackDetail.Length;

                HoursString = TotalDuration.Hours.ToString("D2");
                MinutesString = TotalDuration.Minutes.ToString("D2");
                SecondsString = TotalDuration.Seconds.ToString("D2");
            });
            _isNotRegistered = false;
        }
    }

    // Searchcommands for authors
    [RelayCommand]
    public void SearchAuthors(string newText)
    {
        SearchbarAuthorsText = newText;
        Debug.WriteLine("SearchAuthors has triggered");
        FilterAuthors();
    }


    private void FilterAuthors()
    {
        Debug.WriteLine($"Searching for {SearchbarAuthorsText}");
        if (string.IsNullOrWhiteSpace(SearchbarAuthorsText))
        {
            Debug.WriteLine("Author Searchbar text is empty");
            Artists = new ObservableCollection<ArtistListModel>(_allArtists);
        }
        else
        {
            Debug.WriteLine("Author SearchbarText is NOT empty");
            var lower = SearchbarAuthorsText.ToLowerInvariant();
            var filtered = _allArtists
                .Where(artist =>
                    !string.IsNullOrWhiteSpace(artist.ArtistName) && artist.ArtistName.ToLowerInvariant().Contains(lower))
                .ToList();
            Artists = new ObservableCollection<ArtistListModel>(filtered);
        }


        foreach (var artist in Artists)
        {
            artist.PropertyChanged += Artist_PropertyChanged;
        }
    }

    // Searchcommands for genres
    [RelayCommand]
    public void SearchGenres(string newText)
    {
        SearchbarGenresText = newText;
        Debug.WriteLine("SearchGenres has triggered");
        FilterGenres();
    }


    private void FilterGenres()
    {
        Debug.WriteLine($"Searching for {SearchbarGenresText}");
        if (string.IsNullOrWhiteSpace(SearchbarGenresText))
        {
            Debug.WriteLine("Genre Searchbar text is empty");
            Genres = new ObservableCollection<GenreListModel>(_allGenres);
        }
        else
        {
            Debug.WriteLine("Genre SearchbarText is NOT empty");
            var lower = SearchbarGenresText.ToLowerInvariant();
            var filtered = _allGenres
                .Where(genre =>
                    !string.IsNullOrWhiteSpace(genre.GenreName) && genre.GenreName.ToLowerInvariant().Contains(lower))
                .ToList();
            Genres = new ObservableCollection<GenreListModel>(filtered);
        }


        foreach (var genre in Genres)
        {
            genre.PropertyChanged += Genre_PropertyChanged;
        }
    }


    [RelayCommand]
    private void ValidateAndCorrectHoursString()
    {
        string? newValue = HoursString;
        string correctedValue = newValue ?? "0"; // Default if Null

        if (string.IsNullOrWhiteSpace(newValue))
        {
            correctedValue = "00"; 
            Debug.WriteLine("Hour is whitespace");
        }
        else if (int.TryParse(newValue, out int hours))
        {
            if (hours < 0) correctedValue = "00";
            else if (hours > 99) correctedValue = "99";
            else correctedValue = hours.ToString("D2"); // Keep format with leading zeor
            Debug.WriteLine($"Hour valid number: {correctedValue}");
        }
        else
        {
            correctedValue = "00"; // Fallback
        }

        if (HoursString != correctedValue) 
        {
            HoursString = correctedValue; 
        }

        UpdateTotalDuration();
    }

    [RelayCommand]
    private void ValidateAndCorrectMinutesString()
    {
        string? newValue = MinutesString;
        string correctedValue = newValue ?? "0"; 

        if (string.IsNullOrWhiteSpace(newValue))
        {
            correctedValue = "00";
            Debug.WriteLine("Minute is whitespace");
        }
        else if (int.TryParse(newValue, out int minutes))
        {
            if (minutes < 0) correctedValue = "00";
            else if (minutes > 59) correctedValue = "59";
            else correctedValue = minutes.ToString("D2"); 
            Debug.WriteLine($"Minute valid number: {correctedValue}");
        }
        else
        {
            correctedValue = "00"; // Fallback
        }

        if (MinutesString != correctedValue) 
        {
            MinutesString = correctedValue;
        }

        UpdateTotalDuration();
    }

    [RelayCommand]
    private void ValidateAndCorrectSecondsString()
    {
        string newValue = SecondsString;
        string correctedValue = newValue ?? "0"; 

        if (string.IsNullOrWhiteSpace(newValue))
        {
            correctedValue = "00";
            Debug.WriteLine("Second is whitespace");
        }
        else if (int.TryParse(newValue, out int seconds))
        {
            if (seconds < 0) correctedValue = "00";
            else if (seconds > 59) correctedValue = "59";
            else correctedValue = seconds.ToString("D2"); 
            Debug.WriteLine($"Second valid number: {correctedValue}");
        }
        else
        {
            correctedValue = "00"; // Fallback
        }

        if (SecondsString != correctedValue) 
        {
            SecondsString = correctedValue;
        }

        UpdateTotalDuration();
    }

    private void UpdateTotalDuration()
    {
        if (int.TryParse(HoursString, out int hours) &&
            int.TryParse(MinutesString, out int minutes) &&
            int.TryParse(SecondsString, out int seconds))
        {
            TotalDuration = new TimeSpan(hours, minutes, seconds);
            Debug.WriteLine($"TotalDuration: {TotalDuration}"); // Debugging line
        }
        else
        {
            Debug.WriteLine("Failed to parse time values. Setting default Value");
            TotalDuration = TimeSpan.Zero; // Default value if parsing fails
        }
    }

    [RelayCommand]
    private void ParseAndCorrectFileSize()
    {
        string newValue = FileSizeString;
        string correctedValue = newValue ?? "0,0"; // Default if Null
        double megabytes = 0.0;

        if (string.IsNullOrWhiteSpace(newValue))
        {
            correctedValue = "0,0"; 
            Debug.WriteLine("File size is whitespace");
        }
        else if (double.TryParse(newValue, out megabytes))
        {
            if (megabytes < 0)
            {
                correctedValue = "0,0";
                megabytes = 0.0;
            }
            Debug.WriteLine($"File size valid number: {correctedValue}");
        }
        else
        {
            correctedValue = "0,0"; // Fallback
        }

        if (FileSizeString != correctedValue) 
        {
            FileSizeString = correctedValue;
        }

        FileSizeMB = megabytes;
    }
}