using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public sealed record AddMealPlanCommand : ICommand
{
    public required string Name { get; init; }

    public required List<AddMealPlanMealRequest> Meals { get; init; }
}

public interface IAddMealPlan
{
    Task<Result<Guid>> ExecuteAsync(AddMealPlanCommand command, CancellationToken cancellationToken = default);
}

internal sealed partial class AddMealPlanHandler(ILogger<AddMealPlanHandler> logger,
                                                 IStateUnit stateUnit,
                                                 IFoodRepository foodRepository,
                                                 IMealRepository mealRepository,
                                                 IMealPlanRepository mealPlanRepository)
    : StateCommandHandler<AddMealPlanCommand, Guid>(logger, stateUnit), IAddMealPlan
{
    protected override string CommandName => "Add Meal Plan";

    protected override void Validate(ValidationState state, AddMealPlanCommand command)
    {
        state.IsNotNullEmptyOrWhitespace(command.Name, nameof(command.Name));

        state.IsNotEmpty(command.Meals, nameof(command.Meals));

        command.Meals.ForEach(x =>
        {
            if (x.MealId is not null)
            {
                state.IsNull(x.Name, nameof(x.Name));

                state.IsEmpty(x.Ingredients, nameof(x.Ingredients));
            }
            else if (state.IsNotEmpty(x.Ingredients, nameof(x.Ingredients)))
            {
                state.IsNotNullEmptyOrWhitespace(x.Name, nameof(x.Name));

                x.Ingredients.ForEach(y =>
                {
                    state.IsNonEmptyGuid(y.FoodId, nameof(y.FoodId));

                    state.IsPositive(y.Quantity, nameof(y.Quantity));
                });
            }
        });
    }

    protected async override Task<Result<Guid>> InternalExecuteAsync(AddMealPlanCommand command, CancellationToken cancellationToken = default)
    {
        LogCommandStarted(logger, CommandName, command.Name);

        if (await mealPlanRepository.ExistsByNameAsync(command.Name, cancellationToken))
        {
            return Result<Guid>.Conflict("A meal plan with the same name already exists");
        }

        IEnumerable<Guid> incomingFoodIds = command.Meals
            .SelectMany(x => x.Ingredients)
            .Select(x => x.FoodId)
            .Distinct()
            .ToList();

        Dictionary<Guid, int> foodIdMap = await foodRepository.GetIdMappingAsync(incomingFoodIds, cancellationToken);

        List<Guid> missingFoodIds = foodIdMap.Keys.Except(incomingFoodIds).ToList();

        if (missingFoodIds.Count > 0)
        {
            return Result<Guid>.DomainError($"The following foods are missing: {string.Join(", ", missingFoodIds)}.");
        }

        List<Guid> missingTemplateMeals = [];

        List<MealPlanMeal> mealPlanMeals = [];

        foreach (var mealDto in command.Meals)
        {
            if (mealDto.MealId is not null)
            {
                Guid mealId = mealDto.MealId.Value;

                Meal? meal = await mealRepository.GetAsync(mealId, cancellationToken);

                if (meal is not null)
                {
                    mealPlanMeals.Add(meal.ToMealPlanMeal());
                }
                else
                {
                    missingTemplateMeals.Add(mealId);
                }
            }
            else
            {
                mealPlanMeals.Add(new MealPlanMeal
                {
                    PublicId = Guid.NewGuid(),
                    Name = mealDto.Name!,
                    NormalizedName = mealDto.Name!.ToNormalizedString(),
                    Ingredients = mealDto.Ingredients.Select(x => new MealPlanIngredient
                        {
                            FoodId = foodIdMap[x.FoodId],
                            Quantity = x.Quantity,
                        })
                        .ToList(),
                });
            }
        }

        if (missingTemplateMeals.Count > 0)
        {
            return Result<Guid>.DomainError($"The following template meals are missing: {string.Join(", ", missingTemplateMeals)}.");
        }

        var mealPlan = mealPlanRepository.Add(new MealPlan
        {
            PublicId = Guid.NewGuid(),
            Name = command.Name,
            NormalizedName = command.Name.ToNormalizedString(),
            Meals = mealPlanMeals
        });

        return Result.Success(mealPlan.PublicId);
    }

    public new Task<Result<Guid>> ExecuteAsync(AddMealPlanCommand command, CancellationToken cancellationToken = default)
        => base.ExecuteAsync(command, cancellationToken);

    [LoggerMessage(LogLevel.Information, "Command '{Command}' started for Name '{Name}'.")]
    private static partial void LogCommandStarted(ILogger logger, string command, string name);
}
