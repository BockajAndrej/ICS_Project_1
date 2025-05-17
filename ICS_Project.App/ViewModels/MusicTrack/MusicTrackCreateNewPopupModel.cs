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

    private List<ArtistListModel> _allArtist = []; // What is this good for?

    [ObservableProperty]
    private ObservableCollection<GenreListModel> _genres = []; // +- As said above, but maybe one music track can have multiple genres

    private List<GenreListModel> _allGenres = []; // What is this good for?

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string _URL = "";

    [ObservableProperty]
    private TimeSpan _length = TimeSpan.Zero;

    private readonly List<ArtistListModel> _selectedArtists = new();

    private readonly List<GenreListModel> _selectedGenres = new();

    private HashSet<Guid> _originalArtistsIds = new();
    private HashSet<Guid> _selectedArtistsIds = new();

    private HashSet<Guid> _originalGenresIds = new();
    private HashSet<Guid> _selectedGenresIds = new();

    private bool _isRegistered = false;
    private bool _isNotRegistered = true; // What is this good for?

    private bool _isEditMode;
    private bool _editedGenres; // TODO: currently isn't used - check it
    private bool _editedArtists;

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
            UrlAddress = ""
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

    }

    public async Task LoadGenresAsync()
    {

    }

    private void ListenForGUID()
    {
        throw new NotImplementedException();
    }
}