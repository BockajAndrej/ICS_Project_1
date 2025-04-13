using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ICS_Project.BL.Tests;

public class MusicTrackFacadeTests : FacadeTestsBase
{
    private readonly IMusicTrackFacade _facadeSUT;
    private readonly IMusicTrackModelMapper _musicTrackModelMapper;
    private readonly IGenreModelMapper _genreModelMapper;
    private readonly IArtistModelMapper _artistModelMapper;
    private readonly IPlaylistModelMapper _playlistModelMapper;

    public MusicTrackFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = ServiceProvider.GetRequiredService<IMusicTrackFacade>();
        _musicTrackModelMapper = ServiceProvider.GetRequiredService<IMusicTrackModelMapper>();
        _genreModelMapper = ServiceProvider.GetRequiredService<IGenreModelMapper>();
        _artistModelMapper = ServiceProvider.GetRequiredService<IArtistModelMapper>();
        _playlistModelMapper = ServiceProvider.GetRequiredService<IPlaylistModelMapper>();
    }
    
    //Seeded tests
    [Fact]
    public async Task Save_Test()
    {
        var detailModel = _musicTrackModelMapper.MapToDetailModel(MusicTrackSeeds.NonEmptyMusicTrack2);

        var returnedModel = await _facadeSUT.SaveAsync(detailModel);

        FixIds(detailModel, returnedModel); // What is this good for? <Vitan_, AKA xkolosv00>
        DeepAssert.Equal(detailModel, returnedModel);
    }

    private static void FixIds(MusicTrackDetailModel expectedModel, MusicTrackDetailModel returnedModel) //TODO: Delete this, as it has no usage? <Vitan_, AKA xkolosv00>
    {
        returnedModel.Id = expectedModel.Id;
    }
}
