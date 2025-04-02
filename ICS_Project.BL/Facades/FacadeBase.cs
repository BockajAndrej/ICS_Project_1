using System.Collections;
using System.Reflection;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.Repositories;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

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
    protected virtual ICollection<string> IncludesNavigationPathDetail => new List<string>();
    
    protected readonly IModelMapper<TEntity, TListModel, TDetailModel> ModelMapper = modelMapper;
    protected readonly IUnitOfWorkFactory UnitOfWorkFactory = unitOfWorkFactory;
    public async Task DeleteAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        try
        {
            //Perform delete with repository 
            await uow.GetRepository<TEntity, TEntityMapper>().DeleteAsync(id).ConfigureAwait(false);
            //Save changes into database
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException e)
        {
            //User can see message exception in UI, e can be processed later
            throw new InvalidOperationException("Entity deletion failed.", e);
        }
    }

    public async Task<TDetailModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        
        IQueryable<TEntity> query = uow.GetRepository<TEntity, TEntityMapper>().Get();

        //Include associated relationships 
        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }
        
        //Find entity by id
        TEntity? entity = await query.SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
        
        //If intity was found make a conversion 
        return entity is null
            ? null
            : ModelMapper.MapToDetailModel(entity);
    }

    public async Task<IEnumerable<TListModel>> GetAsync()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        List<TEntity> entities = await uow
            .GetRepository<TEntity, TEntityMapper>()
            .Get()
            .ToListAsync().ConfigureAwait(false);

        return ModelMapper.MapToListModel(entities);
    }

    public async Task<TDetailModel> SaveAsync(TDetailModel model)
    {
        TDetailModel result;

        GuardCollectionsAreNotSet(model);

        TEntity entity = ModelMapper.MapToEntity(model);

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<TEntity> repository = uow.GetRepository<TEntity, TEntityMapper>();

        if (await repository.ExistsAsync(entity).ConfigureAwait(false))
        {
            TEntity updatedEntity = await repository.UpdateAsync(entity).ConfigureAwait(false);
            result = ModelMapper.MapToDetailModel(updatedEntity);
        }
        else
        {
            entity.Id = Guid.NewGuid();
            TEntity insertedEntity = repository.Insert(entity);
            result = ModelMapper.MapToDetailModel(insertedEntity);
        }

        await uow.CommitAsync().ConfigureAwait(false);

        return result;
    }
    
    private static void GuardCollectionsAreNotSet(TDetailModel model)
    {
        IEnumerable<PropertyInfo> collectionProperties = model
            .GetType()
            .GetProperties()
            .Where(i => typeof(ICollection).IsAssignableFrom(i.PropertyType));

        foreach (PropertyInfo collectionProperty in collectionProperties)
        {
            if (collectionProperty.GetValue(model) is ICollection { Count: > 0 })
            {
                throw new InvalidOperationException(
                    "Current BL and DAL infrastructure disallows insert or update of models with adjacent collections.");
            }
        }
    }
}