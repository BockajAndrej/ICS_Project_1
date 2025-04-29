using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Facades;

public interface IMusicTrackFacade : IFacade<MusicTrack, MusicTrackListModel, MusicTrackDetailModel>
{
    public Task<IEnumerable<MusicTrackListModel>> GetAsync(string searchTerm);
}