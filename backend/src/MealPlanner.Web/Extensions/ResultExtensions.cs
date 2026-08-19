using DannyGoodacre.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web;

internal static class ResultExtensions
{
    public static IActionResult ToHttpResponse(this Result result)
        => result.Status switch
        {
            Status.Success => new NoContentResult(),

            Status.Invalid => new BadRequestObjectResult(result.ValidationState!.ToValidationProblemDetails()),

            Status.Conflict => new ConflictObjectResult(result.Error),

            Status.DomainError => new UnprocessableEntityObjectResult(result.Error),

            Status.NotFound => new NotFoundResult(),

            _ => new StatusCodeResult(500)
        };

    public static IActionResult ToHttpResponse<T>(this Result<T> result)
        => result.Status switch
        {
            Status.Success => new OkObjectResult(result.Value),

            Status.Invalid => new BadRequestObjectResult(result.ValidationState!.ToValidationProblemDetails()),

            Status.Conflict => new ConflictObjectResult(result.Error),

            Status.DomainError => new UnprocessableEntityObjectResult(result.Error),

            Status.NotFound => new NotFoundResult(),

            _ => new StatusCodeResult(500)
        };
}
