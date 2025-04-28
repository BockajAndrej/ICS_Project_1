using System.Linq.Expressions;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.UnitOfWork;

namespace ICS_Project.BL.Facades;

public class GenreFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    IGenreModelMapper modelMapper)
    : FacadeBase<Genre, GenreListModel, GenreDetailModel, GenreEntityMapper>(unitOfWorkFactory, modelMapper),
        IGenreFacade
{
    public async Task<IEnumerable<GenreListModel>> GetAsync(string searchTerm)
    {
        // Null returns each artist in db
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await base.GetAsync().ConfigureAwait(false);
        }

        string lowerSearchTerm = searchTerm.Trim().ToLower();
        Expression<Func<Genre, bool>> predicate = lamb => lamb.GenreName.ToLower().Contains(lowerSearchTerm);

        return await GetListAsync(predicate).ConfigureAwait(false);
    }
    
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { nameof(Genre.MusicTracks) };
}