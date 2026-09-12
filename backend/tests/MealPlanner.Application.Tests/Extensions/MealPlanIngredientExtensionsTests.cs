using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using MealPlanner.Application.Models;
using NUnit.Framework;

namespace MealPlanner.Application.Tests.Extensions;

[TestFixture]
public sealed class MealPlanIngredientExtensionsTests
{
    [Test]
    public void ToResponse()
    {
        // Arrange
        MealPlanIngredient ingredient = new()
        {
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
