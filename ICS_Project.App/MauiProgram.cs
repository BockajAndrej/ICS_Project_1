using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.Views.Playlist;
using ICS_Project.BL.Facades;
using ICS_Project.BL;
using Microsoft.Extensions.Logging;
using ICS_Project.DAL;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Mappers;
using ICS_Project.DAL.Seeds;
using ICS_Project.DAL.Migrations;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging; // Needed for IMessenger
using ICS_Project.App.Services;        // Needed for your services
using ICS_Project.App.Services.Interfaces;
using ICS_Project.App.Views.MusicTrack;
using ICS_Project.App.ViewModels.MusicTrack; // Needed for your interfaces
using ICS_Project.App.Services.Interfaces;
using ICS_Project.App.Views.Playlist.Popups; // Needed for your interfaces
using ICS_Project.App.Services.Popups;
using ICS_Project.App.Views.MusicTrack.Popups; // <-- Make sure this using is here
using ICS_Project.DAL.Entities;

namespace ICS_Project.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IPopupService, Services.Popups.PopupService>(); 
        builder.Services.AddTransient<PlaylistView>();
        builder.Services.AddTransient<MusicTrackView>();


        builder.Services.AddTransient<PlaylistListViewModel>();
        builder.Services.AddTransient<PlaylistDetailViewModel>();

        builder.Services.AddTransient<MusicTrackListViewModel>();
        builder.Services.AddTransient<MusicTrackDetailViewModel>();
        builder.Services.AddTransient<MusicTrackEditView>(); 

        builder.Services.AddTransient<MusicTrackDetailView>(); 

        builder.Services.AddTransient<PlaylistCreateNewPopup>();
        builder.Services.AddTransient<PlaylistCreateNewPopupModel>();

        builder.Services.AddTransient<PlaylistOptionsView>();

        builder.Services
            .addDALServises()
            .AddBLServices()
            .AddAppServices();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        MigrateDb(app.Services.GetRequiredService<IDbMigrator>());
        SeedDb(app.Services.GetRequiredService<IDbSeeder>());

        return app;
    }


    private static void MigrateDb(IDbMigrator migrator) => migrator.Migrate();
    private static void SeedDb(IDbSeeder dbSeeder) => dbSeeder.Seed();
}
