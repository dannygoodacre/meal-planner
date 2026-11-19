using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Commands;

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

/// <summary>
/// Shared by <see cref="MealDetailsValidator{T}"/> and <see cref="MealPlanDetailsValidator{T}"/>, which applies it to each meal.
/// </summary>
internal static class MealDetailsRules
{
    public static void Validate(IMealDetails meal, ValidationState validationState, string prefix = "")
    {
        validationState.IsNotNullEmptyOrWhitespace(meal.Name, prefix + nameof(meal.Name));

        if (!validationState.IsNotEmpty(meal.Ingredients, prefix + nameof(meal.Ingredients)))
        {
            return;
        }

        for (int i = 0; i < meal.Ingredients.Count; i++)
        {
            IngredientRequest ingredient = meal.Ingredients[i];

            string ingredientPrefix = $"{prefix}{nameof(meal.Ingredients)}[{i}].";

            validationState.IsNonEmptyGuid(ingredient.FoodId, ingredientPrefix + nameof(ingredient.FoodId));

            validationState.IsPositive(ingredient.Quantity, ingredientPrefix + nameof(ingredient.Quantity));
        }

        validationState.AddErrorIf(
            meal.Ingredients.DistinctBy(x => x.FoodId).Count() != meal.Ingredients.Count,
            prefix + nameof(meal.Ingredients),
            "Must not contain the same food more than once.");
    }
}
