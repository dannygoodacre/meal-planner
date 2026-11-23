using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Enums;

namespace MealPlanner.Application.Foods;

public interface IFoodDetails
{
    string Name { get; }

    Unit Unit { get; }

    int ReferenceQuantity { get; }

    int Calories { get; }

    decimal Protein { get; }

    decimal Carbohydrates { get; }

    decimal Sugar { get; }

    decimal Fat { get; }

    decimal SaturatedFat { get; }

    decimal Fibre { get; }

    decimal Salt { get; }

    string Source { get; }
}

internal sealed class FoodDetailsValidator<T> : IValidator<T> where T : IRequest, IFoodDetails
{
    public void Validate(T request, ValidationState validationState)
    {
        validationState.IsNotNullEmptyOrWhitespace(request.Name, nameof(request.Name));

        validationState.IsValidEnum(request.Unit, nameof(request.Unit));

        validationState.IsPositive(request.ReferenceQuantity, nameof(request.ReferenceQuantity));

        validationState.IsNonNegative(request.Calories, nameof(request.Calories));

        validationState.IsNonNegative(request.Protein, nameof(request.Protein));

        validationState.IsNonNegative(request.Carbohydrates, nameof(request.Carbohydrates));

        validationState.IsNonNegative(request.Sugar, nameof(request.Sugar));

        validationState.IsLessThanOrEqualTo(request.Sugar, nameof(request.Sugar), request.Carbohydrates, nameof(request.Carbohydrates));

        validationState.IsNonNegative(request.Fat, nameof(request.Fat));

        validationState.IsNonNegative(request.SaturatedFat, nameof(request.SaturatedFat));

        validationState.IsLessThanOrEqualTo(request.SaturatedFat, nameof(request.SaturatedFat), request.Fat, nameof(request.Fat));

        validationState.IsNonNegative(request.Fibre, nameof(request.Fibre));

        validationState.IsNonNegative(request.Salt, nameof(request.Salt));

        validationState.IsValidUrl(request.Source, nameof(request.Source));
    }
}
