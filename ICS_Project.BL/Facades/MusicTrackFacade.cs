using System.Linq.Expressions;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.UnitOfWork;

namespace ICS_Project.BL.Facades;

public class MusicTrackFacade(
    IUnitOfWorkFactory uowf,
    IMusicTrackModelMapper modelMapper)
    : FacadeBase<MusicTrack, MusicTrackListModel, MusicTrackDetailModel, MusicTrackEntityMapper>(uowf, modelMapper),
        IMusicTrackFacade
{
    public async Task<IEnumerable<MusicTrackListModel>> GetAsync(string searchTerm)
    {
        // Null returns each artist in db
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await base.GetAsync().ConfigureAwait(false);
        }

        string lowerSearchTerm = searchTerm.Trim().ToLower();
        Expression<Func<MusicTrack, bool>> predicate = lamb => lamb.Title.ToLower().Contains(lowerSearchTerm);

        return await GetListAsync(predicate).ConfigureAwait(false);
    }
    
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[]
        {
            nameof(MusicTrack.Artists),
            nameof(MusicTrack.Playlists),
            nameof(MusicTrack.Genres)
        };
}