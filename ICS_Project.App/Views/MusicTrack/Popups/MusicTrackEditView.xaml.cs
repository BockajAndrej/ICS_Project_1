using ICS_Project.App.ViewModels.MusicTrack;
using CommunityToolkit.Maui.Views;
using ICS_Project.App.ViewModels.MusicTrack;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;

namespace ICS_Project.App.Views.MusicTrack.Popups;

public partial class MusicTrackEditView : Popup
{
    private readonly IServiceProvider _serviceProvider;
    public MusicTrackEditView(MusicTrackDetailViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _serviceProvider = serviceProvider;
    }

    private void EditButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Edit button clicked!");

        var popup = _serviceProvider.GetRequiredService<MusicTrackCreateNewPopup>();

        WeakReferenceMessenger.Default.Send(new MusicTrackPopupContext { IsEditMode = true });

        WeakReferenceMessenger.Default.Send(new MusicTrackRequestGUID());

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

    private void ClosePopup(object sender, EventArgs e)
    {
        Debug.WriteLine("ClosePopup: Closing popup after button click.");
        this.Close();
    }
}