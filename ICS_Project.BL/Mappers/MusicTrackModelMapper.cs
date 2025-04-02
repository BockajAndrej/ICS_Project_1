using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers;

public class MusicTrackModelMapper : ModelMapperBase<MusicTrack, MusicTrackListModel, MusicTrackDetailModel>
{
    public override MusicTrackListModel MapToListModel(MusicTrack? entity)
        => entity is null
            ? MusicTrackListModel.Empty
            : new MusicTrackListModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Length = entity.Length,
                Size = entity.Size,
                UrlAddress = entity.UrlAddress,
            };

    public override MusicTrackDetailModel MapToDetailModel(MusicTrack? entity)
        => entity is null
            ? MusicTrackDetailModel.Empty
            : new MusicTrackDetailModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Length = entity.Length,
                Size = entity.Size,
                UrlAddress = entity.UrlAddress,
            };

    public override MusicTrack MapToEntity(MusicTrackDetailModel model)
        => new()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Length = model.Length,
                Size = model.Size,
                UrlAddress = model.UrlAddress,
            };
}