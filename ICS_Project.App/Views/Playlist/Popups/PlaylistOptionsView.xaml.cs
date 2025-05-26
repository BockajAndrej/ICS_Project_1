using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using System.Diagnostics;


namespace ICS_Project.App.Views.Playlist.Popups;

public partial class PlaylistOptionsView : Popup
{
    private readonly IServiceProvider _serviceProvider;
    public PlaylistOptionsView(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Edit button clicked!");
        var popup = _serviceProvider.GetRequiredService<PlaylistCreateNewPopup>();

        WeakReferenceMessenger.Default.Send(new PlaylistPopupContext { IsEditMode = true });

        WeakReferenceMessenger.Default.Send(new PlaylistRequestGUID());

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
        this.Close();
    }


    private void DeleteButton_AndClose_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("DeleteButton_AndClose_Clicked: Zatváram popup po príkaze.");
        this.Close();
    }
}