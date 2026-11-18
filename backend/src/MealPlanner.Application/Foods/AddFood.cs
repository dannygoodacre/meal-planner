using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;

namespace MealPlanner.Application.Commands;

public sealed record AddFoodCommand : IStateCommand, IFoodDetails
{
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

internal sealed class AddFood(IFoodRepository foodRepository) : ICommandHandler<AddFoodCommand, Guid>
{
    public async Task<IResult<Guid>> HandleAsync(AddFoodCommand command, CancellationToken ct)
    {
        if (await foodRepository.ExistsByNameAsync(command.Name, ct))
        {
            return new Conflict<Guid>("A food with the same name already exists");
        }

        var food = foodRepository.Add(new Food
        {
            PublicId = Guid.NewGuid(),
            Name = command.Name,
            Unit = command.Unit,
            ReferenceQuantity = command.ReferenceQuantity,
            Calories = command.Calories,
            Protein = command.Protein,
            Carbohydrates = command.Carbohydrates,
            Sugar = command.Sugar,
            Fat = command.Fat,
            SaturatedFat = command.SaturatedFat,
            Fibre = command.Fibre,
            Salt = command.Salt,
            Source = command.Source
        });

        return new Success<Guid>(food.PublicId);
    }
}
