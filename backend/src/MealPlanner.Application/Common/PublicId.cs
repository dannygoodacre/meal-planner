using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;

namespace MealPlanner.Application.Commands;

public interface IPublicIdRequest
{
    Guid Id { get; }
}

internal sealed class PublicIdValidator<T> : IValidator<T> where T : IRequest, IPublicIdRequest
{
    public void Validate(T request, ValidationState validationState)
        => validationState.IsNonEmptyGuid(request.Id, nameof(request.Id));
}
