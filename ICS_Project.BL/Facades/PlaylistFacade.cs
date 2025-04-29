using System.Linq.Expressions;
using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.Repositories;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.BL;

public class PlaylistFacade(
    IUnitOfWorkFactory uowf, 
    IPlaylistModelMapper modelMapper) 
    : FacadeBase<Playlist, PlaylistListModel, PlaylistDetailModel, PlaylistEntityMapper>(uowf, modelMapper),
        IPlaylistFacade
{
    private readonly IUnitOfWorkFactory _uowf = uowf;

    public async Task<IEnumerable<PlaylistListModel>> GetAsync(string searchTerm)
    {
        // Null returns each artist in db
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await base.GetAsync().ConfigureAwait(false);
        }

        string lowerSearchTerm = searchTerm.Trim().ToLower();
        Expression<Func<Playlist, bool>> predicate = lamb => lamb.Name.ToLower().Contains(lowerSearchTerm);

        return await GetListAsync(predicate).ConfigureAwait(false);
    }
    
    public async Task AddMusicTrackToPlaylistAsync(Guid playlistId, Guid musicTrackId)
    {
        await using IUnitOfWork uow = _uowf.Create();
        
        IRepository<Playlist> repository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        var playListEntity = await repository.Get().SingleOrDefaultAsync(e => e.Id == playlistId).ConfigureAwait(false);
        
        if (playListEntity == null)
            throw new ArgumentException("Playlist not found.", nameof(playlistId));
        
        var playlist = await repository.UpdateAsync(playListEntity).ConfigureAwait(false);
        
        IRepository<MusicTrack> musicTrackrepository = uow.GetRepository<MusicTrack, MusicTrackEntityMapper>();
        var musicTrackEntity = await musicTrackrepository.Get().SingleOrDefaultAsync(e => e.Id == musicTrackId).ConfigureAwait(false);
        
        if (musicTrackEntity == null)
            throw new ArgumentException("Music track not found.", nameof(musicTrackId));
        
        var musicTrack = await musicTrackrepository.UpdateAsync(musicTrackEntity).ConfigureAwait(false);

        playlist.MusicTracks.Add(musicTrack);
        musicTrack.Playlists.Add(playlist);
        
        await uow.CommitAsync().ConfigureAwait(false);
    }

    public async Task RemoveMusicTrackFromPlaylistAsync(Guid playlistId, Guid musicTrackId)
    {
        await using IUnitOfWork uow = _uowf.Create();

        IRepository<Playlist> playlistRepository = uow.GetRepository<Playlist, PlaylistEntityMapper>();
        IRepository<MusicTrack> musicTrackRepository = uow.GetRepository<MusicTrack, MusicTrackEntityMapper>();

        var playlistEntity = await playlistRepository.Get()
            .Include(p => p.MusicTracks)
            .SingleOrDefaultAsync(p => p.Id == playlistId)
            .ConfigureAwait(false);

        if (playlistEntity == null)
            throw new ArgumentException("Playlist not found.", nameof(playlistId));

        var musicTrackEntity = await musicTrackRepository.Get()
            .SingleOrDefaultAsync(e => e.Id == musicTrackId)
            .ConfigureAwait(false);

        if (musicTrackEntity == null)
            throw new ArgumentException("Music track not found.", nameof(musicTrackId));

        var trackToRemove = playlistEntity.MusicTracks.SingleOrDefault(mt => mt.Id == musicTrackId);

        if (trackToRemove == null)
        {
            throw new InvalidOperationException($"Music track with id {musicTrackId} was not found in the playlist with id {playlistId}'s collection.");
        }

        playlistEntity.MusicTracks.Remove(trackToRemove);
        musicTrackEntity.Playlists.Remove(playlistEntity);

        await uow.CommitAsync().ConfigureAwait(false);
    }
    
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { nameof(Playlist.MusicTracks) };
}