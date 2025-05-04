using ICS_Project.App.Services.Interfaces;

namespace ICS_Project.App.Services
{
    public class AlertService : IAlertService
    {
        // Try to get the current MainPage safely
        private Page? CurrentMainPage => Application.Current?.MainPage;

        public async Task DisplayAsync(string title, string message)
        {
            // Check if MainPage is available before calling DisplayAlert
            if (CurrentMainPage is not null)
            {
                await CurrentMainPage.DisplayAlert(title, message, "OK");
            }
            else
            {
                // Log error or handle case where MainPage isn't ready/available
                System.Diagnostics.Debug.WriteLine($"AlertService Error: MainPage is null. Cannot display alert '{title}'.");
            }
        }
    }
}