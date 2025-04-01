using System.Xml;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.UnitOfWork;

namespace ICS_Project.BL.Facades;

public class ArtistFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    ArtistModelMapper modelMapper)
    : FacadeBase<Artist, ArtistListModel, ArtistDetailModel, ArtistEntityMapper>(unitOfWorkFactory, modelMapper),
        IArtistFacade
{
    
}