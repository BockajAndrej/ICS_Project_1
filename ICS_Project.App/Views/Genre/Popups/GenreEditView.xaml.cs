using CommunityToolkit.Maui.Views;
using System.Diagnostics;
using CommunityToolkit.Maui.Views; // Make sure this using is present
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages; // Often needed for enums like PlacementMode
using ICS_Project.App.ViewModels.Genre;


namespace ICS_Project.App.Views.Genre.Popups;

public partial class GenreEditView : Popup
{
    private readonly IServiceProvider _serviceProvider;
    public GenreEditView(
        GenreEditViewModel genreEditViewModel,
        IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        BindingContext = genreEditViewModel; // Set BindingContext
        WeakReferenceMessenger.Default.Register<GenreEditViewClosed>(this, (r, m) =>
        {
            Close(); // closes the popup
        });
    }
}