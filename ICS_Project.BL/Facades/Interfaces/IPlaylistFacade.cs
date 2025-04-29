using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Facades;

public interface IPlaylistFacade : IFacade<Playlist, PlaylistListModel, PlaylistDetailModel>
{
    public Task<IEnumerable<PlaylistListModel>> GetAsync(string searchTerm);

    public Task AddMusicTrackToPlaylistAsync(Guid playlistId, Guid musicTrackId);
    public Task RemoveMusicTrackFromPlaylistAsync(Guid playlistId, Guid musicTrackId);
}