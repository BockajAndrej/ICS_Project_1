using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers;

public class GenreModelMapper 
    : ModelMapperBase<Genre, GenreListModel, GenreDetailModel>, IGenreModelMapper
{
    private readonly Lazy<IMusicTrackModelMapper> _musicTrackMapperLazy;

    public GenreModelMapper(Lazy<IMusicTrackModelMapper> musicTrackMapperLazy)
    {
        _musicTrackMapperLazy = musicTrackMapperLazy;
    }
    public override GenreListModel MapToListModel(Genre? entity)
        => entity is null
            ? GenreListModel.Empty
            : new GenreListModel
            {
                Id = entity.Id,
                GenreName = entity.GenreName
            };

    public override GenreDetailModel MapToDetailModel(Genre? entity)
        => entity is null
            ? GenreDetailModel.Empty
            : new GenreDetailModel
            {
                Id = entity.Id,
                GenreName = entity.GenreName,
                MusicTracks = _musicTrackMapperLazy.Value.MapToListModel(entity.MusicTracks)
                    .ToObservableCollection()
            };

    public override Genre MapToEntity(GenreDetailModel model)
        => new ()
            {
                Id = model.Id,
                GenreName = model.GenreName
            };
}