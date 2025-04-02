using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;

namespace ICS_Project.BL.Mappers;

public abstract class ModelMapperBase<TEntity, TListModel, TDetailModel> : IModelMapper<TEntity, TListModel, TDetailModel>
{
    public abstract TListModel MapToListModel(TEntity? entity);
    public abstract TDetailModel MapToDetailModel(TEntity? entity);
    public abstract TEntity MapToEntity(TDetailModel model);
}