using MealPlanner.Application.Commands;
using MealPlanner.Application.Enums;
using MealPlanner.Application.Foods;
using MealPlanner.Application.MealPlans;
using MealPlanner.Application.Meals;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;

namespace MealPlanner.Application.Tests;

public sealed class FoodDetailsValidatorTests
{
    private static AddFoodCommand Valid() => new()
    {
        Name = "Banana",
        Unit = Unit.Grams,
        ReferenceQuantity = 100,
        Calories = 89,
        Protein = 1.1m,
        Carbohydrates = 23m,
        Sugar = 12m,
        Fat = 0.3m,
        SaturatedFat = 0.1m,
        Fibre = 2.6m,
        Salt = 0m,
        Source = "https://example.com/banana"
    };

    private static ValidationState Validate(AddFoodCommand command)
    {
        var state = new ValidationState();
        new FoodDetailsValidator<AddFoodCommand>().Validate(command, state);
        return state;
    }

    [Fact]
    public void Validate_WhenValid_HasNoErrors() => Validate(Valid()).ShouldHaveNoErrors();

    [Fact]
    public void Validate_WhenNameIsBlank_HasError()
        => Validate(Valid() with { Name = " " }).ShouldHaveErrorCode("Name", "Required");

    [Fact]
    public void Validate_WhenUnitIsUndefined_HasError()
        => Validate(Valid() with { Unit = (Unit)99 }).ShouldHaveErrorCode("Unit", "InvalidEnum");

    [Fact]
    public void Validate_WhenReferenceQuantityIsNotPositive_HasError()
        => Validate(Valid() with { ReferenceQuantity = 0 }).ShouldHaveErrorCode("ReferenceQuantity", "Positive");

    [Fact]
    public void Validate_WhenCaloriesAreNegative_HasError()
        => Validate(Valid() with { Calories = -1 }).ShouldHaveErrorCode("Calories", "NonNegative");

    [Fact]
    public void Validate_WhenProteinIsNegative_HasError()
        => Validate(Valid() with { Protein = -1 }).ShouldHaveErrorCode("Protein", "NonNegative");

    [Fact]
    public void Validate_WhenCarbohydratesAreNegative_HasError()
        => Validate(Valid() with { Carbohydrates = -1 }).ShouldHaveErrorCode("Carbohydrates", "NonNegative");

    [Fact]
    public void Validate_WhenSugarIsNegative_HasError()
        => Validate(Valid() with { Sugar = -1 }).ShouldHaveErrorCode("Sugar", "NonNegative");

    [Fact]
    public void Validate_WhenSugarExceedsCarbohydrates_HasError()
        => Validate(Valid() with { Carbohydrates = 10m, Sugar = 11m })
            .ShouldHaveErrorCode("Sugar", "LessThanOrEqualTo");

    [Fact]
    public void Validate_WhenFatIsNegative_HasError()
        => Validate(Valid() with { Fat = -1 }).ShouldHaveErrorCode("Fat", "NonNegative");

    [Fact]
    public void Validate_WhenSaturatedFatIsNegative_HasError()
        => Validate(Valid() with { SaturatedFat = -1 }).ShouldHaveErrorCode("SaturatedFat", "NonNegative");

    [Fact]
    public void Validate_WhenSaturatedFatExceedsFat_HasError()
        => Validate(Valid() with { Fat = 1m, SaturatedFat = 2m })
            .ShouldHaveErrorCode("SaturatedFat", "LessThanOrEqualTo");

    [Fact]
    public void Validate_WhenFibreIsNegative_HasError()
        => Validate(Valid() with { Fibre = -1 }).ShouldHaveErrorCode("Fibre", "NonNegative");

    [Fact]
    public void Validate_WhenSaltIsNegative_HasError()
        => Validate(Valid() with { Salt = -1 }).ShouldHaveErrorCode("Salt", "NonNegative");

    [Theory]
    [InlineData("not a url")]
    [InlineData("ftp://example.com")]
    public void Validate_WhenSourceIsInvalidUrl_HasError(string source)
        => Validate(Valid() with { Source = source }).ShouldHaveErrorCode("Source", "InvalidUrl");
}

public sealed class MealDetailsValidatorTests
{
    private static readonly Guid FoodA = Guid.NewGuid();
    private static readonly Guid FoodB = Guid.NewGuid();

    private static AddMealCommand Valid() => new()
    {
        Name = "Porridge",
        Ingredients = [new IngredientRequest(FoodA, 100), new IngredientRequest(FoodB, 50)]
    };

    private static ValidationState Validate(AddMealCommand command)
    {
        var state = new ValidationState();
        new MealDetailsValidator<AddMealCommand>().Validate(command, state);
        return state;
    }

    [Fact]
    public void Validate_WhenValid_HasNoErrors() => Validate(Valid()).ShouldHaveNoErrors();

    [Fact]
    public void Validate_WhenNameIsBlank_HasError()
        => Validate(Valid() with { Name = "" }).ShouldHaveErrorCode("Name", "Required");

    [Fact]
    public void Validate_WhenIngredientsAreEmpty_HasError()
        => Validate(Valid() with { Ingredients = [] }).ShouldHaveErrorCode("Ingredients", "NotEmpty");

    [Fact]
    public void Validate_WhenFoodIdIsEmpty_HasError()
        => Validate(Valid() with { Ingredients = [new IngredientRequest(Guid.Empty, 100)] })
            .ShouldHaveErrorCode("Ingredients[0].FoodId", "NotEmpty");

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WhenQuantityIsNotPositive_HasError(int quantity)
        => Validate(Valid() with { Ingredients = [new IngredientRequest(FoodA, quantity)] })
            .ShouldHaveErrorCode("Ingredients[0].Quantity", "Positive");

    [Fact]
    public void Validate_WhenFoodIdsAreDuplicated_HasError()
        => Validate(Valid() with { Ingredients = [new IngredientRequest(FoodA, 1), new IngredientRequest(FoodA, 2)] })
            .ShouldHaveErrorCode("Ingredients", "Duplicate");
}

public sealed class MealPlanDetailsValidatorTests
{
    private static readonly Guid FoodA = Guid.NewGuid();

    private static AddMealPlanMealRequest ValidMeal() => new()
    {
        Name = "Breakfast",
        Ingredients = [new IngredientRequest(FoodA, 100)]
    };

    private static AddMealPlanCommand Valid() => new() { Name = "Week 1", Meals = [ValidMeal()] };

    private static ValidationState Validate(AddMealPlanCommand command)
    {
        var state = new ValidationState();
        new MealPlanDetailsValidator<AddMealPlanCommand>().Validate(command, state);
        return state;
    }

    [Fact]
    public void Validate_WhenValid_HasNoErrors() => Validate(Valid()).ShouldHaveNoErrors();

    [Fact]
    public void Validate_WhenNameIsBlank_HasError()
        => Validate(Valid() with { Name = " " }).ShouldHaveErrorCode("Name", "Required");

    [Fact]
    public void Validate_WhenMealsAreEmpty_HasError()
        => Validate(Valid() with { Meals = [] }).ShouldHaveErrorCode("Meals", "NotEmpty");

    [Fact]
    public void Validate_WhenMealNameIsBlank_HasIndexedError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Name = "" }] })
            .ShouldHaveErrorCode("Meals[0].Name", "Required");

    [Fact]
    public void Validate_WhenMealIngredientsAreEmpty_HasError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Ingredients = [] }] })
            .ShouldHaveErrorCode("Meals[0].Ingredients", "NotEmpty");

    [Fact]
    public void Validate_WhenMealFoodIdIsEmpty_HasError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Ingredients = [new IngredientRequest(Guid.Empty, 1)] }] })
            .ShouldHaveErrorCode("Meals[0].Ingredients[0].FoodId", "NotEmpty");

    [Fact]
    public void Validate_WhenMealQuantityIsNotPositive_HasError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Ingredients = [new IngredientRequest(FoodA, 0)] }] })
            .ShouldHaveErrorCode("Meals[0].Ingredients[0].Quantity", "Positive");

    [Fact]
    public void Validate_WhenMealFoodIdsAreDuplicated_HasError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Ingredients = [new IngredientRequest(FoodA, 1), new IngredientRequest(FoodA, 2)] }] })
            .ShouldHaveErrorCode("Meals[0].Ingredients", "Duplicate");
}

public sealed class PublicIdValidatorTests
{
    private static ValidationState Validate(Guid id)
    {
        var state = new ValidationState();
        new PublicIdValidator<DeleteFoodCommand>().Validate(new DeleteFoodCommand(id), state);
        return state;
    }

    [Fact]
    public void Validate_WhenIdIsValid_HasNoErrors() => Validate(Guid.NewGuid()).ShouldHaveNoErrors();

    [Fact]
    public void Validate_WhenIdIsEmpty_HasError() => Validate(Guid.Empty).ShouldHaveErrorCode("Id", "NotEmpty");
}

public sealed class SearchTermValidatorTests
{
    private static ValidationState Validate(string term)
    {
        var state = new ValidationState();
        new SearchTermValidator<SearchFoodQuery>().Validate(new SearchFoodQuery(term), state);
        return state;
    }

    [Fact]
    public void Validate_WhenTwoCharacters_HasError()
        => Validate("ab").ShouldHaveErrorCode("SearchTerm", "MinimumLength");

    [Fact]
    public void Validate_WhenThreeCharacters_HasNoErrors() => Validate("abc").ShouldHaveNoErrors();
}
