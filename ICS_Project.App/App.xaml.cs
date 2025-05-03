using ICS_Project.App.Views.MusicTrack;
using ICS_Project.App.Views.Playlist;
using Microsoft.Extensions.DependencyInjection;

namespace ICS_Project.App
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = serviceProvider.GetRequiredService<AppShell>();
        }
    }
}
