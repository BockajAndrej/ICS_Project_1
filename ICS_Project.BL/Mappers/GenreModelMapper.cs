using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers;

public class GenreModelMapper : ModelMapperBase<Genre, GenreListModel, GenreDetailModel>
{
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
                GenreName = entity.GenreName
            };

    public override Genre MapToEntity(GenreDetailModel model)
        => new ()
            {
                Id = model.Id,
                GenreName = model.GenreName
            };
}