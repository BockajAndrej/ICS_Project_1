using ICS_Project.BL.Facades;
using ICS_Project.BL.Mappers;
using ICS_Project.BL.Models;
using ICS_Project.Common.Tests;
using ICS_Project.DAL.UnitOfWork;
using Xunit.Abstractions;

namespace ICS_Project.BL.Tests;

public class ArtistFacadeTests : FacadeTestsBase
{
    private readonly IArtistFacade _facadeSUT;

    public ArtistFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = new ArtistFacade(UnitOfWorkFactory, ArtistModelMapper);
    }
    [Fact]
    public async Task Run()
    {
        //Arrange
        var model = new ArtistDetailModel()
        {
            ArtistName = "Recipe 1",
        };

        //Act
        var returnedModel = await _facadeSUT.SaveAsync(model);

        //Assert
        FixIds(model, returnedModel);
        DeepAssert.Equal(model, returnedModel);
    }
    
    private static void FixIds(ArtistDetailModel expectedModel, ArtistDetailModel returnedModel)
    {
        returnedModel.Id = expectedModel.Id;
    }
}