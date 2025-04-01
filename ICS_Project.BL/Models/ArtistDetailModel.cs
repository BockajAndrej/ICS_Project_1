namespace ICS_Project.BL.Models;

public class ArtistDetailModel : ModelBase,  IModel
{
    public required string ArtistName { get; set; }
    
    public static ArtistDetailModel Empty = new()
    {
        Id = Guid.NewGuid(),
        ArtistName = string.Empty
    }; 
}