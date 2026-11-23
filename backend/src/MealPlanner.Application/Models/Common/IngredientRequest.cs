namespace MealPlanner.Application.Models;

public sealed record IngredientRequest(Guid FoodId, int Quantity);
