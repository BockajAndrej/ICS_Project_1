﻿using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.DAL.Repositories;

public class Repository<TEntity>(
    DbContext dbContext,
    IEntityMapper<TEntity> mapper)
    : IRepository<TEntity> where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Get() => _dbSet;

    public async ValueTask<bool> ExistsAsync(TEntity entity)
        => entity.Id != Guid.Empty && await _dbSet.AnyAsync(e => e.Id == entity.Id).ConfigureAwait(false);
    
    public TEntity Insert(TEntity entity)
        => _dbSet.Add(entity).Entity;
    
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        TEntity existingEntity = await _dbSet.SingleAsync(e => e.Id == entity.Id).ConfigureAwait(false);
        mapper.MapToExistingEntity(existingEntity, entity);
        return entity;
    }
    
    public async Task DeleteAsync(Guid entityId)
        => _dbSet.Remove(await _dbSet.SingleAsync(i => i.Id == entityId).ConfigureAwait(false));
}