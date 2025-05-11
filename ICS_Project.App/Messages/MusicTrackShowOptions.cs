using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.ViewModels.MusicTrack;


namespace ICS_Project.App.Messages;

public class MusicTrackShowOptions
{
    public VisualElement? Anchor { get; }

    public PlaylistTrackViewModel? ViewModel { get; }

    public MusicTrackShowOptions(VisualElement? anchor, PlaylistTrackViewModel? viewModel)
    {
        Anchor = anchor;
        ViewModel = viewModel;
    }

}