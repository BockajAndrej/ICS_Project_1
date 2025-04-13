using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers;

public class PlaylistModelMapper(MusicTrackModelMapper musicTrackMapper) : ModelMapperBase<Playlist, PlaylistListModel, PlaylistDetailModel>
{
    public override PlaylistListModel MapToListModel(Playlist? entity)
        => entity is null
            ? PlaylistListModel.Empty
            : new PlaylistListModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                NumberOfMusicTracks = entity.NumberOfMusicTracks,
                TotalPlayTime = entity.TotalPlayTime,
            };

    public override PlaylistDetailModel MapToDetailModel(Playlist? entity)
        => entity is null
            ? PlaylistDetailModel.Empty
            : new PlaylistDetailModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                NumberOfMusicTracks = entity.NumberOfMusicTracks,
                TotalPlayTime = entity.TotalPlayTime,
                MusicTracks = musicTrackMapper.MapToListModel(entity.MusicTracks)
                    .ToObservableCollection()
            };

    public override Playlist MapToEntity(PlaylistDetailModel model)
        => new()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                NumberOfMusicTracks = model.NumberOfMusicTracks,
                TotalPlayTime = model.TotalPlayTime,
            };
}