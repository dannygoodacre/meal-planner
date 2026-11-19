using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;

namespace MealPlanner.Application.Commands;

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

        if (validationState.AddErrorIf(request.Meals.Count == 0, nameof(request.Meals), "Must not be empty."))
        {
            return;
        }

        int index = 0;

        foreach (IMealDetails meal in request.Meals)
        {
            MealDetailsRules.Validate(meal, validationState, $"{nameof(request.Meals)}[{index++}].");
        }
    }
}
