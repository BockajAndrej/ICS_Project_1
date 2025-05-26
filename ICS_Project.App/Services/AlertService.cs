using ICS_Project.App.Services.Interfaces;

namespace ICS_Project.App.Services
{
    public class AlertService : IAlertService
    {
        private Page? CurrentMainPage => Application.Current?.MainPage;

        public async Task DisplayAsync(string title, string message)
        {
            if (CurrentMainPage is not null)
            {
                await CurrentMainPage.DisplayAlert(title, message, "OK");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"AlertService Error: MainPage is null. Cannot display alert '{title}'.");
            }
        }
    }
}