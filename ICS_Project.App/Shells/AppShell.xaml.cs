using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;        
using ICS_Project.App.Services.Interfaces;
using ICS_Project.App.Services.Popups;
using ICS_Project.App.Views.Playlist.Popups; 
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection; 


namespace ICS_Project.App; 

// Implement IRecipient for the specific message
public partial class AppShell : Shell
{
    private readonly IMessengerService _messengerService;


    private readonly IPopupService _popupService;

    // Inject IMessengerService
    public AppShell(IMessengerService messengerService, IServiceProvider serviceProvider, IPopupService popupService)
    {
        InitializeComponent();
        _messengerService = messengerService;
        _popupService = popupService; 
        
        _messengerService.Messenger.Register<PlaylistShowOptions>(this, (recipient, message) =>
        {
            Debug.WriteLine("--- AppShell: Received PlaylistShowOptions message, calling PopupService ---");

            _popupService.ShowPopup(
                typeof(PlaylistOptionsView),
                message.ViewModel, 
                message.Anchor 
            );
        });

    }
}