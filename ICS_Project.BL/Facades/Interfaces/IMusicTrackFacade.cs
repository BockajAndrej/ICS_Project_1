using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Facades;

public interface IMusicTrackFacade : IFacade<MusicTrack, MusicTrackListModel, MusicTrackDetailModel>
{
    public Task<IEnumerable<MusicTrackListModel>> GetAsync(string searchTerm);
    public Task AddArtistToMusicTrackAsync(Guid musicTrackId, Guid artistId);
    public Task RemoveArtistFromMusicTrackAsync(Guid musicTrackId, Guid artistId);
    public Task AddGenreToMusicTrackAsync(Guid musicTrackId, Guid genreId);
    public Task RemoveGenreFromMusicTrackAsync(Guid playlistId, Guid genreId);
}