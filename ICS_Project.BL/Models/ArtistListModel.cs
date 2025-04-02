namespace ICS_Project.BL.Models;

public class ArtistListModel : ModelBase
{
    public required string ArtistName { get; set; }
    
    public static ArtistListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        ArtistName = string.Empty
    }; 
}