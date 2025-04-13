using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers.Interfaces;

public interface IArtistModelMapper : IModelMapper<Artist, ArtistListModel, ArtistDetailModel>
{
    
}