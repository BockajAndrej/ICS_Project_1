using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICS_Project.App
{
    public static class AppInstaller
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            //services.AddSingleton<AppShell>();

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
