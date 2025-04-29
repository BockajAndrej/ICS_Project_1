using System.Linq.Expressions;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.UnitOfWork;

namespace ICS_Project.BL;

public class PlaylistFacade(
    IUnitOfWorkFactory uowf, 
    IPlaylistModelMapper modelMapper) 
    : FacadeBase<Playlist, PlaylistListModel, PlaylistDetailModel, PlaylistEntityMapper>(uowf, modelMapper),
        IPlaylistFacade
{
    private readonly IUnitOfWorkFactory _uowf = uowf;

    public async Task<IEnumerable<PlaylistListModel>> GetAsync(string searchTerm)
    {
        // Null returns each artist in db
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await base.GetAsync().ConfigureAwait(false);
        }

        string lowerSearchTerm = searchTerm.Trim().ToLower();
        Expression<Func<Playlist, bool>> predicate = lamb => lamb.Name.ToLower().Contains(lowerSearchTerm);

        return await GetListAsync(predicate).ConfigureAwait(false);
    }
    
    public async Task AddToPlaylist(Guid playlistId, PlaylistDetailModel musicTrack)
    {
        await using var uow = _uowf.Create();
        
        var playlist = await base.GetAsync(playlistId).ConfigureAwait(false);
        if (playlist == null)
        {
            throw new ArgumentException("Playlist not found.", nameof(playlistId));
        }

        await SaveAsync(musicTrack).ConfigureAwait(false);
    }
    
    public async Task RemoveFromPlaylist(Guid playlistId, Guid musicTrackId)
    {
        await using var uow = _uowf.Create();
        
        var playlist = await base.GetAsync(playlistId).ConfigureAwait(false);
        if (playlist == null)
        {
            throw new ArgumentException("Playlist not found.", nameof(playlistId));
        }

        await DeleteAsync(musicTrackId).ConfigureAwait(false);
    }

    
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { nameof(Playlist.MusicTracks) };
}