using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;

namespace MealPlanner.Application.Foods;

public sealed record UpdateFoodCommand : IStateCommand, IPublicIdRequest, IFoodDetails
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required Unit Unit { get; init; }

    public required int ReferenceQuantity { get; init; }

    public required int Calories { get; init; }

    public required decimal Protein { get; init; }

    public required decimal Carbohydrates { get; init; }

    public required decimal Sugar { get; init; }

    public required decimal Fat { get; init; }

    public required decimal SaturatedFat { get; init; }

    public required decimal Fibre { get; init; }

    public required decimal Salt { get; init; }

    public required string Source { get; init; }
}

internal sealed class UpdateFood(IFoodRepository foodRepository) : ICommandHandler<UpdateFoodCommand>
{
    public async Task<IResult> HandleAsync(UpdateFoodCommand command, CancellationToken ct)
    {
        Food? food = await foodRepository.GetWithTrackingAsync(command.Id, ct);

        if (food is null)
        {
            return new NotFound();
        }

        if (await foodRepository.ExistsByNameAsync(command.Name, command.Id, ct))
        {
            return new Conflict("A food with the same name already exists");
        }

        food.Name = command.Name;
        food.Unit = command.Unit;
        food.ReferenceQuantity = command.ReferenceQuantity;
        food.Calories = command.Calories;
        food.Protein = command.Protein;
        food.Carbohydrates = command.Carbohydrates;
        food.Sugar = command.Sugar;
        food.Fat = command.Fat;
        food.SaturatedFat = command.SaturatedFat;
        food.Fibre = command.Fibre;
        food.Salt = command.Salt;
        food.Source = command.Source;

        return new Success();
    }
}
