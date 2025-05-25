using CommunityToolkit.Mvvm.ComponentModel;

namespace ICS_Project.BL.Models;

public partial class GenreListModel : ModelBase
{
    [ObservableProperty]
    private bool isSelected;
    public string GenreName { get; set; }
    
    public static GenreListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        GenreName = string.Empty
    };
}