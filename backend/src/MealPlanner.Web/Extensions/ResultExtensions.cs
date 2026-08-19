using DannyGoodacre.Primitives;
using Microsoft.AspNetCore.Mvc;
using IResult = DannyGoodacre.Primitives.IResult;

namespace MealPlanner.Web;

internal static class ResultExtensions
{
    public static IActionResult ToHttpResponse(this IResult result)
        => result switch
        {
            Success => new NoContentResult(),

            Invalid invalid => new BadRequestObjectResult(invalid.ValidationState.ToValidationProblemDetails()),

            Conflict conflict => new ConflictObjectResult(conflict.Message),

            DomainError domainError => new UnprocessableEntityObjectResult(domainError.Message),

            NotFound => new NotFoundResult(),

            _ => new StatusCodeResult(500)
        };

    public static IActionResult ToHttpResponse<T>(this IResult<T> result)
        => result switch
        {
            Success<T> success => new OkObjectResult(success.Value),

            Invalid invalid => new BadRequestObjectResult(invalid.ValidationState!.ToValidationProblemDetails()),

            Conflict conflict => new ConflictObjectResult(conflict.Message),

            DomainError domainError => new UnprocessableEntityObjectResult(domainError.Message),

            NotFound => new NotFoundResult(),

            _ => new StatusCodeResult(500)
        };
}
