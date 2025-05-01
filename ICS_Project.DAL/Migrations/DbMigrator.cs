using ICS_Project.DAL.Options;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICS_Project.DAL.Migrations
{
    public class DbMigrator(IDbContextFactory<MusicDbContext> dbContextFactory, IOptions<DALOptions> options)
    : IDbMigrator
    {
        public void Migrate()
        {
            using MusicDbContext dbContext = dbContextFactory.CreateDbContext();

            if (options.Value.RecreateDatabaseEachTime)
            {
                dbContext.Database.EnsureDeleted();
            }

            // Ensures that database is created applying the latest state
            // Application of migration later on may fail
            // If you want to use migrations, you should create database by calling  dbContext.Database.Migrate() instead
            dbContext.Database.EnsureCreated();
        }
    }
}
