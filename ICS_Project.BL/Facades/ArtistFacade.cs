using System.Linq.Expressions;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.UnitOfWork;

namespace ICS_Project.BL.Facades;

public class ArtistFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    IArtistModelMapper modelMapper)
    : FacadeBase<Artist, ArtistListModel, ArtistDetailModel, ArtistEntityMapper>(unitOfWorkFactory, modelMapper),
        IArtistFacade
{
    public async Task<IEnumerable<ArtistListModel>> GetAsync(string searchTerm)
    {
        // Null returns each artist in db
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await base.GetAsync().ConfigureAwait(false);
        }

        string lowerSearchTerm = searchTerm.Trim().ToLower();
        Expression<Func<Artist, bool>> predicate = artist => artist.ArtistName.ToLower().Contains(lowerSearchTerm);

        return await GetListAsync(predicate).ConfigureAwait(false);
    }
    
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { nameof(Artist.MusicTracks) };
}