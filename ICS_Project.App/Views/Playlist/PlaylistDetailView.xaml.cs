using CommunityToolkit.Maui.Views;
using ICS_Project.App.ViewModels.Playlist;
using System.Diagnostics;
using ICS_Project.App.Views.Playlist.Popups;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging; // Needed for IMessenger and registration
using ICS_Project.App.Messages; // Needed for your custom message
using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.Views.Playlist.Popups;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;           // Needed
using ICS_Project.App.Messages;                 // Needed
using ICS_Project.App.Services.Interfaces;      // Needed
using ICS_Project.App.ViewModels.Playlist;      // Needed
using ICS_Project.App.Views.Playlist.Popups;    // Needed

namespace ICS_Project.App.Views.Playlist;

public partial class PlaylistDetailView : ContentView
{
    public PlaylistDetailView(PlaylistDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Set BindingContext
    }
}