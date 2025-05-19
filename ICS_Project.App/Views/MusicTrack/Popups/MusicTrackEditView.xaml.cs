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

        // Handler for the Edit button
        private void EditButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Edit button clicked!");
       
        throw new NotImplementedException("Edit functionality is not implemented yet.");
    }
}