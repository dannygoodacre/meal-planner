using DannyGoodacre.Testing;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using NUnit.Framework;

namespace MealPlanner.Application.Tests.Extensions;

[TestFixture]
public sealed class FoodExtensionsTests : TestBase
{
    [Test]
    public void ToResponse()
    {
        // Arrange
        Food food = new()
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
            Source = "Test Source",
            PublicId = Guid.NewGuid(),
            Name = "Test Name",
            NormalizedName = "Test Normalized Name",
        };

        // Act
        FoodResponse result = food.ToResponse();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Unit, Is.EqualTo(food.Unit));
            Assert.That(result.ReferenceQuantity, Is.EqualTo(food.ReferenceQuantity));
            Assert.That(result.Calories, Is.EqualTo(food.Calories));
            Assert.That(result.Protein, Is.EqualTo(food.Protein));
            Assert.That(result.Carbohydrates, Is.EqualTo(food.Carbohydrates));
            Assert.That(result.Sugar, Is.EqualTo(food.Sugar));
            Assert.That(result.Fat, Is.EqualTo(food.Fat));
            Assert.That(result.SaturatedFat, Is.EqualTo(food.SaturatedFat));
            Assert.That(result.Fibre, Is.EqualTo(food.Fibre));
            Assert.That(result.Salt, Is.EqualTo(food.Salt));
            Assert.That(result.Source, Is.EqualTo(food.Source));
            Assert.That(result.Id, Is.EqualTo(food.PublicId));
            Assert.That(result.Name, Is.EqualTo(food.Name));
            Assert.That(result.NormalizedName, Is.EqualTo(food.NormalizedName));
        }
    }
}
