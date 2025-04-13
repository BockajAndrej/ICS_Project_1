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
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { nameof(Genre.MusicTracks) };
}