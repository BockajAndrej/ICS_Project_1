using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;

namespace ICS_Project.BL.Facades;

public interface IArtistFacade : IFacade<Artist, ArtistListModel, ArtistDetailModel>
{
    public Task<IEnumerable<ArtistListModel>> GetAsync(string searchTerm);
}