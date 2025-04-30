using ICS_Project.BL.Models;

namespace ICS_Project.App.ViewModels.Playlist
{
    public partial class PlaylistListViewModel
    {
        public List<PlaylistListModel> PlaylistList { get; set; } = new List<PlaylistListModel>
            {
                new PlaylistListModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Chill Vibes",
                    Description = "A playlist for relaxing and unwinding.",
                    NumberOfMusicTracks = 15,
                    TotalPlayTime = new TimeSpan(1, 12, 0) // 1 hour, 12 minutes
                }
        };
    }
}
