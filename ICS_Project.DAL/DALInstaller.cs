using ICS_Project.DAL.Factories;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.Migrations;
using ICS_Project.DAL.Options;
using ICS_Project.DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICS_Project.DAL
{
    public static class DALInstaller
    {
        public static IServiceCollection addDALServises(this IServiceCollection services)
        {
            services.AddSingleton<IDbContextFactory<MusicDbContext>>(serviceProvider =>
            {
                var dalOptions = serviceProvider.GetRequiredService<IOptions<DALOptions>>();
                return new DbContextSqLiteFactory(dalOptions.Value.DatabaseFilePath);
            });

            services.AddSingleton<IDbMigrator, DbMigrator>();
            services.AddSingleton<IDbSeeder, DbSeeder>();

            services.AddSingleton<ArtistEntityMapper>();
            services.AddSingleton<GenreEntityMapper>();
            services.AddSingleton<MusicTrackEntityMapper>();
            services.AddSingleton<PlaylistEntityMapper>();

            return services;
        }
    }
}
