using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // For VisualElement, View
using Microsoft.Maui.Primitives; // For PopupPlacement
using CommunityToolkit.Maui.Views; // For Popup (optional, could use object)
using System;

namespace ICS_Project.App.Services.Interfaces
{

    // Define the interface for the Popup Service
    public interface IPopupService
    {
        // A generic method to show any popup
        // Takes the Type of the popup View, its BindingContext, anchor, and placement
        void ShowPopup(Type popupViewType, object? bindingContext = null, VisualElement? anchor = null);

        // Optional: Specific methods for clarity
        // void ShowPlaylistOptions(object? bindingContext = null, VisualElement? anchor = null, PopupPlacement? placement = null);
        // void ShowConfirmDialog(object? bindingContext = null, VisualElement? anchor = null, PopupPlacement? placement = null);
    }

}
