using DannyGoodacre.Testing;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using NUnit.Framework;

namespace MealPlanner.Application.Tests.Extensions;

[TestFixture]
public sealed class IngredientExtensionsTests : TestBase
{
    [Test]
    public void ToResponse()
    {
        // Arrange
        Ingredient ingredient = new()
        {
            FoodId = 123,
            MealId = 456,
            Quantity = 789,
            Food = new Food
            {
                PublicId = Guid.NewGuid(),
                Name = "Test Food Name",
                NormalizedName = "Test Food Normalized Name",
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
                Source = "Test Source"
            },
            Meal = new Meal
            {
                PublicId = Guid.NewGuid(),
                Name = "Test Meal Name",
                NormalizedName = "Test Meal Normalized Name",
            }
        };

        // Act
        IngredientResponse result = ingredient.ToResponse();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Quantity, Is.EqualTo(ingredient.Quantity));

            Assert.That(result.Food.Id, Is.EqualTo(ingredient.Food.PublicId));
            Assert.That(result.Food.Name, Is.EqualTo(ingredient.Food.Name));
            Assert.That(result.Food.NormalizedName, Is.EqualTo(ingredient.Food.NormalizedName));
            Assert.That(result.Food.Unit, Is.EqualTo(ingredient.Food.Unit));
            Assert.That(result.Food.ReferenceQuantity, Is.EqualTo(ingredient.Food.ReferenceQuantity));
            Assert.That(result.Food.Calories, Is.EqualTo(ingredient.Food.Calories));
            Assert.That(result.Food.Protein, Is.EqualTo(ingredient.Food.Protein));
            Assert.That(result.Food.Carbohydrates, Is.EqualTo(ingredient.Food.Carbohydrates));
            Assert.That(result.Food.Sugar, Is.EqualTo(ingredient.Food.Sugar));
            Assert.That(result.Food.Fat, Is.EqualTo(ingredient.Food.Fat));
            Assert.That(result.Food.SaturatedFat, Is.EqualTo(ingredient.Food.SaturatedFat));
            Assert.That(result.Food.Fibre, Is.EqualTo(ingredient.Food.Fibre));
            Assert.That(result.Food.Salt, Is.EqualTo(ingredient.Food.Salt));
            Assert.That(result.Food.Source, Is.EqualTo(ingredient.Food.Source));
        }
    }
}
