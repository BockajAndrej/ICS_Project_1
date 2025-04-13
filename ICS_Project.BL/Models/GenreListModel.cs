namespace ICS_Project.BL.Models;

public class GenreListModel : ModelBase
{
    public string GenreName { get; set; }
    
    public static GenreListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        GenreName = string.Empty
    };
}