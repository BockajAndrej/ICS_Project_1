using ICS_Project.App.ViewModels.Playlist;

namespace ICS_Project.App.Messages;

public class PlaylistShowOptions
{
    public VisualElement? Anchor { get; }
    public PlaylistDetailViewModel? ViewModel { get; }

    public PlaylistShowOptions(VisualElement? anchor, PlaylistDetailViewModel? viewModel)
    {
        Anchor = anchor;
        ViewModel = viewModel;
    }
}