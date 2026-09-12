using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public sealed record UpdateFoodCommand : AddFoodCommand
{
    public required Guid Id { get; init; }
}

public interface IUpdateFood
{
    Task<IResult<Guid>> ExecuteAsync(UpdateFoodCommand command, CancellationToken cancellationToken = default);
}

internal sealed partial class UpdateFoodHandler(ILogger<UpdateFoodHandler> logger,
                                                IStateUnit stateUnit,
                                                IFoodRepository repository)
    : StateCommandHandler<UpdateFoodCommand, Guid>(logger, stateUnit), IUpdateFood
{
    protected override string CommandName => "Update Food";

    protected override void Validate(ValidationState state, UpdateFoodCommand command)
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

    protected async override Task<IResult<Guid>> InternalExecuteAsync(UpdateFoodCommand command, CancellationToken cancellationToken = new CancellationToken())
    {
        LogCommandStarted(Logger, CommandName, command.Id);

        Food? food = await repository.GetWithTrackingAsync(command.Id, cancellationToken);

        if (food is null)
        {
            return Result<Guid>.NotFound();
        }

        food.Name = command.Name;
        food.NormalizedName = command.Name.ToNormalizedString();
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

        return Result.Success(command.Id);
    }

    public new Task<IResult<Guid>> ExecuteAsync(UpdateFoodCommand command, CancellationToken cancellationToken = default)
        => base.ExecuteAsync(command, cancellationToken);

    [LoggerMessage(LogLevel.Information, "Command '{Command}' started for Food Id '{FoodId}'.")]
    private static partial void LogCommandStarted(ILogger logger, string command, Guid foodId);
}
