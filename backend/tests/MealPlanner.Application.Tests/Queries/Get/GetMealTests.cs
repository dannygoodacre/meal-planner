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

namespace MealPlanner.Application.Tests.Queries;

[TestFixture]
internal sealed class GetMealTests : QueryHandlerTestBase<GetMealHandler, MealResponse>
{
    protected override string QueryName => "Get Meal";

    private Guid _requestId;

    protected override Task<IResult<MealResponse>> Act()
        => QueryHandler.ExecuteAsync(_requestId, TestCancellationToken);

    private Mock<IMealRepository> _mealRepositoryMock;

    private Meal _testMeal;

    [SetUp]
    public void SetUp()
    {
        _requestId = Guid.NewGuid();

        _testMeal = new Meal
        {
            PublicId = Guid.NewGuid(),
            Name = "Test Meal Name",
            NormalizedName = "Test Meal Normalized Name",
            Ingredients = [
                new Ingredient
                {
                    FoodId = 123,
                    MealId = 456,
                    Quantity = 789,
                    Food = new Food
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
                        Source = "Test Food Source 1",
                        PublicId = Guid.NewGuid(),
                        Name = "Test Food Name 1",
                        NormalizedName = "Test Food Normalized Name 1",
                    }
                }
            ]
        };

        _mealRepositoryMock = new Mock<IMealRepository>(MockBehavior.Strict);

        QueryHandler = new GetMealHandler(LoggerMock.Object, _mealRepositoryMock.Object);
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
        IResult<MealResponse> result = await Act();

        // Assert
        AssertInvalid(result, testValidationState);
    }

    [Test]
    public async Task WhenMealIsNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _testMeal = null!;

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupMealRepository_GetAsync();

        SetupLogger_NotFound();

        // Act
        IResult<MealResponse> result = await Act();

        // Assert
        AssertNotFound(result);
    }

    [Test]
    public async Task WhenMealIsFound_ShouldReturnSuccess()
    {
        // Arrange
        MealResponse expectedResponse = _testMeal.ToResponse();

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupMealRepository_GetAsync();

        // Act
        IResult<MealResponse> result = await Act();

        // Assert
        AssertSuccess(result, expectedResponse);
    }

    private void SetupLogger_QueryStarted()
        => LoggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' started with Id '{_requestId}'.");

    private void SetupMealRepository_GetAsync()
        => _mealRepositoryMock
            .Setup(x => x.GetAsync(
                It.Is<Guid>(y => y == _requestId),
                It.Is<CancellationToken>(y => y == TestCancellationToken)))
            .ReturnsAsync(_testMeal)
            .Verifiable(Times.Once);

    private void SetupLogger_NotFound()
        => LoggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' could not find entity with Id '{_requestId}'.");
}
