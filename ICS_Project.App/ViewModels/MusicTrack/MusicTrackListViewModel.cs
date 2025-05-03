using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ICS_Project.BL;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System.Collections.ObjectModel;

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackListViewModel : ObservableObject
{
    private readonly IMusicTrackFacade _facade;

    [ObservableProperty]
    private string _searchMusicTrackStr;

    [ObservableProperty]
    private ObservableCollection<MusicTrackListModel> _musicTracks = [];

    [RelayCommand]
    private async Task SearchMusicTracks()
    {
        MusicTracks.Clear();
        MusicTracks = (await _facade.GetAsync(_searchMusicTrackStr)).ToObservableCollection();
    }

    [RelayCommand]
    public async Task LoadAllMusicTracksAsync()
    {
        MusicTracks = (await _facade.GetAsync()).ToObservableCollection();
    }

    public MusicTrackListViewModel(IMusicTrackFacade MusicTrackFacade)
    {
        _facade = MusicTrackFacade;
        LoadAllMusicTracksAsync();
    }
}