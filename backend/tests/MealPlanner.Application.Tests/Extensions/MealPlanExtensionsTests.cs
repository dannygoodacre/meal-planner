using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using NUnit.Framework;

namespace MealPlanner.Application.Tests.Extensions;

[TestFixture]
public sealed class MealPlanExtensionsTests
{
    [Test]
    public void ToResponse()
    {
        // Arrange
        MealPlan mealPlan = new()
        {
            PublicId = Guid.NewGuid(),
            Name = "Test Meal Plan Name",
            NormalizedName = "Test Meal Plan Normalized Name",
            Meals = [
                new MealPlanMeal
                {
                    PublicId = Guid.NewGuid(),
                    Name = "Test Meal Name 1",
                    NormalizedName = "Test Meal Normalized Name 1",
                    MealPlanId = 123,
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
                },
                new MealPlanMeal
                {
                    PublicId = Guid.NewGuid(),
                    Name = "Test Meal Name 2",
                    NormalizedName = "Test Meal Normalized Name 2",
                    MealPlanId = 456,
                    Ingredients = [
                        new MealPlanIngredient
                        {
                            Quantity = 300,
                            Food = new Food
                            {
                                Unit = Unit.Grams,
                                ReferenceQuantity = 19,
                                Calories = 20,
                                Protein = 21,
                                Carbohydrates = 22,
                                Sugar = 23,
                                Fat = 24,
                                SaturatedFat = 25,
                                Fibre = 26,
                                Salt = 27,
                                Source = "Test Food Source 3",
                                PublicId = Guid.NewGuid(),
                                Name = "Test Food Name 3",
                                NormalizedName = "Test Food Normalized Name 3",
                            }
                        },
                        new MealPlanIngredient
                        {
                            Quantity = 400,
                            Food = new Food
                            {
                                Unit = Unit.Count,
                                ReferenceQuantity = 28,
                                Calories = 29,
                                Protein = 30,
                                Carbohydrates = 31,
                                Sugar = 32,
                                Fat = 33,
                                SaturatedFat = 34,
                                Fibre = 35,
                                Salt = 36,
                                Source = "Test Food Source 4",
                                PublicId = Guid.NewGuid(),
                                Name = "Test Food Name 4",
                                NormalizedName = "Test Food Normalized Name 4",
                            }
                        }
                    ]
                }
            ]
        };

        // Act
        MealPlanResponse result = mealPlan.ToResponse();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(mealPlan.PublicId));
            Assert.That(result.Name, Is.EqualTo(mealPlan.Name));

            Assert.That(result.Meals, Has.Count.EqualTo(mealPlan.Meals.Count));

            List<MealPlanMeal> meals = mealPlan.Meals.ToList();

            Assert.That(result.Meals[0].Id, Is.EqualTo(meals[0].PublicId));
            Assert.That(result.Meals[0].Name, Is.EqualTo(meals[0].Name));

            List<MealPlanIngredient> mealIngredients1 = meals[0].Ingredients.ToList();

            Assert.That(result.Meals[0].Ingredients, Has.Count.EqualTo(mealIngredients1.Count));

            Assert.That(result.Meals[0].Ingredients[0].Quantity, Is.EqualTo(mealIngredients1[0].Quantity));
            Assert.That(result.Meals[0].Ingredients[0].Food.Id, Is.EqualTo(mealIngredients1[0].Food.PublicId));
            Assert.That(result.Meals[0].Ingredients[0].Food.Name, Is.EqualTo(mealIngredients1[0].Food.Name));
            Assert.That(result.Meals[0].Ingredients[0].Food.NormalizedName, Is.EqualTo(mealIngredients1[0].Food.NormalizedName));
            Assert.That(result.Meals[0].Ingredients[0].Food.Unit, Is.EqualTo(mealIngredients1[0].Food.Unit));
            Assert.That(result.Meals[0].Ingredients[0].Food.ReferenceQuantity, Is.EqualTo(mealIngredients1[0].Food.ReferenceQuantity));
            Assert.That(result.Meals[0].Ingredients[0].Food.Calories, Is.EqualTo(mealIngredients1[0].Food.Calories));
            Assert.That(result.Meals[0].Ingredients[0].Food.Protein, Is.EqualTo(mealIngredients1[0].Food.Protein));
            Assert.That(result.Meals[0].Ingredients[0].Food.Carbohydrates, Is.EqualTo(mealIngredients1[0].Food.Carbohydrates));
            Assert.That(result.Meals[0].Ingredients[0].Food.Sugar, Is.EqualTo(mealIngredients1[0].Food.Sugar));
            Assert.That(result.Meals[0].Ingredients[0].Food.Fat, Is.EqualTo(mealIngredients1[0].Food.Fat));
            Assert.That(result.Meals[0].Ingredients[0].Food.SaturatedFat, Is.EqualTo(mealIngredients1[0].Food.SaturatedFat));
            Assert.That(result.Meals[0].Ingredients[0].Food.Fibre, Is.EqualTo(mealIngredients1[0].Food.Fibre));
            Assert.That(result.Meals[0].Ingredients[0].Food.Salt, Is.EqualTo(mealIngredients1[0].Food.Salt));
            Assert.That(result.Meals[0].Ingredients[0].Food.Source, Is.EqualTo(mealIngredients1[0].Food.Source));

            Assert.That(result.Meals[0].Ingredients[1].Quantity, Is.EqualTo(mealIngredients1[1].Quantity));
            Assert.That(result.Meals[0].Ingredients[1].Food.Id, Is.EqualTo(mealIngredients1[1].Food.PublicId));
            Assert.That(result.Meals[0].Ingredients[1].Food.Name, Is.EqualTo(mealIngredients1[1].Food.Name));
            Assert.That(result.Meals[0].Ingredients[1].Food.NormalizedName, Is.EqualTo(mealIngredients1[1].Food.NormalizedName));
            Assert.That(result.Meals[0].Ingredients[1].Food.Unit, Is.EqualTo(mealIngredients1[1].Food.Unit));
            Assert.That(result.Meals[0].Ingredients[1].Food.ReferenceQuantity, Is.EqualTo(mealIngredients1[1].Food.ReferenceQuantity));
            Assert.That(result.Meals[0].Ingredients[1].Food.Calories, Is.EqualTo(mealIngredients1[1].Food.Calories));
            Assert.That(result.Meals[0].Ingredients[1].Food.Protein, Is.EqualTo(mealIngredients1[1].Food.Protein));
            Assert.That(result.Meals[0].Ingredients[1].Food.Carbohydrates, Is.EqualTo(mealIngredients1[1].Food.Carbohydrates));
            Assert.That(result.Meals[0].Ingredients[1].Food.Sugar, Is.EqualTo(mealIngredients1[1].Food.Sugar));
            Assert.That(result.Meals[0].Ingredients[1].Food.Fat, Is.EqualTo(mealIngredients1[1].Food.Fat));
            Assert.That(result.Meals[0].Ingredients[1].Food.SaturatedFat, Is.EqualTo(mealIngredients1[1].Food.SaturatedFat));
            Assert.That(result.Meals[0].Ingredients[1].Food.Fibre, Is.EqualTo(mealIngredients1[1].Food.Fibre));
            Assert.That(result.Meals[0].Ingredients[1].Food.Salt, Is.EqualTo(mealIngredients1[1].Food.Salt));
            Assert.That(result.Meals[0].Ingredients[1].Food.Source, Is.EqualTo(mealIngredients1[1].Food.Source));


            Assert.That(result.Meals[1].Id, Is.EqualTo(meals[1].PublicId));
            Assert.That(result.Meals[1].Name, Is.EqualTo(meals[1].Name));

            List<MealPlanIngredient> mealIngredients2 = meals[1].Ingredients.ToList();

            Assert.That(result.Meals[1].Ingredients, Has.Count.EqualTo(mealIngredients2.Count));

            Assert.That(result.Meals[1].Ingredients[0].Quantity, Is.EqualTo(mealIngredients2[0].Quantity));
            Assert.That(result.Meals[1].Ingredients[0].Food.Id, Is.EqualTo(mealIngredients2[0].Food.PublicId));
            Assert.That(result.Meals[1].Ingredients[0].Food.Name, Is.EqualTo(mealIngredients2[0].Food.Name));
            Assert.That(result.Meals[1].Ingredients[0].Food.NormalizedName, Is.EqualTo(mealIngredients2[0].Food.NormalizedName));
            Assert.That(result.Meals[1].Ingredients[0].Food.Unit, Is.EqualTo(mealIngredients2[0].Food.Unit));
            Assert.That(result.Meals[1].Ingredients[0].Food.ReferenceQuantity, Is.EqualTo(mealIngredients2[0].Food.ReferenceQuantity));
            Assert.That(result.Meals[1].Ingredients[0].Food.Calories, Is.EqualTo(mealIngredients2[0].Food.Calories));
            Assert.That(result.Meals[1].Ingredients[0].Food.Protein, Is.EqualTo(mealIngredients2[0].Food.Protein));
            Assert.That(result.Meals[1].Ingredients[0].Food.Carbohydrates, Is.EqualTo(mealIngredients2[0].Food.Carbohydrates));
            Assert.That(result.Meals[1].Ingredients[0].Food.Sugar, Is.EqualTo(mealIngredients2[0].Food.Sugar));
            Assert.That(result.Meals[1].Ingredients[0].Food.Fat, Is.EqualTo(mealIngredients2[0].Food.Fat));
            Assert.That(result.Meals[1].Ingredients[0].Food.SaturatedFat, Is.EqualTo(mealIngredients2[0].Food.SaturatedFat));
            Assert.That(result.Meals[1].Ingredients[0].Food.Fibre, Is.EqualTo(mealIngredients2[0].Food.Fibre));
            Assert.That(result.Meals[1].Ingredients[0].Food.Salt, Is.EqualTo(mealIngredients2[0].Food.Salt));
            Assert.That(result.Meals[1].Ingredients[0].Food.Source, Is.EqualTo(mealIngredients2[0].Food.Source));

            Assert.That(result.Meals[1].Ingredients[1].Quantity, Is.EqualTo(mealIngredients2[1].Quantity));
            Assert.That(result.Meals[1].Ingredients[1].Food.Id, Is.EqualTo(mealIngredients2[1].Food.PublicId));
            Assert.That(result.Meals[1].Ingredients[1].Food.Name, Is.EqualTo(mealIngredients2[1].Food.Name));
            Assert.That(result.Meals[1].Ingredients[1].Food.NormalizedName, Is.EqualTo(mealIngredients2[1].Food.NormalizedName));
            Assert.That(result.Meals[1].Ingredients[1].Food.Unit, Is.EqualTo(mealIngredients2[1].Food.Unit));
            Assert.That(result.Meals[1].Ingredients[1].Food.ReferenceQuantity, Is.EqualTo(mealIngredients2[1].Food.ReferenceQuantity));
            Assert.That(result.Meals[1].Ingredients[1].Food.Calories, Is.EqualTo(mealIngredients2[1].Food.Calories));
            Assert.That(result.Meals[1].Ingredients[1].Food.Protein, Is.EqualTo(mealIngredients2[1].Food.Protein));
            Assert.That(result.Meals[1].Ingredients[1].Food.Carbohydrates, Is.EqualTo(mealIngredients2[1].Food.Carbohydrates));
            Assert.That(result.Meals[1].Ingredients[1].Food.Sugar, Is.EqualTo(mealIngredients2[1].Food.Sugar));
            Assert.That(result.Meals[1].Ingredients[1].Food.Fat, Is.EqualTo(mealIngredients2[1].Food.Fat));
            Assert.That(result.Meals[1].Ingredients[1].Food.SaturatedFat, Is.EqualTo(mealIngredients2[1].Food.SaturatedFat));
            Assert.That(result.Meals[1].Ingredients[1].Food.Fibre, Is.EqualTo(mealIngredients2[1].Food.Fibre));
            Assert.That(result.Meals[1].Ingredients[1].Food.Salt, Is.EqualTo(mealIngredients2[1].Food.Salt));
            Assert.That(result.Meals[1].Ingredients[1].Food.Source, Is.EqualTo(mealIngredients2[1].Food.Source));
        }
    }
}
