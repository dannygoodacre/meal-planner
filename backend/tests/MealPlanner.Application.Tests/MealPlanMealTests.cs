using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Tests;

public sealed class MealPlanMealTests
{
    private static MealPlanIngredient Ingredient(int foodId, int quantity) => new() { FoodId = foodId, Quantity = quantity };

    [Fact]
    public void Create_SetsPublicIdNameAndIngredients()
    {
        MealPlanIngredient[] ingredients = [Ingredient(1, 100), Ingredient(2, 50)];

        MealPlanMeal meal = MealPlanMeal.Create("Lunch", ingredients);

        Assert.NotEqual(Guid.Empty, meal.PublicId);
        Assert.Equal("Lunch", meal.Name);
        Assert.Equal(ingredients, meal.Ingredients);
    }

    [Fact]
    public void SetIngredients_UpdatesQuantityAndKeepsSameInstance()
    {
        MealPlanIngredient existing = Ingredient(1, 100);
        MealPlanMeal meal = MealPlanMeal.Create("Lunch", [existing]);

        meal.SetIngredients([Ingredient(1, 250)]);

        Assert.Same(existing, Assert.Single(meal.Ingredients));
        Assert.Equal(250, existing.Quantity);
    }

    [Fact]
    public void SetIngredients_RemovesIngredientsMissingFromInput()
    {
        MealPlanIngredient kept = Ingredient(1, 100);
        MealPlanMeal meal = MealPlanMeal.Create("Lunch", [kept, Ingredient(2, 50)]);

        meal.SetIngredients([Ingredient(1, 100)]);

        Assert.Same(kept, Assert.Single(meal.Ingredients));
    }

    [Fact]
    public void SetIngredients_AddsNewIngredients()
    {
        MealPlanIngredient added = Ingredient(2, 50);
        MealPlanMeal meal = MealPlanMeal.Create("Lunch", [Ingredient(1, 100)]);

        meal.SetIngredients([Ingredient(1, 100), added]);

        Assert.Equal(2, meal.Ingredients.Count);
        Assert.Contains(added, meal.Ingredients);
    }

    [Fact]
    public void SetIngredients_WhenInputIsTheSame_ChangesNothing()
    {
        MealPlanIngredient first = Ingredient(1, 100);
        MealPlanIngredient second = Ingredient(2, 50);
        MealPlanMeal meal = MealPlanMeal.Create("Lunch", [first, second]);

        meal.SetIngredients([Ingredient(1, 100), Ingredient(2, 50)]);

        Assert.Equal([first, second], meal.Ingredients);
        Assert.Equal(100, first.Quantity);
        Assert.Equal(50, second.Quantity);
    }
}
