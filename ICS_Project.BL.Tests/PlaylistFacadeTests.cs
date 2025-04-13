using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ICS_Project.BL.Tests;

public class PlaylistFacadeTests : FacadeTestsBase
{
    private readonly IPlaylistFacade _facadeSUT;
    private readonly IPlaylistModelMapper _playlistModelMapper;
    private readonly IMusicTrackModelMapper _musicTrackModelMapper;

    public PlaylistFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = ServiceProvider.GetRequiredService<IPlaylistFacade>();
        _playlistModelMapper = ServiceProvider.GetRequiredService<IPlaylistModelMapper>();
        _musicTrackModelMapper = ServiceProvider.GetRequiredService<IMusicTrackModelMapper>();
    }
    
    //Seeded tests
    [Fact]
    public async Task Save_Test()
    {
        var detailModel = _playlistModelMapper.MapToDetailModel(PlaylistSeeds.PlaylistUpdate);

        var returnedModel = await _facadeSUT.SaveAsync(detailModel);

        FixIds(detailModel, returnedModel); // What is this good for? <Vitan_, AKA xkolosv00>
        DeepAssert.Equal(detailModel, returnedModel);
    }

    private static void FixIds(PlaylistDetailModel expectedModel, PlaylistDetailModel returnedModel) //TODO: Delete this, as it has no usage? <Vitan_, AKA xkolosv00>
    {
        returnedModel.Id = expectedModel.Id;
    }
}