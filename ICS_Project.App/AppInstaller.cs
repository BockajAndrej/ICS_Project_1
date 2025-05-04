using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICS_Project.App.Extensions;
using ICS_Project.App.Services;             // For service implementations
using ICS_Project.App.Services.Interfaces;  // For service interfaces
using ICS_Project.App.ViewModels.Playlist;  // For ViewModels
using ICS_Project.App.Views.Playlist;       // For Views

namespace ICS_Project.App
{
    public static class AppInstaller
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddSingleton<AppShell>();

            services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);

            services.AddSingleton<IMessengerService, MessengerService>();
            services.AddSingleton<IAlertService, AlertService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // --- Call the new extension methods ---
            services.AddAppViews();       // << CALL THIS
            services.AddAppViewModels();  // << CALL THIS
            // --------------------------------------


            //services.AddSingleton<IMessenger>(_ => WeakReferenceMessenger.Default);

            //services.AddSingleton<IMessengerService, MessengerService>();
            //services.AddSingleton<INavigationService, NavigationService>();
            //services.AddSingleton<IAlertService, AlertService>();

            //services.AddViews();
            //services.AddViewModels();

            return services;
        }
    }
}
