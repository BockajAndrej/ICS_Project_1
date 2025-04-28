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
    
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { nameof(Playlist.MusicTracks) };
}