using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System.Diagnostics;

namespace ICS_Project.App.ViewModels.Artist;

public partial class ArtistEditViewModel : ObservableObject
{
    private readonly IArtistFacade _facade;

    [ObservableProperty]
    private ArtistDetailModel _artistDetail;

    [ObservableProperty]
    private string _artistName = "";

    public async Task InitializeAsync(Guid id)
    {
        ArtistDetail = await _facade.GetAsync(id);
    }

    public ArtistEditViewModel(IArtistFacade artistFacade)
    {
        _facade = artistFacade;

        // there were number of tracks and something in OG. Are we missing something here aswell?

        ArtistDetail = new ArtistDetailModel
        {
            ArtistName = "",
        };
    }

    // Save music track command
    [RelayCommand]
    public async void SaveChanges()
    {
        // Constraints
        if (ArtistName == "")
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Vyplňte název autora", "OK");
        }
        else
        {
            // Check for changes
            bool scalarValuesChanged = ArtistDetail.ArtistName != ArtistName;

            // No changes -> skip saving 
            if (!scalarValuesChanged)
            {
                WeakReferenceMessenger.Default.Send(new ArtistEditViewClosed());
                Debug.WriteLine("No changes detected, skipping save.");
                return;
            }


            // Changes detected -> save
            ArtistDetail.ArtistName = ArtistName;

            var saveArtist = await _facade.SaveAsync(ArtistDetail);
            Debug.WriteLine("Music track created:");
            Debug.WriteLine($"ArtistName: {ArtistDetail.ArtistName}");

            // TODO: Here is probably the best place to inform about the changes - make the popup refresh

            WeakReferenceMessenger.Default.Send(new ArtistEditViewClosed());
        }
    }


    // Revert changes command
    [RelayCommand]
    public void RevertChanges()
    {
        WeakReferenceMessenger.Default.Send(new ArtistEditViewClosed());
    }
}