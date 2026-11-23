using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;

namespace MealPlanner.Application.Queries;

public interface ISearchTermRequest
{
    string SearchTerm { get; }
}

internal sealed class SearchTermValidator<T> : IValidator<T> where T : IRequest, ISearchTermRequest
{
    public void Validate(T request, ValidationState validationState)
        => validationState.IsMinimumLength(request.SearchTerm, 3, nameof(request.SearchTerm));
}
