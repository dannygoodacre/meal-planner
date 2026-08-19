using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using MealPlanner.Application.Models;
using NUnit.Framework;

namespace MealPlanner.Application.Tests.Extensions;

[TestFixture]
public sealed class MealExtensionsTests
{
    [Test]
    public void ToResponse()
    {
        // Arrange
        Meal meal = new()
        {
            PublicId = Guid.NewGuid(),
            Name = "Test Meal Name",
            NormalizedName = "Test Meal Normalized Name",
            Ingredients =
            [
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
                    },
                    Meal = new Meal
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Test Meal Name 1",
                        NormalizedName = "Test Meal Normalized Name 1",
                    }
                },
                new Ingredient
                {
                    FoodId = 101,
                    MealId = 112,
                    Quantity = 131,
                    Food = new Food
                    {
                        Unit = Unit.Count,
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
                    },
                    Meal = new Meal
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Test Meal Name 2",
                        NormalizedName = "Test Meal Normalized Name 2",
                    }
                },
            ]
        };

        // Act
        MealResponse result = meal.ToResponse();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(meal.PublicId));
            Assert.That(result.Name, Is.EqualTo(meal.Name));

            List<Ingredient> ingredients = meal.Ingredients.ToList();

            Assert.That(result.Ingredients, Has.Count.EqualTo(ingredients.Count));

            Assert.That(result.Ingredients[0].Quantity, Is.EqualTo(ingredients[0].Quantity));

            Assert.That(result.Ingredients[0].Food.Id, Is.EqualTo(ingredients[0].Food.PublicId));
            Assert.That(result.Ingredients[0].Food.Name, Is.EqualTo(ingredients[0].Food.Name));
            Assert.That(result.Ingredients[0].Food.NormalizedName, Is.EqualTo(ingredients[0].Food.NormalizedName));
            Assert.That(result.Ingredients[0].Food.Unit, Is.EqualTo(ingredients[0].Food.Unit));
            Assert.That(result.Ingredients[0].Food.ReferenceQuantity, Is.EqualTo(ingredients[0].Food.ReferenceQuantity));
            Assert.That(result.Ingredients[0].Food.Calories, Is.EqualTo(ingredients[0].Food.Calories));
            Assert.That(result.Ingredients[0].Food.Protein, Is.EqualTo(ingredients[0].Food.Protein));
            Assert.That(result.Ingredients[0].Food.Carbohydrates, Is.EqualTo(ingredients[0].Food.Carbohydrates));
            Assert.That(result.Ingredients[0].Food.Sugar, Is.EqualTo(ingredients[0].Food.Sugar));
            Assert.That(result.Ingredients[0].Food.Fat, Is.EqualTo(ingredients[0].Food.Fat));
            Assert.That(result.Ingredients[0].Food.SaturatedFat, Is.EqualTo(ingredients[0].Food.SaturatedFat));
            Assert.That(result.Ingredients[0].Food.Fibre, Is.EqualTo(ingredients[0].Food.Fibre));
            Assert.That(result.Ingredients[0].Food.Salt, Is.EqualTo(ingredients[0].Food.Salt));
            Assert.That(result.Ingredients[0].Food.Source, Is.EqualTo(ingredients[0].Food.Source));

            Assert.That(result.Ingredients[1].Quantity, Is.EqualTo(ingredients[1].Quantity));

            Assert.That(result.Ingredients[1].Food.Id, Is.EqualTo(ingredients[1].Food.PublicId));
            Assert.That(result.Ingredients[1].Food.Name, Is.EqualTo(ingredients[1].Food.Name));
            Assert.That(result.Ingredients[1].Food.NormalizedName, Is.EqualTo(ingredients[1].Food.NormalizedName));
            Assert.That(result.Ingredients[1].Food.Unit, Is.EqualTo(ingredients[1].Food.Unit));
            Assert.That(result.Ingredients[1].Food.ReferenceQuantity, Is.EqualTo(ingredients[1].Food.ReferenceQuantity));
            Assert.That(result.Ingredients[1].Food.Calories, Is.EqualTo(ingredients[1].Food.Calories));
            Assert.That(result.Ingredients[1].Food.Protein, Is.EqualTo(ingredients[1].Food.Protein));
            Assert.That(result.Ingredients[1].Food.Carbohydrates, Is.EqualTo(ingredients[1].Food.Carbohydrates));
            Assert.That(result.Ingredients[1].Food.Sugar, Is.EqualTo(ingredients[1].Food.Sugar));
            Assert.That(result.Ingredients[1].Food.Fat, Is.EqualTo(ingredients[1].Food.Fat));
            Assert.That(result.Ingredients[1].Food.SaturatedFat, Is.EqualTo(ingredients[1].Food.SaturatedFat));
            Assert.That(result.Ingredients[1].Food.Fibre, Is.EqualTo(ingredients[1].Food.Fibre));
            Assert.That(result.Ingredients[1].Food.Salt, Is.EqualTo(ingredients[1].Food.Salt));
            Assert.That(result.Ingredients[1].Food.Source, Is.EqualTo(ingredients[1].Food.Source));
        }
    }

    [Test]
    public void ToMealPlanMeal()
    {
        // Arrange
        Meal meal = new()
        {
            PublicId = Guid.NewGuid(),
            Name = "Test Meal Name",
            NormalizedName = "Test Meal Normalized Name",
            Ingredients =
            [
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
                    },
                    Meal = new Meal
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Test Meal Name 1",
                        NormalizedName = "Test Meal Normalized Name 1",
                    }
                },
                new Ingredient
                {
                    FoodId = 101,
                    MealId = 112,
                    Quantity = 131,
                    Food = new Food
                    {
                        Unit = Unit.Count,
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
                    },
                    Meal = new Meal
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Test Meal Name 2",
                        NormalizedName = "Test Meal Normalized Name 2",
                    }
                },
            ]
        };

        // Act
        MealPlanMeal result = meal.ToMealPlanMeal();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.PublicId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Name, Is.EqualTo("Test Meal Name"));
            Assert.That(result.NormalizedName, Is.EqualTo("Test Meal Normalized Name"));

            List<Ingredient> ingredients = meal.Ingredients.ToList();

            List<MealPlanIngredient> resultIngredients = result.Ingredients.ToList();


            Assert.That(resultIngredients, Has.Count.EqualTo(resultIngredients.Count));

            Assert.That(resultIngredients[0].FoodId, Is.EqualTo(ingredients[0].FoodId));
            Assert.That(resultIngredients[0].Quantity, Is.EqualTo(ingredients[0].Quantity));

            Assert.That(resultIngredients[1].FoodId, Is.EqualTo(ingredients[1].FoodId));
            Assert.That(resultIngredients[1].Quantity, Is.EqualTo(ingredients[1].Quantity));
        }
    }
}
