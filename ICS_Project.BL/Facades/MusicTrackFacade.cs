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
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[]
        {
            nameof(MusicTrack.Artists),
            nameof(MusicTrack.Playlists),
            nameof(MusicTrack.Genres)
        };
}