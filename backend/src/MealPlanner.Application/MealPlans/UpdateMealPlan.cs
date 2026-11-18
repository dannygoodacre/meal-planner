using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Commands;

public sealed record UpdateMealPlanCommand : IStateCommand, IPublicIdRequest, IMealPlanDetails
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required List<UpdateMealPlanMealRequest> Meals { get; init; }

    IReadOnlyCollection<IMealDetails> IMealPlanDetails.Meals => Meals;
}

internal sealed class UpdateMealPlan(IFoodRepository foodRepository, IMealPlanRepository mealPlanRepository) : ICommandHandler<UpdateMealPlanCommand, Guid>
{
    public async Task<IResult<Guid>> HandleAsync(UpdateMealPlanCommand command, CancellationToken ct)
    {
        MealPlan? mealPlan = await mealPlanRepository.GetWithTrackingAsync(command.Id, ct);

        if (mealPlan is null)
        {
            return new NotFound<Guid>();
        }

        if (await mealPlanRepository.ExistsByNameAsync(command.Name, command.Id, ct))
        {
            return new Conflict<Guid>("A meal plan with the same name already exists");
        }

        List<Guid> foodIds = command.Meals.SelectMany(x => x.Ingredients).Select(x => x.FoodId).Distinct().ToList();

        Dictionary<Guid, int> foodIdMap = await foodRepository.GetIdMappingAsync(foodIds, ct);

        List<Guid> missingFoodIds = foodIds.Except(foodIdMap.Keys).ToList();

        if (missingFoodIds.Count > 0)
        {
            return new DomainError<Guid>($"The following foods are missing: {string.Join(", ", missingFoodIds)}.");
        }

        Dictionary<Guid, MealPlanMeal> unmatchedMeals = mealPlan.Meals.ToDictionary(x => x.PublicId);

        List<Guid> missingMealIds = [];

        foreach (UpdateMealPlanMealRequest incoming in command.Meals)
        {
            IEnumerable<MealPlanIngredient> ingredients = incoming.Ingredients.Select(x => new MealPlanIngredient
            {
                FoodId = foodIdMap[x.FoodId],
                Quantity = x.Quantity
            });

            if (incoming.Id is not { } id || id == Guid.Empty)
            {
                mealPlan.Meals.Add(MealPlanMeal.Create(incoming.Name, ingredients));
            }
            else if (unmatchedMeals.Remove(id, out MealPlanMeal? existing))
            {
                existing.Name = incoming.Name;

                existing.SetIngredients(ingredients);
            }
            else
            {
                missingMealIds.Add(id);
            }
        }

        if (missingMealIds.Count > 0)
        {
            return new DomainError<Guid>($"The following meals were not found in the meal plan: {string.Join(", ", missingMealIds)}.");
        }

        foreach (MealPlanMeal removed in unmatchedMeals.Values)
        {
            mealPlan.Meals.Remove(removed);
        }

        mealPlan.Name = command.Name;

        return new Success<Guid>(mealPlan.PublicId);
    }
}
