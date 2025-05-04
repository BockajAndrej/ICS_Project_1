using ICS_Project.App.Services.Interfaces;

namespace ICS_Project.App.Services // Use your namespace
{
    public class NavigationService : INavigationService
    {
        // Define routes later if using Shell Navigation extensively
        // public IEnumerable<RouteModel> Routes { get; } = new List<RouteModel> { ... };

        public async Task GoToAsync(string route)
            => await Shell.Current.GoToAsync(route);

        public async Task GoToAsync(string route, IDictionary<string, object?> parameters)
            => await Shell.Current.GoToAsync(route, parameters);

        public bool SendBackButtonPressed()
            => Shell.Current.SendBackButtonPressed();
    }
}