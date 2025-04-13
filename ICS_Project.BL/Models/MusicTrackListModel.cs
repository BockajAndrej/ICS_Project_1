namespace ICS_Project.BL.Models;

public class MusicTrackListModel : ModelBase
{
    public required string Title { get; set; }
    public string Description { get; set; }
    public required TimeSpan Length { get; set; }
    public required double Size { get; set; }
    public required string UrlAddress { get; set; }

    public static MusicTrackListModel Empty = new()
    {
        Id = Guid.NewGuid(),
        Title = string.Empty,
        Description = string.Empty,
        Length = TimeSpan.Zero,
        Size = 0,
        UrlAddress = string.Empty
    };
}