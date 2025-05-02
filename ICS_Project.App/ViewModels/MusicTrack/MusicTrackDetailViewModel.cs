using ICS_Project.BL.Models;
using System.Collections.ObjectModel;

namespace ICS_Project.App.ViewModels.MusicTrack;

public class MusicTrackDetailViewModel
{
    public MusicTrackDetailModel TestMusicTrack { get; set; } = new MusicTrackDetailModel()
    {
        Id = Guid.NewGuid(),
        Title = "Reconsider",
        Description = "Nieco sa stalo neviem ASDADASDADADADASD",
        Length = new TimeSpan(0, 3, 45), // 3 minutes, 45 seconds
        Size = 5.2, // Size in MB
        UrlAddress = "https://example.com/music/song.mp3",

    };
}
