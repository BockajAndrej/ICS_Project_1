using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Facades;

public interface IGenreFacade : IFacade<Genre, GenreListModel, GenreDetailModel>
{
    public Task<IEnumerable<GenreListModel>> GetAsync(string searchTerm);
}