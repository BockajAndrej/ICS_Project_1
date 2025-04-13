using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;

namespace ICS_Project.BL.Mappers;

public class MusicTrackModelMapper : ModelMapperBase<MusicTrack, MusicTrackListModel, MusicTrackDetailModel>, IMusicTrackModelMapper
{
    private readonly Lazy<IArtistModelMapper> _artistMapperLazy;
    private readonly Lazy<IGenreModelMapper> _genreMapperLazy;
    private readonly Lazy<IPlaylistModelMapper> _playlistMapperLazy;

    public MusicTrackModelMapper(
        Lazy<IArtistModelMapper> artistMapperLazy,
        Lazy<IGenreModelMapper> genreMapperLazy,
        Lazy<IPlaylistModelMapper> playlistMapperLazy)
    {
        _artistMapperLazy = artistMapperLazy;
        _genreMapperLazy = genreMapperLazy;
        _playlistMapperLazy = playlistMapperLazy;
    }
    
    public override MusicTrackListModel MapToListModel(MusicTrack? entity)
        => entity is null
            ? MusicTrackListModel.Empty
            : new MusicTrackListModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Length = entity.Length,
                Size = entity.Size,
                UrlAddress = entity.UrlAddress,
            };

    public override MusicTrackDetailModel MapToDetailModel(MusicTrack? entity)
        => entity is null
            ? MusicTrackDetailModel.Empty
            : new MusicTrackDetailModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Length = entity.Length,
                Size = entity.Size,
                UrlAddress = entity.UrlAddress,
                Artists = _artistMapperLazy.Value.MapToListModel(entity.Artists)
                    .ToObservableCollection(),
                Genres = _genreMapperLazy.Value.MapToListModel(entity.Genres)
                    .ToObservableCollection(),
                Playlists = _playlistMapperLazy.Value.MapToListModel(entity.Playlists)
                    .ToObservableCollection(),  
            };

    public override MusicTrack MapToEntity(MusicTrackDetailModel model)
        => new()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Length = model.Length,
                Size = model.Size,
                UrlAddress = model.UrlAddress,
            };
}