using System.Diagnostics;
using System.Linq.Expressions;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.DAL.Entities;
using ICS_Project.DAL.Mappers;
using ICS_Project.DAL.Repositories;
using ICS_Project.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ICS_Project.BL.Facades;

public class MusicTrackFacade(
    IUnitOfWorkFactory uowf,
    IMusicTrackModelMapper modelMapper)
    : FacadeBase<MusicTrack, MusicTrackListModel, MusicTrackDetailModel, MusicTrackEntityMapper>(uowf, modelMapper),
        IMusicTrackFacade
{
    private readonly IUnitOfWorkFactory _uowf = uowf;
    public async Task<IEnumerable<MusicTrackListModel>> GetAsync(string searchTerm)
    {
        // Null returns each artist in db
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await base.GetAsync().ConfigureAwait(false);
        }

        string lowerSearchTerm = searchTerm.Trim().ToLower();
        Expression<Func<MusicTrack, bool>> predicate = lamb => lamb.Title.ToLower().Contains(lowerSearchTerm);

        return await GetListAsync(predicate).ConfigureAwait(false);
    }
    
    public async Task AddArtistToMusicTrackAsync(Guid musicTrackId, Guid artistId)
    {
        await using IUnitOfWork uow = _uowf.Create();

        IRepository<MusicTrack> repository = uow.GetRepository<MusicTrack, MusicTrackEntityMapper>();
        var musicTrackEntity = await repository.Get().SingleOrDefaultAsync(equals => equals.Id == musicTrackId).ConfigureAwait(false);

        if (musicTrackEntity == null) {
            throw new ArgumentException("Music track not found.", nameof(musicTrackId));
        }

        var musicTrack = await repository.UpdateAsync(musicTrackEntity).ConfigureAwait(false);

        IRepository<Artist> artistRepository = uow.GetRepository<Artist, ArtistEntityMapper>();
        var artistEntity = await artistRepository.Get().SingleOrDefaultAsync(e => e.Id == artistId).ConfigureAwait(false);

        if (artistEntity == null) {
            throw new ArgumentException("Artist not found.", nameof(artistId));
        }

        var artist = await artistRepository.UpdateAsync(artistEntity).ConfigureAwait(false);

        musicTrack.Artists.Add(artist);
        artist.MusicTracks.Add(musicTrack);

        await uow.CommitAsync().ConfigureAwait(false);
        Debug.WriteLine("Author added to the musicTrack");
    }

    public async Task AddGenreToMusicTrackAsync(Guid musicTrackId, Guid genreId)
    {
        await using IUnitOfWork uow = _uowf.Create();

        IRepository<MusicTrack> repository = uow.GetRepository<MusicTrack, MusicTrackEntityMapper>();
        var musicTrackEntity = await repository.Get().SingleOrDefaultAsync(equals => equals.Id == musicTrackId).ConfigureAwait(false);

        if (musicTrackEntity == null)
        {
            throw new ArgumentException("Music track not found.", nameof(musicTrackId));
        }

        var musicTrack = await repository.UpdateAsync(musicTrackEntity).ConfigureAwait(false);

        IRepository<Genre> genreRepository = uow.GetRepository<Genre, GenreEntityMapper>();
        var genreEntity = await genreRepository.Get().SingleOrDefaultAsync(e => e.Id == genreId).ConfigureAwait(false);

        if (genreEntity == null)
        {
            throw new ArgumentException("Genre not found.", nameof(genreId));
        }

        var genre = await genreRepository.UpdateAsync(genreEntity).ConfigureAwait(false);

        musicTrack.Genres.Add(genre);
        genre.MusicTracks.Add(musicTrack);

        await uow.CommitAsync().ConfigureAwait(false);
    }

    public async Task RemoveArtistFromMusicTrackAsync(Guid musicTrackId, Guid artistId)
    {
        await using IUnitOfWork uow = _uowf.Create();

        IRepository<MusicTrack> MusicTrackRepository = uow.GetRepository<MusicTrack, MusicTrackEntityMapper>();
        IRepository<Artist> ArtistRepository = uow.GetRepository<Artist, ArtistEntityMapper>();

        var musicTrackEntity = await MusicTrackRepository.Get()
            .Include(m => m.Artists)
            .SingleOrDefaultAsync(m => m.Id == musicTrackId)
            .ConfigureAwait(false);

        if (musicTrackEntity == null)
        {
            throw new ArgumentException("Music track not found.", nameof(musicTrackId));
        }

        var artistEntity = await ArtistRepository.Get()
            .SingleOrDefaultAsync(e => e.Id == artistId)
            .ConfigureAwait(false);

        if (artistEntity == null)
            throw new ArgumentException("Artist not found.", nameof(artistId));

        var artistToRemove = musicTrackEntity.Artists.SingleOrDefault(a => a.Id == artistId);

        if (artistToRemove == null)
        {
            throw new InvalidOperationException($"Artist with id {artistId} was not found in the music Track with id {musicTrackId}'s collection.");
        }

        musicTrackEntity.Artists.Remove(artistToRemove);
        artistEntity.MusicTracks.Remove(musicTrackEntity);

        await uow.CommitAsync().ConfigureAwait(false);
    }

    public async Task RemoveGenreFromMusicTrackAsync(Guid musicTrackId, Guid genreId)
    {
        await using IUnitOfWork uow = _uowf.Create();

        IRepository<MusicTrack> MusicTrackRepository = uow.GetRepository<MusicTrack, MusicTrackEntityMapper>();
        IRepository<Genre> GenreRepository = uow.GetRepository<Genre, GenreEntityMapper>();

        var musicTrackEntity = await MusicTrackRepository.Get()
            .Include(m => m.Genres)
            .SingleOrDefaultAsync(m => m.Id == musicTrackId)
            .ConfigureAwait(false);

        if (musicTrackEntity == null)
        {
            throw new ArgumentException("Music track not found.", nameof(musicTrackId));
        }

        var genreEntity = await GenreRepository.Get()
            .SingleOrDefaultAsync(e => e.Id == genreId)
            .ConfigureAwait(false);

        if (genreEntity == null)
            throw new ArgumentException("Genre not found.", nameof(genreId));

        var genreToRemove = musicTrackEntity.Genres.SingleOrDefault(g => g.Id == genreId);

        if (genreToRemove == null)
        {
            throw new InvalidOperationException($"Genre with id {genreId} was not found in the music Track with id {musicTrackId}'s collection.");
        }

        musicTrackEntity.Genres.Remove(genreToRemove);
        genreEntity.MusicTracks.Remove(musicTrackEntity);

        await uow.CommitAsync().ConfigureAwait(false);
    }

    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[]
        {
            nameof(MusicTrack.Artists),
            nameof(MusicTrack.Playlists),
            nameof(MusicTrack.Genres)
        };
}