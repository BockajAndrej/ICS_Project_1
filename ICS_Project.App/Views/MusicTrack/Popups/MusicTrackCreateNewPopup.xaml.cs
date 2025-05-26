using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.ViewModels.MusicTrack;
using ICS_Project.App.Messages;
using ICS_Project.App.Views.Artist.Popups;
using System.Diagnostics;
using ICS_Project.App.Views.Genre.Popups;

namespace ICS_Project.App.Views.MusicTrack;

public partial class MusicTrackCreateNewPopup : Popup
{

    private readonly IServiceProvider _serviceProvider;

    public MusicTrackCreateNewPopup(MusicTrackCreateNewPopupModel musicTrackCreateNewPopupModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        BindingContext = musicTrackCreateNewPopupModel;
        WeakReferenceMessenger.Default.Register<MusicTrackNewMusicTrackClosed>(this, (r, m) =>
        {
            Close();
        });
    }
    private void MusicTrackCreateNewPopup_Opened(object sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new MusicTrackNewMusicTrackOpened(true));
    }

    private async void OnNewArtistClicked(object sender, EventArgs e)
    {
        var popup = _serviceProvider.GetRequiredService<ArtistEditView>();

        Element parent = this;
        while (parent != null && !(parent is Page))
        {
            parent = parent.Parent;
        }

        Page currentPage = parent as Page;


        if (currentPage != null)
        {
            currentPage.ShowPopup(popup);
        }
        else
        {
            Debug.WriteLine("Error: Could not find the parent Page to display the popup.");
        }
    }

    private async void OnNewGenreClicked(object sender, EventArgs e)
    {
        var popup = _serviceProvider.GetRequiredService<GenreEditView>();

        Element parent = this;
        while (parent != null && !(parent is Page))
        {
            parent = parent.Parent;
        }

        Page currentPage = parent as Page;

        if (currentPage != null)
        {
            currentPage.ShowPopup(popup);
        }
        else
        {
            Debug.WriteLine("Error: Could not find the parent Page to display the popup.");
        }
    }
}