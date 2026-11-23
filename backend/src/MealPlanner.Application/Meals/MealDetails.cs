using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Meals;

public interface IMealDetails
{
    string Name { get; }

    List<IngredientRequest> Ingredients { get; }
}

internal sealed class MealDetailsValidator<T> : IValidator<T> where T : IRequest, IMealDetails
{
    public void Validate(T request, ValidationState validationState)
        => MealDetailsRules.Validate(request, validationState);
}

internal static class MealDetailsRules
{
    public static void Validate(IMealDetails meal, ValidationState validationState, string prefix = "")
    {
        validationState.IsNotNullEmptyOrWhitespace(meal.Name, prefix + nameof(meal.Name));

        if (!validationState.IsNotEmpty(meal.Ingredients, prefix + nameof(meal.Ingredients)))
        {
            return;
        }

        for (var i = 0; i < meal.Ingredients.Count; i++)
        {
            IngredientRequest ingredient = meal.Ingredients[i];

            var ingredientPrefix = $"{prefix}{nameof(meal.Ingredients)}[{i}].";

            validationState.IsNonEmptyGuid(ingredient.FoodId, ingredientPrefix + nameof(ingredient.FoodId));

            validationState.IsPositive(ingredient.Quantity, ingredientPrefix + nameof(ingredient.Quantity));
        }

        validationState.AddErrorIf(
            meal.Ingredients.DistinctBy(x => x.FoodId).Count() != meal.Ingredients.Count,
            prefix + nameof(meal.Ingredients),
            new ValidationError("Must not contain the same food more than once.", "Duplicate"));
    }
}
