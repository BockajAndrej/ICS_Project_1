using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers;

public class ArtistModelMapper() : ModelMapperBase<Artist, ArtistListModel, ArtistDetailModel>
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
            };

    public override Artist MapToEntity(ArtistDetailModel model)
        => new()
        {
            Id = model.Id,
            ArtistName = model.ArtistName,
        };
}