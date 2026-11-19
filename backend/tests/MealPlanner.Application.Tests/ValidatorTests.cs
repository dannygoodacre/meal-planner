using MealPlanner.Application.Commands;
using MealPlanner.Application.Enums;
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
        => Validate(Valid() with { Name = " " }).ShouldHaveError("Name", "Must not be null or empty.");

    [Fact]
    public void Validate_WhenUnitIsUndefined_HasError()
        => Validate(Valid() with { Unit = (Unit)99 }).ShouldHaveError("Unit", "Must be defined.");

    [Fact]
    public void Validate_WhenReferenceQuantityIsNotPositive_HasError()
        => Validate(Valid() with { ReferenceQuantity = 0 }).ShouldHaveError("ReferenceQuantity", "Must be greater than 0.");

    [Fact]
    public void Validate_WhenCaloriesAreNegative_HasError()
        => Validate(Valid() with { Calories = -1 }).ShouldHaveError("Calories", "Must be greater than or equal to 0.");

    [Fact]
    public void Validate_WhenProteinIsNegative_HasError()
        => Validate(Valid() with { Protein = -1 }).ShouldHaveError("Protein", "Must be greater than or equal to 0.");

    [Fact]
    public void Validate_WhenCarbohydratesAreNegative_HasError()
        => Validate(Valid() with { Carbohydrates = -1 }).ShouldHaveError("Carbohydrates", "Must be greater than or equal to 0.");

    [Fact]
    public void Validate_WhenSugarIsNegative_HasError()
        => Validate(Valid() with { Sugar = -1 }).ShouldHaveError("Sugar", "Must be greater than or equal to 0.");

    [Fact]
    public void Validate_WhenSugarExceedsCarbohydrates_HasError()
        => Validate(Valid() with { Carbohydrates = 10m, Sugar = 11m })
            .ShouldHaveError("Sugar", "Must be less than or equal to Carbohydrates.");

    [Fact]
    public void Validate_WhenFatIsNegative_HasError()
        => Validate(Valid() with { Fat = -1 }).ShouldHaveError("Fat", "Must be greater than or equal to 0.");

    [Fact]
    public void Validate_WhenSaturatedFatIsNegative_HasError()
        => Validate(Valid() with { SaturatedFat = -1 }).ShouldHaveError("SaturatedFat", "Must be greater than or equal to 0.");

    [Fact]
    public void Validate_WhenSaturatedFatExceedsFat_HasError()
        => Validate(Valid() with { Fat = 1m, SaturatedFat = 2m })
            .ShouldHaveError("SaturatedFat", "Must be less than or equal to Fat.");

    [Fact]
    public void Validate_WhenFibreIsNegative_HasError()
        => Validate(Valid() with { Fibre = -1 }).ShouldHaveError("Fibre", "Must be greater than or equal to 0.");

    [Fact]
    public void Validate_WhenSaltIsNegative_HasError()
        => Validate(Valid() with { Salt = -1 }).ShouldHaveError("Salt", "Must be greater than or equal to 0.");

    [Theory]
    [InlineData("not a url")]
    [InlineData("ftp://example.com")]
    public void Validate_WhenSourceIsInvalidUrl_HasError(string source)
        => Validate(Valid() with { Source = source }).ShouldHaveError("Source", "Must be a valid URL.");
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
        => Validate(Valid() with { Name = "" }).ShouldHaveError("Name", "Must not be null or empty.");

    [Fact]
    public void Validate_WhenIngredientsAreEmpty_HasError()
        => Validate(Valid() with { Ingredients = [] }).ShouldHaveError("Ingredients", "Must not be empty.");

    [Fact]
    public void Validate_WhenFoodIdIsEmpty_HasError()
        => Validate(Valid() with { Ingredients = [new IngredientRequest(Guid.Empty, 100)] })
            .ShouldHaveError("Ingredients[0].FoodId", "Must not be empty.");

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WhenQuantityIsNotPositive_HasError(int quantity)
        => Validate(Valid() with { Ingredients = [new IngredientRequest(FoodA, quantity)] })
            .ShouldHaveError("Ingredients[0].Quantity", "Must be greater than 0.");

    [Fact]
    public void Validate_WhenFoodIdsAreDuplicated_HasError()
        => Validate(Valid() with { Ingredients = [new IngredientRequest(FoodA, 1), new IngredientRequest(FoodA, 2)] })
            .ShouldHaveError("Ingredients", "Must not contain the same food more than once.");
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
        => Validate(Valid() with { Name = " " }).ShouldHaveError("Name", "Must not be null or empty.");

    [Fact]
    public void Validate_WhenMealsAreEmpty_HasError()
        => Validate(Valid() with { Meals = [] }).ShouldHaveError("Meals", "Must not be empty.");

    [Fact]
    public void Validate_WhenMealNameIsBlank_HasIndexedError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Name = "" }] })
            .ShouldHaveError("Meals[0].Name", "Must not be null or empty.");

    [Fact]
    public void Validate_WhenMealIngredientsAreEmpty_HasError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Ingredients = [] }] })
            .ShouldHaveError("Meals[0].Ingredients", "Must not be empty.");

    [Fact]
    public void Validate_WhenMealFoodIdIsEmpty_HasError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Ingredients = [new IngredientRequest(Guid.Empty, 1)] }] })
            .ShouldHaveError("Meals[0].Ingredients[0].FoodId", "Must not be empty.");

    [Fact]
    public void Validate_WhenMealQuantityIsNotPositive_HasError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Ingredients = [new IngredientRequest(FoodA, 0)] }] })
            .ShouldHaveError("Meals[0].Ingredients[0].Quantity", "Must be greater than 0.");

    [Fact]
    public void Validate_WhenMealFoodIdsAreDuplicated_HasError()
        => Validate(Valid() with { Meals = [ValidMeal() with { Ingredients = [new IngredientRequest(FoodA, 1), new IngredientRequest(FoodA, 2)] }] })
            .ShouldHaveError("Meals[0].Ingredients", "Must not contain the same food more than once.");
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
    public void Validate_WhenIdIsEmpty_HasError() => Validate(Guid.Empty).ShouldHaveError("Id", "Must not be empty.");
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
        => Validate("ab").ShouldHaveError("SearchTerm", "Must be at least 3 characters long.");

    [Fact]
    public void Validate_WhenThreeCharacters_HasNoErrors() => Validate("abc").ShouldHaveNoErrors();
}
