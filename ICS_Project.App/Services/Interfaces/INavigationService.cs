// using ICS_Project.App.Models; // If you create a RouteModel later

namespace ICS_Project.App.Services.Interfaces // Use your namespace
{
    public interface INavigationService
    {
        // IEnumerable<RouteModel> Routes { get; } // Add later if needed

        Task GoToAsync(string route);
        Task GoToAsync(string route, IDictionary<string, object?> parameters);
        bool SendBackButtonPressed(); // Useful for hardware/shell back buttons
    }
}