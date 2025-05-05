using CommunityToolkit.Maui.Views;
using System.Diagnostics;
using CommunityToolkit.Maui.Views; // Make sure this using is present
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages; // Often needed for enums like PlacementMode
using ICS_Project.App.ViewModels.Playlist; // <<< Add this using


namespace ICS_Project.App.Views.Playlist.Popups;

public partial class PlaylistOptionsView : Popup
{
    private readonly IServiceProvider _serviceProvider;
    public PlaylistOptionsView(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }


    // Handler for the Edit button
    private void EditButton_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Edit button clicked!");
        var popup = _serviceProvider.GetRequiredService<PlaylistCreateNewPopup>();

        WeakReferenceMessenger.Default.Send(new PlaylistPopupContext { IsEditMode = true });

        WeakReferenceMessenger.Default.Send(new PlaylistRequestGUID());

        // --- Find the parent Page ---
        // We need to traverse up the visual tree from the ContentView ('this')
        // until we find the Page that contains it.
        Element parent = this;
        while (parent != null && !(parent is Page))
        {
            parent = parent.Parent; // Go up one level
        }

        // Cast the found parent element to a Page
        Page currentPage = parent as Page;
        // --- End Finding Page ---


        // --- Show the Popup using the Page context ---
        if (currentPage != null)
        {

            // Now call ShowPopup on the Page instance.
            // The toolkit will use the Anchor (if set) and the Page context
            // to display the popup correctly.
            currentPage.ShowPopup(popup);
        }
        else
        {
            // Should not happen in a normal MAUI app structure, but good to handle.
            Debug.WriteLine("Error: Could not find the parent Page to display the popup.");
            // Maybe show an alert or log this error properly.
        }
        // --- End Showing Popup ---
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