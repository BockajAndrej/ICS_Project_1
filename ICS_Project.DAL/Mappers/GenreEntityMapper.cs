using ICS_Project.DAL.Entities;

namespace ICS_Project.DAL.Mappers;

public class GenreEntityMapper : IEntityMapper<Genre>
{
    public void MapToExistingEntity(Genre existingEntity, Genre newEntity)
    {
        existingEntity.GenreName = newEntity.GenreName;
    }
}