using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.ViewModels.MusicTrack;


namespace ICS_Project.App.Messages;

public class MusicTrackShowDetail
{
    public VisualElement? Anchor { get; }

    public PlaylistTrackViewModel? ViewModel { get; }

    public MusicTrackShowDetail(VisualElement? anchor, PlaylistTrackViewModel? viewModel)
    {
        Anchor = anchor;
        ViewModel = viewModel;
    }

}