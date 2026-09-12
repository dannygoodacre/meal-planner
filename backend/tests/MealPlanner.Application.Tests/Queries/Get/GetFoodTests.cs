using DannyGoodacre.Cqrs.Testing;
using DannyGoodacre.Primitives;
using DannyGoodacre.Testing;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MealPlanner.Application.Tests.Queries;

[TestFixture]
internal sealed class GetFoodTests : QueryHandlerTestBase<GetFoodHandler, FoodResponse>
{
    protected override string QueryName => "Get Food";

    private Guid _requestId;

    protected override Task<IResult<FoodResponse>> Act()
        => QueryHandler.ExecuteAsync(_requestId, TestCancellationToken);

    private Mock<IFoodRepository> _foodRepositoryMock;

    private Food _testFood;

    private FoodResponse _testResponse;

    [SetUp]
    public void SetUp()
    {
        _requestId = Guid.NewGuid();

        _testFood = new Food
        {
            Unit = Unit.Count,
            ReferenceQuantity = 1,
            Calories = 2,
            Protein = 3,
            Carbohydrates = 4,
            Sugar = 5,
            Fat = 6,
            SaturatedFat = 7,
            Fibre = 8,
            Salt = 9,
            Source = "Test Source",
            PublicId = Guid.NewGuid(),
            Name = "Test Name",
            NormalizedName = "Test Normalized Name"
        };

        _testResponse = _testFood.ToResponse();

        _foodRepositoryMock = new Mock<IFoodRepository>(MockBehavior.Strict);

        QueryHandler = new GetFoodHandler(LoggerMock.Object, _foodRepositoryMock.Object);
    }

    [Test]
    public async Task WhenIdIsEmpty_ShouldReturnInvalid()
    {
        // Arrange
        _requestId = Guid.Empty;

        LoggerMock.IsEnabled();

        LoggerMock.LogQueryFailedValidation(QueryName, $"Id:{Environment.NewLine}  - Must not be empty.");

        var testValidationState = new ValidationState();

        testValidationState.AddError("Id", "Must not be empty.");

        // Act
        var result = await Act();

        // Assert
        AssertInvalid(result, testValidationState);
    }

    [Test]
    public async Task WhenFoodIsNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _testFood = null!;

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupFoodRepository_GetAsync();

        SetupLogger_NotFound();

        // Act
        var result = await Act();

        // Assert
        AssertNotFound(result);
    }

    [Test]
    public async Task WhenFoodIsFound_ShouldReturnSuccess()
    {
        // Arrange
        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupFoodRepository_GetAsync();

        // Act
        var result = await Act();

        // Assert
        AssertSuccess(result, _testResponse);
    }

    private void SetupLogger_QueryStarted()
        => LoggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' started with Id '{_requestId}'.");

    private void SetupFoodRepository_GetAsync()
        => _foodRepositoryMock
            .Setup(x => x.GetAsync(
                It.Is<Guid>(y => y == _requestId),
                It.Is<CancellationToken>(y => y == TestCancellationToken)))
            .ReturnsAsync(_testFood)
            .Verifiable(Times.Once);

    private void SetupLogger_NotFound()
        => LoggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' could not find entity with Id '{_requestId}'.");
}
