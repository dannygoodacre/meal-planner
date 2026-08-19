using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public record AddMealCommand : ICommand
{
    public required string Name { get; init; }

    public required List<IngredientRequest> Ingredients { get; init; }
}

public interface IAddMeal
{
    Task<IResult<Guid>> ExecuteAsync(AddMealCommand command, CancellationToken cancellationToken = default);
}

internal sealed partial class AddMealHandler(ILogger<AddMealHandler> logger,
                                             IStateUnit stateUnit,
                                             IFoodRepository foodRepository,
                                             IMealRepository mealRepository)
    : StateCommandHandler<AddMealCommand, Guid>(logger, stateUnit), IAddMeal
{
    protected override string CommandName => "Add Meal";

    protected override void Validate(ValidationState state, AddMealCommand command)
    {
        state.IsNotNullEmptyOrWhitespace(command.Name, nameof(command.Name));

        state.IsNotEmpty(command.Ingredients, nameof(command.Ingredients));

        command.Ingredients.ForEach(x =>
        {
            state.IsNonEmptyGuid(x.FoodId, nameof(command.Ingredients));

            state.IsPositive(x.Quantity, nameof(command.Ingredients));
        });
    }

    protected async override Task<IResult<Guid>> InternalExecuteAsync(AddMealCommand command, CancellationToken cancellationToken = default)
    {
        LogCommandStarted(Logger, CommandName, command.Name);

        if (await mealRepository.ExistsByNameAsync(command.Name, cancellationToken))
        {
            return Result<Guid>.Conflict("A meal with the same name already exists.");
        }

        List<Guid> requestedFoodIds = command.Ingredients.Select(x => x.FoodId).ToList();

        Dictionary<Guid, int> foodIdMap = await foodRepository.GetIdMappingAsync(requestedFoodIds, cancellationToken);

        List<Ingredient> ingredients = [];

        List<Guid> missingFoodIds = [];

        foreach (var ingredient in command.Ingredients)
        {
            if (foodIdMap.TryGetValue(ingredient.FoodId, out int internalId))
            {
                ingredients.Add(new Ingredient
                {
                    FoodId = internalId,
                    Quantity = ingredient.Quantity
                });
            }
            else
            {
                missingFoodIds.Add(ingredient.FoodId);
            }
        }

        if (missingFoodIds.Count > 0)
        {
            return Result<Guid>.DomainError($"The following foods are missing: {string.Join(", ", missingFoodIds)}.");
        }

        var meal = mealRepository.Add(new Meal
        {
            PublicId = Guid.NewGuid(),
            Name = command.Name,
            NormalizedName = command.Name.ToNormalizedString(),
            Ingredients = ingredients
        });

        return Result.Success(meal.PublicId);
    }

    public new Task<IResult<Guid>> ExecuteAsync(AddMealCommand command, CancellationToken cancellationToken = default)
        => base.ExecuteAsync(command, cancellationToken);

    [LoggerMessage(LogLevel.Information, "Command '{Command}' started for Name '{Name}'.")]
    private static partial void LogCommandStarted(ILogger logger, string command, string name);
}
