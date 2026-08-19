using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public record AddFoodCommand : ICommand
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

public interface IAddFood
{
    Task<Result<Guid>> ExecuteAsync(AddFoodCommand command, CancellationToken cancellationToken = default);
}

internal sealed partial class AddFoodHandler(ILogger<AddFoodHandler> logger,
                                             IStateUnit stateUnit,
                                             IFoodRepository repository)
    : StateCommandHandler<AddFoodCommand, Guid>(logger, stateUnit), IAddFood
{
    protected override string CommandName => "Add Food";

    protected override void Validate(ValidationState state, AddFoodCommand command)
    {
        state.IsNotNullEmptyOrWhitespace(command.Name, nameof(command.Name));

        state.IsValidEnum(command.Unit, nameof(command.Unit));

        state.IsPositive(command.ReferenceQuantity, nameof(command.ReferenceQuantity));

        state.IsNonNegative(command.Calories, nameof(command.Calories));

        state.IsNonNegative(command.Protein, nameof(command.Protein));

        state.IsNonNegative(command.Carbohydrates, nameof(command.Carbohydrates));

        state.IsNonNegative(command.Sugar, nameof(command.Sugar));

        state.IsLessThanOrEqualTo(command.Sugar, nameof(command.Sugar), command.Carbohydrates, nameof(command.Carbohydrates));

        state.IsNonNegative(command.Fat, nameof(command.Fat));

        state.IsNonNegative(command.SaturatedFat, nameof(command.SaturatedFat));

        state.IsLessThanOrEqualTo(command.SaturatedFat, nameof(command.SaturatedFat), command.Fat, nameof(command.Fat));

        state.IsNonNegative(command.Fibre, nameof(command.Fibre));

        state.IsNonNegative(command.Salt, nameof(command.Salt));

        state.IsValidUrl(command.Source, nameof(command.Source));
    }

    protected async override Task<Result<Guid>> InternalExecuteAsync(AddFoodCommand command, CancellationToken cancellationToken = default)
    {
        LogCommandStarted(Logger, CommandName, command.Name);

        if (await repository.ExistsByNameAsync(command.Name, cancellationToken))
        {
            return Result<Guid>.Conflict("A food with the same name already exists.");
        }

        var food = repository.Add(new Food
        {
            PublicId = Guid.NewGuid(),
            Name = command.Name,
            NormalizedName = command.Name.ToNormalizedString(),
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

        return Result.Success(food.PublicId);
    }

    public new Task<Result<Guid>> ExecuteAsync(AddFoodCommand command, CancellationToken cancellationToken = default)
        => base.ExecuteAsync(command, cancellationToken);

    [LoggerMessage(LogLevel.Information, "Command '{Command}' started for Name '{Name}'.")]
    private static partial void LogCommandStarted(ILogger logger, string command, string name);
}
