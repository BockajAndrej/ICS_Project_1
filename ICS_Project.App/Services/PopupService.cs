using Microsoft.Maui.Controls;
using Microsoft.Maui.Primitives;
using CommunityToolkit.Maui.Views;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using ICS_Project.App.Services.Interfaces;


namespace ICS_Project.App.Services.Popups
{
    public class PopupService : IPopupService
    {
        private readonly IServiceProvider _serviceProvider;

        // Inject the ServiceProvider so the service can create popups
        public PopupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // The generic method to show a popup
        public void ShowPopup(Type popupViewType, object? bindingContext = null, VisualElement? anchor = null)
        {
            // Ensure UI operations are on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine($"--- PopupService: Attempting to show popup of type {popupViewType.Name} ---");

                // 1. Get the popup instance from the service provider
                // Using GetService returns null if not found, safer than GetRequiredService here
                if (_serviceProvider.GetService(popupViewType) is not Popup popup)
                {
                    Debug.WriteLine($"--- PopupService: ERROR - Could not resolve popup instance of type {popupViewType.Name}. Is it registered in MauiProgram.cs? ---");
                    return;
                }


                // 2. Set the BindingContext if provided
                if (bindingContext != null)
                {
                    popup.BindingContext = bindingContext;
                    Debug.WriteLine($"--- PopupService: Set BindingContext for {popupViewType.Name} ---");
                }


                // 3. Set the Anchor and Placement if Anchor is provided
                if (anchor != null)
                {
                    try
                    {
                        // The Anchor property expects a View
                        popup.Anchor = (View)anchor;
                        Debug.WriteLine($"--- PopupService: Set Anchor for {popupViewType.Name} to {anchor.GetType().Name} ---");


                    }
                    catch (InvalidCastException ex)
                    {
                        Debug.WriteLine($"--- PopupService: InvalidCastException setting Anchor for {popupViewType.Name}: {ex.Message}. Popup will show without anchor. ---");
                        // Continue showing the popup, but without the anchor
                    }
                }


                // 4. Find the current Page and show the popup
                // The PopupService needs access to Shell.Current or the current page somehow.
                // The simplest way is via Shell.Current if using Shell.
                Page? currentPage = Shell.Current?.CurrentPage;

                if (currentPage != null)
                {
                    Debug.WriteLine($"--- PopupService: Showing popup {popupViewType.Name} on page: {currentPage.GetType().Name} ---");
                    currentPage.ShowPopup(popup); // Show using the page context
                }
                else
                {
                    Debug.WriteLine($"--- PopupService: ERROR - Shell.Current.CurrentPage is null. Cannot display popup {popupViewType.Name}. ---");
                }
            });
        }

        // Optional: Implement specific methods as wrappers around the generic one
        // These make the call site (e.g., the AppShell handler) slightly cleaner
        // public void ShowPlaylistOptions(object? bindingContext = null, VisualElement? anchor = null, PopupPlacement? placement = null)
        // {
        //     ShowPopup(typeof(PlaylistOptionsView), bindingContext, anchor, placement ?? PopupPlacement.Bottom); // Default playlist options to Bottom
        // }

        // public void ShowConfirmDialog(object? bindingContext = null)
        // {
        //      ShowPopup(typeof(ConfirmDialogPopup), bindingContext, null, PopupPlacement.Center); // Default confirm dialog to Center, no anchor
        // }
    }
}