using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackListViewModel : ObservableObject
{
    private readonly IMusicTrackFacade _facade;

    [ObservableProperty]
    private string _searchMusicTrackStr;

    [ObservableProperty]
    private ObservableCollection<MusicTrackListModel> _musicTracks = [];

    private List<MusicTrackListModel> _allMusicTracks;

    //TODO: for now suboptimal solution, allMusicTracks has to reload its memory every time new song is added
    [RelayCommand]
    private void SearchMusicTracks(string inputString)
    {
        MusicTracks.Clear();
        SearchMusicTrackStr = inputString;
        Filter();
    }

    private void Filter()
    {
        Debug.WriteLine($"Searching for {SearchMusicTrackStr}");
        if (string.IsNullOrWhiteSpace(SearchMusicTrackStr))
        {
            Debug.WriteLine("Searchbar text is empty");
            MusicTracks = new ObservableCollection<MusicTrackListModel>(_allMusicTracks);
        }
        else
        {
            Debug.WriteLine("SearchbarText is NOT empty");
            var lower = SearchMusicTrackStr.ToLowerInvariant();
            var filtered = _allMusicTracks
                .Where(track =>
                    !string.IsNullOrWhiteSpace(track.Title) && track.Title.ToLowerInvariant().Contains(lower))
                .ToList();
            MusicTracks = new ObservableCollection<MusicTrackListModel>(filtered);
        }
    }

    [RelayCommand]
    public async Task LoadAllMusicTracksAsync()
    {
        _allMusicTracks = (await _facade.GetAsync()).ToList();
        MusicTracks = new ObservableCollection<MusicTrackListModel>(_allMusicTracks);
    }

    public MusicTrackListViewModel(IMusicTrackFacade MusicTrackFacade)
    {
        _facade = MusicTrackFacade;
        LoadAllMusicTracksAsync();
    }
}