using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
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
        BindingContext = genreEditViewModel;
        WeakReferenceMessenger.Default.Register<GenreEditViewClosed>(this, (r, m) =>
        {
            Close();
        });

        this.Opened += GenreEditView_Opened;
    }

    private void GenreEditView_Opened(object sender, PopupOpenedEventArgs e)
    {
        if (GenreNameEntry != null)
        {
            GenreNameEntry.Focus();
        }
    }
}