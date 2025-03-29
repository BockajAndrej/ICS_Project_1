using ICS_Project.DAL.Entities;

namespace ICS_Project.DAL.Mappers;

public class PlaylistEntityMapper : IEntityMapper<Playlist>
{
    public void MapToExistingEntity(Playlist existingEntity, Playlist newEntity)
    {
        existingEntity.Name = newEntity.Name;
        existingEntity.Description = newEntity.Description;
        existingEntity.TotalPlayTime = newEntity.TotalPlayTime;
        existingEntity.NumberOfMusicTracks = newEntity.NumberOfMusicTracks;
    }
}