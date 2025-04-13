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
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { nameof(Playlist.MusicTracks) };
}