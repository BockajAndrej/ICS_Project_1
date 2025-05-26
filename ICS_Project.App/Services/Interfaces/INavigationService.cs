namespace ICS_Project.App.Services.Interfaces
{
    public interface INavigationService
    {
        Task GoToAsync(string route);
        Task GoToAsync(string route, IDictionary<string, object?> parameters);
        bool SendBackButtonPressed();
    }
}