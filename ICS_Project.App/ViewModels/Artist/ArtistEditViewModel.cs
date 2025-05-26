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
        ArtistDetail = new ArtistDetailModel
        {
            ArtistName = "",
        };
    }


    [RelayCommand]
    public async void SaveChanges()
    {
        if (ArtistName == "")
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Vyplňte název autora", "OK");
        }
        else
        {
            bool scalarValuesChanged = ArtistDetail.ArtistName != ArtistName;

            if (!scalarValuesChanged)
            {
                WeakReferenceMessenger.Default.Send(new ArtistEditViewClosed());
                Debug.WriteLine("No changes detected, skipping save.");
                return;
            }

            ArtistDetail.ArtistName = ArtistName;

            var saveArtist = await _facade.SaveAsync(ArtistDetail);
            Debug.WriteLine("Artist created:");
            Debug.WriteLine($"ArtistName: {ArtistDetail.ArtistName}");

            WeakReferenceMessenger.Default.Send(new ArtistCreatedMessage());

            WeakReferenceMessenger.Default.Send(new ArtistEditViewClosed());
        }
    }


    [RelayCommand]
    public void RevertChanges()
    {
        WeakReferenceMessenger.Default.Send(new ArtistEditViewClosed());
    }
}