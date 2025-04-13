using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers.Interfaces;
using ICS_Project.BL.Models;
using ICS_Project.Common.Tests;
using ICS_Project.Common.Tests.Seeds;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace ICS_Project.BL.Tests;

public class GenreFacadeTests : FacadeTestsBase
{
    private readonly IGenreFacade _facadeSUT;
    private readonly IGenreModelMapper _genreModelMapper;
    private readonly IMusicTrackModelMapper _musicTrackModelMapper;

    public GenreFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = ServiceProvider.GetRequiredService<IGenreFacade>();
        _genreModelMapper = ServiceProvider.GetRequiredService<IGenreModelMapper>();
        _musicTrackModelMapper = ServiceProvider.GetRequiredService<IMusicTrackModelMapper>();
    }
    
    //Seeded tests
    [Fact]
    public async Task Save_Test()
    {
        var detailModel = _genreModelMapper.MapToDetailModel(GenreSeeds.GenreEmptyMusicTracks);

        var returnedModel = await _facadeSUT.SaveAsync(detailModel);

        FixIds(detailModel, returnedModel); // What is this good for? <Vitan_, AKA xkolosv00>
        DeepAssert.Equal(detailModel, returnedModel);
    }

    private static void FixIds(GenreDetailModel expectedModel, GenreDetailModel returnedModel) //TODO: Delete this, as it has no usage? <Vitan_, AKA xkolosv00>
    {
        returnedModel.Id = expectedModel.Id;
    }
}
