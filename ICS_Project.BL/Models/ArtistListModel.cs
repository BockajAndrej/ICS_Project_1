using System.Collections.ObjectModel;
using ICS_Project.BL.Facades;

namespace ICS_Project.BL.Models;

public class ArtistListModel : ModelBase, IModel
{
    public required string ArtistName { get; set; }
    
    public static ArtistListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        ArtistName = string.Empty
    }; 
}