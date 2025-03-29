using Microsoft.EntityFrameworkCore;

namespace ICS_Project.DAL.UnitOfWork;

public class UnitOfWorkFactory(IDbContextFactory<MusicDbContext> dbContextFactory) : IUnitOfWorkFactory
{
    public IUnitOfWork Create() => new UnitOfWork(dbContextFactory.CreateDbContext());
}