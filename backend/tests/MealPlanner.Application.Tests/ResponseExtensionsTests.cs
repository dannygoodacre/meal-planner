using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Tests;

public sealed class ResponseExtensionsTests
{
    private static Food Food(string name = "Oats") => new()
    {
        PublicId = Guid.NewGuid(),
        Name = name,
        Unit = Unit.Grams,
        ReferenceQuantity = 100,
        Calories = 380,
        Protein = 13.5m,
        Carbohydrates = 60.1m,
        Sugar = 1.2m,
        Fat = 7.3m,
        SaturatedFat = 1.4m,
        Fibre = 10.5m,
        Salt = 0.02m,
        Source = "Packet"
    };

    private static FoodResponse ExpectedFood(Food food) => new()
    {
        Id = food.PublicId,
        Name = food.Name,
        NormalizedName = food.NormalizedName,
        Unit = Unit.Grams,
        ReferenceQuantity = 100,
        Calories = 380,
        Protein = 13.5m,
        Carbohydrates = 60.1m,
        Sugar = 1.2m,
        Fat = 7.3m,
        SaturatedFat = 1.4m,
        Fibre = 10.5m,
        Salt = 0.02m,
        Source = "Packet"
    };

    [Fact]
    public void Food_MapsEveryField()
    {
        Food food = Food();

        Assert.Equal(ExpectedFood(food), food.ToResponse());
        Assert.Equal("oats", food.ToResponse().NormalizedName);
    }

    [Fact]
    public void Ingredient_MapsQuantityAndFood()
    {
        Food food = Food();

        IngredientResponse response = new Ingredient { Quantity = 75, Food = food }.ToResponse();

        Assert.Equal(75, response.Quantity);
        Assert.Equal(ExpectedFood(food), response.Food);
    }

    [Fact]
    public void MealPlanIngredient_MapsQuantityAndFood()
    {
        Food food = Food();

        IngredientResponse response = new MealPlanIngredient { Quantity = 75, Food = food }.ToResponse();

        Assert.Equal(75, response.Quantity);
        Assert.Equal(ExpectedFood(food), response.Food);
    }

    [Fact]
    public void Meal_MapsIdNameAndIngredients()
    {
        Food food = Food();
        Meal meal = new()
        {
            PublicId = Guid.NewGuid(),
            Name = "Porridge",
            Ingredients = [new Ingredient { Quantity = 50, Food = food }]
        };

        MealResponse response = meal.ToResponse();

        Assert.Equal(meal.PublicId, response.Id);
        Assert.Equal("Porridge", response.Name);
        IngredientResponse ingredient = Assert.Single(response.Ingredients);
        Assert.Equal(50, ingredient.Quantity);
        Assert.Equal(ExpectedFood(food), ingredient.Food);
    }

    [Fact]
    public void MealPlanMeal_MapsIdNameAndIngredients()
    {
        Food food = Food();
        MealPlanMeal meal = MealPlanMeal.Create("Lunch", [new MealPlanIngredient { Quantity = 50, Food = food }]);

        MealResponse response = meal.ToResponse();

        Assert.Equal(meal.PublicId, response.Id);
        Assert.Equal("Lunch", response.Name);
        IngredientResponse ingredient = Assert.Single(response.Ingredients);
        Assert.Equal(50, ingredient.Quantity);
        Assert.Equal(ExpectedFood(food), ingredient.Food);
    }

    [Fact]
    public void MealPlan_MapsIdNameAndMeals()
    {
        Food food = Food();
        MealPlanMeal meal = MealPlanMeal.Create("Lunch", [new MealPlanIngredient { Quantity = 50, Food = food }]);
        MealPlan plan = new() { PublicId = Guid.NewGuid(), Name = "Week 1", Meals = [meal] };

        MealPlanResponse response = plan.ToResponse();

        Assert.Equal(plan.PublicId, response.Id);
        Assert.Equal("Week 1", response.Name);
        MealResponse mapped = Assert.Single(response.Meals);
        Assert.Equal(meal.PublicId, mapped.Id);
        Assert.Equal("Lunch", mapped.Name);
        Assert.Equal(ExpectedFood(food), Assert.Single(mapped.Ingredients).Food);
    }
}
