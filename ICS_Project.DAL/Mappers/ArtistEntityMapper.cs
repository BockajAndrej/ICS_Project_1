using ICS_Project.DAL.Entities;

namespace ICS_Project.DAL.Mappers;

public class ArtistEntityMapper : IEntityMapper<Artist>
{
    public void MapToExistingEntity(Artist existingEntity, Artist newEntity)
    {
        existingEntity.ArtistName = newEntity.ArtistName;
    }
}