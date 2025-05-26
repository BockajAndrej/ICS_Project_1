using ICS_Project.App.Services.Interfaces;

namespace ICS_Project.App.Services
{
    public class NavigationService : INavigationService
    {
        public async Task GoToAsync(string route)
            => await Shell.Current.GoToAsync(route);

        public async Task GoToAsync(string route, IDictionary<string, object?> parameters)
            => await Shell.Current.GoToAsync(route, parameters);

        public bool SendBackButtonPressed()
            => Shell.Current.SendBackButtonPressed();
    }
}