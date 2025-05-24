using CommunityToolkit.Mvvm.ComponentModel;

namespace ICS_Project.BL.Models;

public partial class ArtistListModel : ModelBase
{
    [ObservableProperty]
    private bool isSelected;
    public required string ArtistName { get; set; }
    
    public static ArtistListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        ArtistName = string.Empty
    }; 
}