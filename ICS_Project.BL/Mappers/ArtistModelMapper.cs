using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers;

public class ArtistModelMapper(MusicTrackModelMapper musicTrackMapper) : ModelMapperBase<Artist, ArtistListModel, ArtistDetailModel>
{
    public override ArtistListModel MapToListModel(Artist? entity)
        => entity is null
            ? ArtistListModel.Empty
            : new ArtistListModel
            {
                Id = entity.Id,
                ArtistName = entity.ArtistName,
            };

    public override ArtistDetailModel MapToDetailModel(Artist? entity)
        => entity is null
            ? ArtistDetailModel.Empty
            : new ArtistDetailModel
            {
                Id = entity.Id,
                ArtistName = entity.ArtistName,
                MusicTrack = musicTrackMapper.MapToListModel(entity.MusicTracks)
                    .ToObservableCollection()
            };

    public override Artist MapToEntity(ArtistDetailModel model)
        => new()
        {
            Id = model.Id,
            ArtistName = model.ArtistName,
        };
}