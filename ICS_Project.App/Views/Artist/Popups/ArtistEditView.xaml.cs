using CommunityToolkit.Maui.Views;
using System.Diagnostics;
using CommunityToolkit.Maui.Views; // Make sure this using is present
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages; // Often needed for enums like PlacementMode
using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.ViewModels.Artist; // <<< Add this using
using Microsoft.Maui.Controls; // For Entry


namespace ICS_Project.App.Views.Artist.Popups;

public partial class ArtistEditView : Popup
{
    private readonly IServiceProvider _serviceProvider;
    public ArtistEditView(
        ArtistEditViewModel artistEditViewModel,
        IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        BindingContext = artistEditViewModel; // Set BindingContext
        WeakReferenceMessenger.Default.Register<ArtistEditViewClosed>(this, (r, m) =>
        {
            Close(); // closes the popup
        });

        this.Opened += ArtistEditView_Opened;
    }

    private void ArtistEditView_Opened(object sender, PopupOpenedEventArgs e)
    {
        if (ArtistNameEntry != null)
        {
            ArtistNameEntry.Focus();
        }
    }
}