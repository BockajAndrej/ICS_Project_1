using ICS_Project.DAL.Entities;

namespace ICS_Project.DAL.Mappers;

public class MusicTrackEntityMapper : IEntityMapper<MusicTrack>
{
    public void MapToExistingEntity(MusicTrack existingEntity, MusicTrack newEntity)
    {
        existingEntity.Description = newEntity.Description;
        existingEntity.Length = newEntity.Length;
        existingEntity.Size = newEntity.Size;
        existingEntity.Title = newEntity.Title;
        existingEntity.UrlAddress = newEntity.UrlAddress;
    }
}