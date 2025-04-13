using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers;

public class ArtistModelMapper : ModelMapperBase<Artist, ArtistListModel, ArtistDetailModel>, IArtistModelMapper
{
    private readonly Lazy<IMusicTrackModelMapper> _musicTrackMapperLazy;

    public ArtistModelMapper(Lazy<IMusicTrackModelMapper> musicTrackMapperLazy)
    {
        _musicTrackMapperLazy = musicTrackMapperLazy;
    }
    
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
                MusicTrack = _musicTrackMapperLazy.Value.MapToListModel(entity.MusicTracks)
                    .ToObservableCollection()
            };

    public override Artist MapToEntity(ArtistDetailModel model)
        => new()
        {
            Id = model.Id,
            ArtistName = model.ArtistName,
        };
}