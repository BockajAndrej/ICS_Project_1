using ICS_Project.BL.Facades.Interfaces;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ICS_Project.BL.Facades;

public abstract class FacadeBase<TEntity, TListModel, TDetailModel, TEntityMapper>(
    IUnitOfWorkFactory unitOfWorkFactory,
    IModelMapper<TEntity, TListModel, TDetailModel> modelMapper)
    : IFacade<TEntity, TListModel, TDetailModel>
    where TEntity : class, IEntity
    where TListModel : IModel
    where TDetailModel : class, IModel
    where TEntityMapper : IEntityMapper<TEntity>, new()
{
    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<TDetailModel?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TListModel>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TDetailModel> SaveAsync(TDetailModel model)
    {
        throw new NotImplementedException();
    }
}