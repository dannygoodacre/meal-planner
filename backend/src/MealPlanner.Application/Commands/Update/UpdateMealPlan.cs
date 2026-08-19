using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public sealed record UpdateMealPlanCommand : ICommand
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required List<UpdateMealPlanMealRequest> Meals { get; init; }
}

public interface IUpdateMealPlan
{
    Task<Result<Guid>> ExecuteAsync(UpdateMealPlanCommand command, CancellationToken cancellationToken = default);
}

internal sealed partial class UpdateMealPlanHandler(ILogger<UpdateMealPlanHandler> logger,
                                                    IStateUnit stateUnit,
                                                    IMealPlanRepository mealPlanRepository,
                                                    IMealRepository mealRepository,
                                                    IFoodRepository foodRepository)
    : StateCommandHandler<UpdateMealPlanCommand, Guid>(logger, stateUnit), IUpdateMealPlan
{
    protected override string CommandName => "Update Meal Plan";

    protected override void Validate(ValidationState state, UpdateMealPlanCommand command)
    {
        state.IsNonEmptyGuid(command.Id, nameof(command.Id));

        state.IsNotNullEmptyOrWhitespace(command.Name, nameof(command.Name));

        state.IsNotEmpty(command.Meals, nameof(command.Meals));

        command.Meals.ForEach(meal =>
        {
            bool isUpdate = meal.Id is not null && meal.Id.Value != Guid.Empty;

            bool isTemplate = meal.TemplateMealId is not null && meal.TemplateMealId.Value != Guid.Empty;

            if (isUpdate && isTemplate)
            {
                state.AddError(nameof(meal.Id), "Cannot provide both an existing meal ID and a template meal ID.");

                return;
            }

            if (isUpdate && (meal.Id is null || meal.Id == Guid.Empty))
            {
                state.AddError(nameof(meal.Id), "Must not be an empty GUID.");
            }

            if (isTemplate && (meal.TemplateMealId is null || meal.TemplateMealId == Guid.Empty))
            {
                state.AddError(nameof(meal.TemplateMealId), "Must not be an empty GUID.");
            }

            if (isTemplate)
            {
                // Template meal ID provided, with optional Name and Ingredient overrides.

                if (meal.Name is not null)
                {
                    state.IsNotNullEmptyOrWhitespace(meal.Name, nameof(meal.Name));
                }
            }
            else
            {
                // Existing meal in plan being updated or new meal with no template.

                state.IsNotNullEmptyOrWhitespace(meal.Name, nameof(meal.Name));

                state.IsNotEmpty(meal.Ingredients, nameof(meal.Ingredients));
            }

            meal.Ingredients.ForEach(ingredient =>
            {
                state.IsNonEmptyGuid(ingredient.FoodId, nameof(ingredient.FoodId));

                state.IsPositive(ingredient.Quantity, nameof(ingredient.Quantity));
            });
        });
    }

    protected async override Task<Result<Guid>> InternalExecuteAsync(UpdateMealPlanCommand command, CancellationToken cancellationToken = default)
    {
        LogCommandStarted(logger, CommandName, command.Id);

        MealPlan? mealPlan = await mealPlanRepository.GetWithTrackingAsync(command.Id, cancellationToken);

        if (mealPlan is null)
        {
            return Result<Guid>.NotFound();
        }

        bool doesMealPlanHaveSameName = mealPlan.Name.ToNormalizedString() != command.Name.ToNormalizedString();

        bool isNameAlreadyInUse = await mealPlanRepository.ExistsByNameAsync(command.Name, cancellationToken);

        if (doesMealPlanHaveSameName && isNameAlreadyInUse)
        {
            return Result<Guid>.Conflict("A meal plan already exists by that name");
        }

        List<Guid> incomingIngredientIds = command.Meals
            .SelectMany(x => x.Ingredients)
            .Select(x => x.FoodId)
            .Distinct()
            .ToList();

        Dictionary<Guid, int> foodIdMap = await foodRepository.GetIdMappingAsync(incomingIngredientIds, cancellationToken);

        List<Guid> missingFoodIds = incomingIngredientIds.Except(foodIdMap.Keys).ToList();

        if (missingFoodIds.Count > 0)
        {
            return Result<Guid>.DomainError($"The following foods were not found: {string.Join(", ", missingFoodIds)}.");
        }

        List<Guid> templateIdsToFetch = command.Meals
            .Where(x => x.TemplateMealId is not null && x.TemplateMealId.Value != Guid.Empty)
            .Select(x => x.TemplateMealId!.Value)
            .Distinct()
            .ToList();

        List<Meal> templateMeals = await mealRepository.GetManyAsync(templateIdsToFetch, cancellationToken);

        Dictionary<Guid, Meal> templateMealsMap = templateMeals.ToDictionary(x => x.PublicId);

        List<Guid> missingTemplateMealIds = templateIdsToFetch.Except(templateMealsMap.Keys).ToList();

        if (missingTemplateMealIds.Count > 0)
        {
            return Result<Guid>.DomainError($"The following meals were not found: {string.Join(", ", missingTemplateMealIds)}.");
        }

        Dictionary<Guid, MealPlanMeal> existingMealsMap = mealPlan.Meals.ToDictionary(x => x.PublicId);

        List<Guid> missingMealPlanMealIds = [];

        foreach (UpdateMealPlanMealRequest incomingMeal in command.Meals)
        {
            bool isTemplate = incomingMeal.TemplateMealId is not null && incomingMeal.TemplateMealId.Value != Guid.Empty;

            bool isUpdate = incomingMeal.Id is not null && incomingMeal.Id.Value != Guid.Empty;

            if (isTemplate)
            {
                // New meal plan meal from template

                Meal templateMeal = templateMealsMap[incomingMeal.TemplateMealId!.Value];

                MealPlanMeal clonedMeal = templateMeal.ToMealPlanMeal();

                // Optional name override
                if (!string.IsNullOrWhiteSpace(incomingMeal.Name))
                {
                    clonedMeal.Name = incomingMeal.Name;
                    clonedMeal.NormalizedName = incomingMeal.Name.ToNormalizedString();
                }

                // Optional ingredients override
                if (incomingMeal.Ingredients.Count > 0)
                {
                    clonedMeal.Ingredients.Clear();

                    foreach (IngredientRequest ingredientOverride in incomingMeal.Ingredients)
                    {
                        clonedMeal.Ingredients.Add(new MealPlanIngredient
                        {
                            FoodId = foodIdMap[ingredientOverride.FoodId],
                            Quantity = ingredientOverride.Quantity
                        });
                    }
                }

                mealPlan.Meals.Add(clonedMeal);
            }
            else if (isUpdate)
            {
                // Update Existing meal plan meal

                if (existingMealsMap.TryGetValue(incomingMeal.Id!.Value, out MealPlanMeal? existingMeal))
                {
                    existingMeal.Name = incomingMeal.Name!;
                    existingMeal.NormalizedName = incomingMeal.Name!.ToNormalizedString();

                    Dictionary<int, IngredientRequest> incomingIngredientsMap = incomingMeal.Ingredients.ToDictionary(x => foodIdMap[x.FoodId]);

                    List<MealPlanIngredient> ingredientsToRemove = [];

                    foreach (MealPlanIngredient existingIngredient in existingMeal.Ingredients)
                    {
                        if (incomingIngredientsMap.TryGetValue(existingIngredient.FoodId, out IngredientRequest? incomingIngredient))
                        {
                            existingIngredient.Quantity = incomingIngredient.Quantity;

                            incomingIngredientsMap.Remove(existingIngredient.FoodId);
                        }
                        else
                        {
                            ingredientsToRemove.Add(existingIngredient);
                        }
                    }

                    foreach (MealPlanIngredient ingredient in ingredientsToRemove)
                    {
                        existingMeal.Ingredients.Remove(ingredient);
                    }

                    foreach (var incomingIngredientKvp in incomingIngredientsMap)
                    {
                        existingMeal.Ingredients.Add(new MealPlanIngredient
                        {
                            FoodId = incomingIngredientKvp.Key,
                            Quantity = incomingIngredientKvp.Value.Quantity
                        });
                    }

                    // Remove from the map once processed so we know it's accounted for
                    existingMealsMap.Remove(incomingMeal.Id!.Value);
                }
                else
                {
                    missingMealPlanMealIds.Add(incomingMeal.Id!.Value);
                }
            }
            else
            {
                // New custom meal (no template)

                mealPlan.Meals.Add(new MealPlanMeal
                {
                    PublicId = Guid.NewGuid(),
                    Name = incomingMeal.Name!,
                    NormalizedName = incomingMeal.Name!.ToNormalizedString(),
                    Ingredients = incomingMeal.Ingredients.Select(x => new MealPlanIngredient
                    {
                        FoodId = foodIdMap[x.FoodId],
                        Quantity = x.Quantity
                    }).ToList()
                });
            }
        }

        if (missingMealPlanMealIds.Count > 0)
        {
            return Result<Guid>.DomainError($"The following meals were not found in the meal plan: {string.Join(", ", missingMealPlanMealIds)}.");
        }

        // Remove any meals that existed in the database but weren't in the incoming request
        foreach (MealPlanMeal orphanedMeal in existingMealsMap.Values)
        {
            mealPlan.Meals.Remove(orphanedMeal);
        }

        mealPlan.Name = command.Name;

        mealPlan.NormalizedName = command.Name.ToNormalizedString();

        return Result.Success(mealPlan.PublicId);
    }

    public new Task<Result<Guid>> ExecuteAsync(UpdateMealPlanCommand command, CancellationToken cancellationToken = default)
        => base.ExecuteAsync(command, cancellationToken);

    [LoggerMessage(LogLevel.Information, "Command '{Command}' started for MealPlan '{MealPlanId}'.")]
    private static partial void LogCommandStarted(ILogger logger, string command, Guid mealPlanId);
}
