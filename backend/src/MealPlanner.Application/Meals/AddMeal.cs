using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Meals;

public sealed record AddMealCommand : IStateCommand, IMealDetails
{
    public required string Name { get; init; }

    public required List<IngredientRequest> Ingredients { get; init; }
}

internal sealed class AddMeal(IFoodRepository foodRepository, IMealRepository mealRepository) : ICommandHandler<AddMealCommand, Guid>
{
    public async Task<IResult<Guid>> HandleAsync(AddMealCommand command, CancellationToken ct)
    {
        if (await mealRepository.ExistsByNameAsync(command.Name, ct))
        {
            return new Conflict<Guid>("A meal with the same name already exists");
        }

        List<Guid> foodIds = [.. command.Ingredients.Select(x => x.FoodId)];

        Dictionary<Guid, int> foodIdMap = await foodRepository.GetIdMappingAsync(foodIds, ct);

        List<Guid> missingFoodIds = [.. foodIds.Except(foodIdMap.Keys)];

        if (missingFoodIds.Count > 0)
        {
            return new DomainError<Guid>($"The following foods are missing: {string.Join(", ", missingFoodIds)}.");
        }

        var meal = mealRepository.Add(new Meal
        {
            PublicId = Guid.NewGuid(),
            Name = command.Name,
            Ingredients =
            [
                .. command.Ingredients
                    .Select(x => new Ingredient { FoodId = foodIdMap[x.FoodId], Quantity = x.Quantity })
            ]
        });

        return new Success<Guid>(meal.PublicId);
    }
}
