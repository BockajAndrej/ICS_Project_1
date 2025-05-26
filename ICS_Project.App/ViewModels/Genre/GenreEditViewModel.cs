using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Models;
using System.Diagnostics;

namespace ICS_Project.App.ViewModels.Genre;

public partial class GenreEditViewModel : ObservableObject
{
    private readonly IGenreFacade _facade;

    [ObservableProperty]
    private GenreDetailModel _genreDetail;

    [ObservableProperty]
    private string _genreName = "";

    public async Task InitializeAsync(Guid id)
    {
        GenreDetail = await _facade.GetAsync(id);
    }

    public GenreEditViewModel(IGenreFacade genreFacade)
    {
        _facade = genreFacade;

        GenreDetail = new GenreDetailModel
        {
            GenreName = "",
        };
    }

    [RelayCommand]
    public async void SaveChanges()
    {
        // Constraints
        if (GenreName == "")
        {
            await Application.Current.MainPage.DisplayAlert("Validační chyba", "Vyplňte název žánru", "OK");
        }
        else
        {
            bool scalarValuesChanged = GenreDetail.GenreName != GenreName;

            if (!scalarValuesChanged)
            {
                WeakReferenceMessenger.Default.Send(new GenreEditViewClosed());
                Debug.WriteLine("No changes detected, skipping save.");
                return;
            }

            GenreDetail.GenreName = GenreName;

            var SaveGenre = await _facade.SaveAsync(GenreDetail);
            Debug.WriteLine("Genre created:");
            Debug.WriteLine($"GenreName: {GenreDetail.GenreName}");

            WeakReferenceMessenger.Default.Send(new GenreEditViewClosed());
        }
    }


    [RelayCommand]
    public void RevertChanges()
    {
        WeakReferenceMessenger.Default.Send(new GenreEditViewClosed());
    }
}