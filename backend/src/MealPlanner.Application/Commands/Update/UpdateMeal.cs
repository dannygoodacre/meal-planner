using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public sealed record UpdateMealCommand : AddMealCommand
{
    public required Guid Id { get; init; }
}

public interface IUpdateMeal
{
    Task<IResult<Guid>> ExecuteAsync(UpdateMealCommand command, CancellationToken cancellationToken = default);
}

internal sealed partial class UpdateMealHandler(ILogger<UpdateMealHandler> logger,
                                                IStateUnit stateUnit,
                                                IFoodRepository foodRepository,
                                                IMealRepository mealRepository)
    : StateCommandHandler<UpdateMealCommand, Guid>(logger, stateUnit), IUpdateMeal
{
    protected override string CommandName => "Update Meal";

    protected async override Task<IResult<Guid>> InternalExecuteAsync(UpdateMealCommand command, CancellationToken cancellationToken = default)
    {
        LogCommandStarted(Logger, CommandName, command.Id);

        Meal? meal = await mealRepository.GetWithTrackingAsync(command.Id, cancellationToken);

        if (meal is null)
        {
            return Result<Guid>.NotFound();
        }

        List<Guid> requestedFoodIds = command.Ingredients.Select(x => x.FoodId).ToList();

        Dictionary<Guid, int> foodIdMap = await foodRepository.GetIdMappingAsync(requestedFoodIds, cancellationToken);

        List<Guid> missingFoodIds = requestedFoodIds.Except(foodIdMap.Keys).ToList();

        if (missingFoodIds.Count > 0)
        {
            return Result<Guid>.DomainError($"The following foods are missing: {string.Join(", ", missingFoodIds)}.");
        }

        Dictionary<int, IngredientRequest> incomingIngredientsMap = command.Ingredients.ToDictionary(x => foodIdMap[x.FoodId]);

        foreach (Ingredient existingIngredient in meal.Ingredients)
        {
            if (incomingIngredientsMap.TryGetValue(existingIngredient.FoodId, out IngredientRequest? incomingIngredient))
            {
                existingIngredient.Quantity = incomingIngredient.Quantity;

                incomingIngredientsMap.Remove(existingIngredient.FoodId);
            }
            else
            {
                meal.Ingredients.Remove(existingIngredient);
            }
        }

        foreach (var newIngredientKvp in incomingIngredientsMap)
        {
            meal.Ingredients.Add(new Ingredient
            {
                FoodId = newIngredientKvp.Key,
                Quantity = newIngredientKvp.Value.Quantity
            });
        }

        meal.Name = command.Name;

        meal.NormalizedName = command.Name.ToNormalizedString();

        return Result.Success(command.Id);
    }

    public new Task<IResult<Guid>> ExecuteAsync(UpdateMealCommand command, CancellationToken cancellationToken = default)
        => base.ExecuteAsync(command, cancellationToken);

    [LoggerMessage(LogLevel.Information, "Command '{Command}' started for Meal Id '{MealId}'.")]
    private static partial void LogCommandStarted(ILogger logger, string command, Guid mealId);
}
