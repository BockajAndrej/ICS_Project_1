using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.App.ViewModels.Artist;


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
        BindingContext = artistEditViewModel;
        WeakReferenceMessenger.Default.Register<ArtistEditViewClosed>(this, (r, m) =>
        {
            Close();
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