using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Repositories;
using ICS_Project.DAL.Mappers;

namespace ICS_Project.DAL.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<TEntity> GetRepository<TEntity, TEntityMapper>()
        where TEntity : class, IEntity
        where TEntityMapper : IEntityMapper<TEntity>, new();

    Task CommitAsync();
}