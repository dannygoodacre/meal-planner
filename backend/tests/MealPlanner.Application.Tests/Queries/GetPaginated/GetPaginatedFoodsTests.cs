using DannyGoodacre.Cqrs.Testing;
using DannyGoodacre.Primitives;
using DannyGoodacre.Testing;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MealPlanner.Application.Tests.Queries.GetPaginated;

[TestFixture]
internal sealed class GetPaginatedFoodsTests : QueryHandlerTestBase<GetPaginatedFoodsHandler, PaginatedPublicEntityResponse<FoodResponse>>
{
    protected override string QueryName => "Get Paginated Foods";

    private int _requestLimit;

    private int _requestPage;

    private int _testCount;

    private List<Food> _testFoods = null!;

    private PaginatedPublicEntityResponse<FoodResponse> _testResponse = null!;

    private Mock<IFoodRepository> _foodRepositoryMock;

    protected override Task<IResult<PaginatedPublicEntityResponse<FoodResponse>>> Act()
        => QueryHandler.ExecuteAsync(_requestPage, _requestLimit, TestCancellationToken);

    [SetUp]
    public void SetUp()
    {
        _foodRepositoryMock = new Mock<IFoodRepository>();

        QueryHandler = new GetPaginatedFoodsHandler(LoggerMock.Object, _foodRepositoryMock.Object);
    }

    [Test]
    public async Task WhenRequestInvalid_ShouldReturnInvalid()
    {
        // Arrange
        _requestLimit = 0;

        _requestPage = -1;

        LoggerMock.IsEnabled();

        LoggerMock.LogQueryFailedValidation(QueryName, $"Page:{Environment.NewLine}  - Must be greater than or equal to 0.{Environment.NewLine}Limit:{Environment.NewLine}  - Must be greater than 0.");

        var testValidationState = new ValidationState();

        testValidationState.AddError("Page", "Must be greater than or equal to 0.");

        testValidationState.AddError("Limit", "Must be greater than 0.");

        // Act
        var result = await Act();

        // Assert
        AssertInvalid(result, testValidationState);
    }

    [Test]
    public async Task WhenNoFoodsFound_ShouldReturnSuccessWithNoItems()
    {
        // Arrange
        _requestPage = 2;

        _requestLimit = 3;

        _testCount = 0;

        _testFoods = [];

        _testResponse = new PaginatedPublicEntityResponse<FoodResponse>
        {
            Items = [],
            CurrentPage = _requestPage,
            TotalItemsCount = 0,
            TotalPagesCount = 0
        };

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupFoodRepository_GetCountAsync();

        SetupFoodRepository_GetPaginatedAsync();

        // Act
        var result = await Act();

        // Assert
        AssertSuccess(result, _testResponse);
    }

    [Test]
    public async Task WhenFullPageRequested_ShouldReturnSuccessWithFullPageOfItems()
    {
        // Arrange
        _requestPage = 2;

        _requestLimit = 3;

        _testCount = 11;

        _testFoods = CreateTestFoods(3);

        _testResponse = new PaginatedPublicEntityResponse<FoodResponse>
        {
            Items = _testFoods.Select(x => x.ToResponse()).ToList(),
            CurrentPage = _requestPage,
            TotalItemsCount = _testCount,
            TotalPagesCount = 4
        };

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupFoodRepository_GetCountAsync();

        SetupFoodRepository_GetPaginatedAsync();

        // Act
        var result = await Act();

        // Assert
        AssertSuccess(result, _testResponse);
    }

    [Test]
    public async Task WhenLastPageRequested_ShouldReturnSuccessWithPartialPageOfItems()
    {
        // Arrange
        _requestPage = 4;

        _requestLimit = 3;

        _testCount = 11;

        _testFoods = CreateTestFoods(2);

        _testResponse = new PaginatedPublicEntityResponse<FoodResponse>
        {
            Items = _testFoods.Select(x => x.ToResponse()).ToList(),
            CurrentPage = 4,
            TotalItemsCount = 11,
            TotalPagesCount = 4,
        };

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupFoodRepository_GetCountAsync();

        SetupFoodRepository_GetPaginatedAsync();

        // Act
        var result = await Act();

        // Assert
        AssertSuccess(result, _testResponse);
    }

    [Test]
    public async Task WhenPageOutOfRange_ShouldReturnSuccessWithNoItems()
    {
        // Arrange
        _requestLimit = 3;

        _requestPage = 100;

        _testCount = 11;

        _testFoods = [];

        _testResponse = new PaginatedPublicEntityResponse<FoodResponse>
        {
            Items = [],
            CurrentPage = _requestPage,
            TotalItemsCount = 11,
            TotalPagesCount = 4
        };

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupFoodRepository_GetCountAsync();

        SetupFoodRepository_GetPaginatedAsync();

        // Act
        var result = await Act();

        // Assert
        AssertSuccess(result, _testResponse);
    }

    private static List<Food> CreateTestFoods(int count)
    {
        return Enumerable.Range(0, count).Select(x => new Food
        {
            Unit = Unit.Grams,
            ReferenceQuantity = 10 * x + 1,
            Calories = 10 * x + 2,
            Protein = 10 * x + 3,
            Carbohydrates = 10 * x + 4,
            Sugar = 10 * x + 5,
            Fat = 10 * x + 6,
            SaturatedFat = 10 * x + 7,
            Fibre = 10 * x + 8,
            Salt = 10 * x + 9,
            Source = $"Test Food Source {x}",
            PublicId = new Guid($"00000000-0000-0000-0000-00000000000{x}"),
            Name = $"Test Food Name {x}",
            NormalizedName = $"Test Normalized Food Name {x}"
        }).ToList();
    }

    private void SetupLogger_QueryStarted()
        => LoggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' started with Page '{_requestPage}' and Limit '{_requestLimit}'.");

    private void SetupFoodRepository_GetCountAsync()
        => _foodRepositoryMock
            .Setup(x =>
                x.GetCountAsync(It.Is<CancellationToken>(y => y == TestCancellationToken)))
            .ReturnsAsync(_testCount)
            .Verifiable(Times.Once);

    private void SetupFoodRepository_GetPaginatedAsync()
        => _foodRepositoryMock
            .Setup(x =>
                x.GetPaginatedAsync(
                    It.Is<int>(y => y == _requestPage),
                    It.Is<int>(y => y == _requestLimit),
                    It.Is<CancellationToken>(y => y == TestCancellationToken)))
            .ReturnsAsync(_testFoods)
            .Verifiable(Times.Once);
}
