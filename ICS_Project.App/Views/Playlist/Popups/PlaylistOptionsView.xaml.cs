using CommunityToolkit.Maui.Views;
using System.Diagnostics;
using CommunityToolkit.Maui.Views; // Make sure this using is present
using CommunityToolkit.Maui.Core; // Often needed for enums like PlacementMode
using ICS_Project.App.ViewModels.Playlist; // <<< Add this using


namespace ICS_Project.App.Views.Playlist.Popups;

public partial class PlaylistOptionsView : Popup
{
    public PlaylistOptionsView()
    {
        InitializeComponent();
    }


    // Handler for the Edit button
    private void EditButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Edit button clicked!");
        // In a real app, you would pass data back or trigger an action
        // For now, just close the popup
        this.Close(); // Close the popup
    }

    // Handler for the Delete button
    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Delete button clicked!");
        // In a real app, you would pass data back or trigger an action
        // For now, just close the popup
        this.Close(); // Close the popup
    }
}