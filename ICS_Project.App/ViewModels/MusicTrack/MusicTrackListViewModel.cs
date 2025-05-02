using ICS_Project.BL.Models;

namespace ICS_Project.App.ViewModels.MusicTrack;

public partial class MusicTrackListViewModel
{
    public List<MusicTrackListModel> MusicTrackList { get; set; } = new List<MusicTrackListModel>
    {
        new MusicTrackListModel
        {
            Id = Guid.NewGuid(),
            Title = "Take me with you",
            Description = "A mellow track for relaxing.",
            Length = new TimeSpan(0, 3, 45),
            Size = 5.2,
            UrlAddress = "https://example.com/music/take_me_with_you.mp3"
        },
    };
}