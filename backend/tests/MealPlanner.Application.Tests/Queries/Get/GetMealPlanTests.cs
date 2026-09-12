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
internal sealed class GetMealPlanTests : QueryHandlerTestBase<GetMealPlanHandler, MealPlanResponse>
{
    protected override string QueryName => "Get MealPlan";

    private Guid _requestId;

    protected override Task<IResult<MealPlanResponse>> Act()
        => QueryHandler.ExecuteAsync(_requestId, TestCancellationToken);

    private Mock<IMealPlanRepository> _mealPlanRepositoryMock;

    private MealPlan _testMealPlan;

    [SetUp]
    public void SetUp()
    {
        _requestId = Guid.NewGuid();

        _testMealPlan = new MealPlan
        {
            PublicId = Guid.NewGuid(),
            Name = "Test Meal Plan Name",
            NormalizedName = "Test Meal Plan Normalized Name",
            Meals = [
                new MealPlanMeal
                {
                    PublicId = Guid.NewGuid(),
                    Name = "Test Meal Plan Meal Name",
                    NormalizedName = "Test Meal Plan Meal Normalized Name",
                    Ingredients = [
                        new MealPlanIngredient
                        {
                            Quantity = 100,
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
                        },
                        new MealPlanIngredient
                        {
                            Quantity = 200,
                            Food = new Food
                            {
                                Unit = Unit.Grams,
                                ReferenceQuantity = 10,
                                Calories = 11,
                                Protein = 12,
                                Carbohydrates = 13,
                                Sugar = 14,
                                Fat = 15,
                                SaturatedFat = 16,
                                Fibre = 17,
                                Salt = 18,
                                Source = "Test Food Source 2",
                                PublicId = Guid.NewGuid(),
                                Name = "Test Food Name 2",
                                NormalizedName = "Test Food Normalized Name 2",
                            }
                        }
                    ]
                }
            ]
        };

        _mealPlanRepositoryMock = new Mock<IMealPlanRepository>(MockBehavior.Strict);

        QueryHandler = new GetMealPlanHandler(LoggerMock.Object, _mealPlanRepositoryMock.Object);
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
        IResult<MealPlanResponse> result = await Act();

        // Assert
        AssertInvalid(result, testValidationState);
    }

    [Test]
    public async Task WhenMealPlanIsNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _testMealPlan = null!;

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupMealPlanRepository_GetAsync();

        SetupLogger_NotFound();

        // Act
        IResult<MealPlanResponse> result = await Act();

        // Assert
        AssertNotFound(result);
    }

    [Test]
    public async Task WhenMealPlanIsFound_ShouldReturnSuccess()
    {
        // Arrange
        MealPlanResponse expectedResponse = _testMealPlan.ToResponse();

        LoggerMock.IsEnabled();

        SetupLogger_QueryStarted();

        SetupMealPlanRepository_GetAsync();

        // Act
        IResult<MealPlanResponse> result = await Act();

        // Assert
        AssertSuccess(result, expectedResponse);
    }

    private void SetupLogger_QueryStarted()
        => LoggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' started with Id '{_requestId}'.");

    private void SetupMealPlanRepository_GetAsync()
        => _mealPlanRepositoryMock
            .Setup(x => x.GetAsync(
                It.Is<Guid>(y => y == _requestId),
                It.Is<CancellationToken>(y => y == TestCancellationToken)))
            .ReturnsAsync(_testMealPlan)
            .Verifiable(Times.Once);

    private void SetupLogger_NotFound()
        => LoggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' could not find entity with Id '{_requestId}'.");
}
