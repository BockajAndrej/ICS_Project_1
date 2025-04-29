using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Facades;

public interface IPlaylistFacade : IFacade<Playlist, PlaylistListModel, PlaylistDetailModel>
{
    public Task<IEnumerable<PlaylistListModel>> GetAsync(string searchTerm);
    public Task AddToPlaylist(Guid playlistId, PlaylistDetailModel musicTrack);
    public Task RemoveFromPlaylist(Guid playlistId, Guid musicTrackId);
}