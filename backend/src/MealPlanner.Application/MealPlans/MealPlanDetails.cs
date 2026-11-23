using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Meals;

namespace MealPlanner.Application.MealPlans;

public interface IMealPlanDetails
{
    string Name { get; }

    IReadOnlyCollection<IMealDetails> Meals { get; }
}

internal sealed class MealPlanDetailsValidator<T> : IValidator<T> where T : IRequest, IMealPlanDetails
{
    public void Validate(T request, ValidationState validationState)
    {
        validationState.IsNotNullEmptyOrWhitespace(request.Name, nameof(request.Name));

        if (validationState.AddErrorIf(request.Meals.Count == 0, nameof(request.Meals), new ValidationError("Must not be empty.", "NotEmpty")))
        {
            return;
        }

        var index = 0;

        foreach (IMealDetails meal in request.Meals)
        {
            MealDetailsRules.Validate(meal, validationState, $"{nameof(request.Meals)}[{index++}].");
        }
    }
}
