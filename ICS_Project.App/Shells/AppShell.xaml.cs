// In AppShell.xaml.cs
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;           // Your message namespace
using ICS_Project.App.Services.Interfaces; // Your service interfaces
using ICS_Project.App.Views.Playlist.Popups; // Your popup namespace
using System.Diagnostics;

namespace ICS_Project.App; // Your App namespace

// Implement IRecipient for the specific message
public partial class AppShell : Shell, IRecipient<PlaylistShowOptions>
{
    private readonly IMessengerService _messengerService;

    private readonly IServiceProvider _serviceProvider;
    // Inject IMessengerService
    public AppShell(IMessengerService messengerService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _messengerService = messengerService;
        _serviceProvider = serviceProvider;

        // Register AppShell itself to receive the message
        Debug.WriteLine("--- AppShell: ABOUT TO REGISTER for PlaylistShowOptions ---"); // <<< ADD THIS
        _messengerService.Messenger.Register<PlaylistShowOptions>(this);
        Debug.WriteLine("--- AppShell: SUCCESSFULLY REGISTERED for PlaylistShowOptions ---"); // <<< ADD THIS

        // *** Add Routing Registration Here if not done elsewhere ***
        // RegisterRoutes(); // Call a method to register your app's routes
    }

    // Handle the received message
    public void Receive(PlaylistShowOptions message)
    {
        Debug.WriteLine($"--- AppShell: Received PlaylistShowOptions message on Thread {Environment.CurrentManagedThreadId} ---");

        // Ensure MainThread for UI operations
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Debug.WriteLine($"--- AppShell: Executing Receive logic on MainThread {Environment.CurrentManagedThreadId} ---");

            // Basic validation
            if (message.ViewModel is null)
            {
                Debug.WriteLine($"--- AppShell: ERROR - Received message without required ViewModel. ---");
                return;
            }
            Debug.WriteLine($"--- AppShell: Message ViewModel Type: {message.ViewModel.GetType().Name} ---");

            if (message.Anchor is null)
            {
                Debug.WriteLine($"--- AppShell: ERROR - Received message without required Anchor. ---");
                return;
            }
            Debug.WriteLine($"--- AppShell: Message Anchor Type: {message.Anchor.GetType().Name} ---");

            Debug.WriteLine("--- AppShell: Creating PlaylistOptionsView popup instance ---");
            var popup = new PlaylistOptionsView(_serviceProvider);

            Debug.WriteLine("--- AppShell: Setting popup BindingContext ---");
            popup.BindingContext = message.ViewModel;

            Debug.WriteLine("--- AppShell: Setting popup Anchor ---");
            // Set the anchor (casting still needed)
            try
            {
                popup.Anchor = (View)message.Anchor;
                Debug.WriteLine("--- AppShell: Popup Anchor set successfully. ---");
            }
            catch (InvalidCastException ex)
            {
                Debug.WriteLine($"--- AppShell: ERROR - Failed to cast Anchor to View. Anchor Type: {message.Anchor.GetType().Name}. Exception: {ex.Message} ---");
                return; // Can't anchor if cast fails
            }


            Debug.WriteLine("--- AppShell: Getting Current Page ---");
            Page? currentPage = Shell.Current?.CurrentPage;

            if (currentPage != null)
            {
                Debug.WriteLine($"--- AppShell: CurrentPage found: {currentPage.GetType().Name}. Attempting to show popup. ---");
                currentPage.ShowPopup(popup);
                Debug.WriteLine("--- AppShell: ShowPopup called. ---");
            }
            else
            {
                Debug.WriteLine($"--- AppShell: ERROR - Shell.Current.CurrentPage is null. Cannot display popup. ---");
            }
            Debug.WriteLine("--- AppShell: Finished processing Receive logic on MainThread ---");
        });
    }

    // Optional: Unregister if AppShell can be disposed (less common for root shell)
    // Consider implementing IDisposable if necessary, though Shell is usually lifetime
    // public void Dispose()
    // {
    //    _messengerService.Messenger.Unregister<ShowPlaylistOptionsForDetailMessage>(this);
    //    // Dispose logic
    // }

    // Example Routing Registration Method (if not done elsewhere like MauiProgram)
    // private void RegisterRoutes()
    // {
    //     Routing.RegisterRoute(nameof(Views.Playlist.PlaylistView), typeof(Views.Playlist.PlaylistView));
    //     Routing.RegisterRoute(nameof(Views.Playlist.PlaylistDetailView), typeof(Views.Playlist.PlaylistDetailView));
    //     // Add other routes...
    // }
}