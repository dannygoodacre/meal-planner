using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Commands;

public sealed record UpdateMealCommand : IStateCommand, IPublicIdRequest, IMealDetails
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required List<IngredientRequest> Ingredients { get; init; }
}

internal sealed class UpdateMeal(IFoodRepository foodRepository, IMealRepository mealRepository) : ICommandHandler<UpdateMealCommand, Guid>
{
    public async Task<IResult<Guid>> HandleAsync(UpdateMealCommand command, CancellationToken ct)
    {
        Meal? meal = await mealRepository.GetWithTrackingAsync(command.Id, ct);

        if (meal is null)
        {
            return new NotFound<Guid>();
        }

        if (await mealRepository.ExistsByNameAsync(command.Name, command.Id, ct))
        {
            return new Conflict<Guid>("A meal with the same name already exists");
        }

        List<Guid> foodIds = command.Ingredients.Select(x => x.FoodId).ToList();

        Dictionary<Guid, int> foodIdMap = await foodRepository.GetIdMappingAsync(foodIds, ct);

        List<Guid> missingFoodIds = foodIds.Except(foodIdMap.Keys).ToList();

        if (missingFoodIds.Count > 0)
        {
            return new DomainError<Guid>($"The following foods are missing: {string.Join(", ", missingFoodIds)}.");
        }

        Dictionary<int, int> incoming = command.Ingredients.ToDictionary(x => foodIdMap[x.FoodId], x => x.Quantity);

        // ToList: removing from the collection being enumerated throws.
        foreach (Ingredient existing in meal.Ingredients.ToList())
        {
            if (incoming.Remove(existing.FoodId, out int quantity))
            {
                existing.Quantity = quantity;
            }
            else
            {
                meal.Ingredients.Remove(existing);
            }
        }

        foreach ((int foodId, int quantity) in incoming)
        {
            meal.Ingredients.Add(new Ingredient { FoodId = foodId, Quantity = quantity });
        }

        meal.Name = command.Name;

        return new Success<Guid>(meal.PublicId);
    }
}
