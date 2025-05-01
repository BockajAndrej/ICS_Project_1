using ICS_Project.DAL.UnitOfWork;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICS_Project.BL;
using ICS_Project.BL.Mappers.Interfaces;
using Microsoft.EntityFrameworkCore.Internal;

namespace ICS_Project.BL
{
    public static class BLInstaller
    {
        public static IServiceCollection AddBLServices(this IServiceCollection services)
        {
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

            services.Scan(selector => selector
                .FromAssemblyOf<BusinessLogic>()
                .AddClasses(filter => filter.AssignableTo(typeof(IFacade<,,>)))
                .AsMatchingInterface()
                .WithSingletonLifetime());

            services.Scan(selector => selector
                .FromAssemblyOf<BusinessLogic>()
                .AddClasses(filter => filter.AssignableTo(typeof(IModelMapper<,,>)))
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

            // Register Mappers
            services.AddTransient<IArtistModelMapper, ArtistModelMapper>();
            services.AddTransient<IMusicTrackModelMapper, MusicTrackModelMapper>();
            services.AddTransient<IGenreModelMapper, GenreModelMapper>();
            services.AddTransient<IPlaylistModelMapper, PlaylistModelMapper>();

            // Register Lazy Mappers (if needed by facades)
            services.AddTransient<Lazy<IArtistModelMapper>>(sp =>
                new Lazy<IArtistModelMapper>(() => sp.GetRequiredService<IArtistModelMapper>()));
            services.AddTransient<Lazy<IMusicTrackModelMapper>>(sp =>
                new Lazy<IMusicTrackModelMapper>(() => sp.GetRequiredService<IMusicTrackModelMapper>()));
            services.AddTransient<Lazy<IGenreModelMapper>>(sp =>
                new Lazy<IGenreModelMapper>(() => sp.GetRequiredService<IGenreModelMapper>()));
            services.AddTransient<Lazy<IPlaylistModelMapper>>(sp =>
                new Lazy<IPlaylistModelMapper>(() => sp.GetRequiredService<IPlaylistModelMapper>()));

            // Register Facades
            services.AddTransient<IArtistFacade, ArtistFacade>();
            services.AddTransient<IMusicTrackFacade, MusicTrackFacade>();
            services.AddTransient<IGenreFacade, GenreFacade>();
            services.AddTransient<IPlaylistFacade, PlaylistFacade>();

            return services;
        }
    }
}
