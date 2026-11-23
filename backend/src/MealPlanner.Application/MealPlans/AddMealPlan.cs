using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Meals;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.MealPlans;

public sealed record AddMealPlanCommand : IStateCommand, IMealPlanDetails
{
    public required string Name { get; init; }

    public required List<AddMealPlanMealRequest> Meals { get; init; }

    IReadOnlyCollection<IMealDetails> IMealPlanDetails.Meals => Meals;
}

internal sealed class AddMealPlan(IFoodRepository foodRepository, IMealPlanRepository mealPlanRepository) : ICommandHandler<AddMealPlanCommand, Guid>
{
    public async Task<IResult<Guid>> HandleAsync(AddMealPlanCommand command, CancellationToken ct)
    {
        if (await mealPlanRepository.ExistsByNameAsync(command.Name, ct))
        {
            return new Conflict<Guid>("A meal plan with the same name already exists");
        }

        List<Guid> foodIds = [.. command.Meals.SelectMany(x => x.Ingredients).Select(x => x.FoodId).Distinct()];

        Dictionary<Guid, int> foodIdMap = await foodRepository.GetIdMappingAsync(foodIds, ct);

        List<Guid> missingFoodIds = [.. foodIds.Except(foodIdMap.Keys)];

        if (missingFoodIds.Count > 0)
        {
            return new DomainError<Guid>($"The following foods are missing: {string.Join(", ", missingFoodIds)}.");
        }

        var mealPlan = mealPlanRepository.Add(new MealPlan
        {
            PublicId = Guid.NewGuid(),
            Name = command.Name,
            Meals =
            [
                .. command.Meals
                    .Select(x => MealPlanMeal.Create(x.Name, x.Ingredients.Select(y => new MealPlanIngredient
                    {
                        FoodId = foodIdMap[y.FoodId],
                        Quantity = y.Quantity
                    })))
            ]
        });

        return new Success<Guid>(mealPlan.PublicId);
    }
}
