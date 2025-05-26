using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Extensions;
using ICS_Project.App.Services;
using ICS_Project.App.Services.Interfaces;

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

            services.AddAppViews();
            services.AddAppViewModels();

            return services;
        }
    }
}
