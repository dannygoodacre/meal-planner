using DannyGoodacre.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using IResult = DannyGoodacre.Results.IResult;

namespace MealPlanner.Web;

internal static class ResultExtensions
{
    public static IActionResult ToHttpResponse(this IResult result)
        => result switch
        {
            Success => new NoContentResult(),

            Invalid invalid => new BadRequestObjectResult(invalid.ValidationState.ToValidationProblemDetails()),

            Conflict conflict => Problem(StatusCodes.Status409Conflict, conflict.Message),

            DomainError domainError => Problem(StatusCodes.Status422UnprocessableEntity, domainError.Message),

            NotFound => Problem(StatusCodes.Status404NotFound),

            _ => Problem(StatusCodes.Status500InternalServerError)
        };

    public static IActionResult ToHttpResponse<T>(this IResult<T> result)
        => result switch
        {
            Success<T> success => new OkObjectResult(success.Value),

            Invalid invalid => new BadRequestObjectResult(invalid.ValidationState!.ToValidationProblemDetails()),

            Conflict conflict => Problem(StatusCodes.Status409Conflict, conflict.Message),

            DomainError domainError => Problem(StatusCodes.Status422UnprocessableEntity, domainError.Message),

            NotFound => Problem(StatusCodes.Status404NotFound),

            _ => Problem(StatusCodes.Status500InternalServerError)
        };

    private static ObjectResult Problem(int status, string? detail = null)
        => new(new ProblemDetails { Status = status, Title = ReasonPhrases.GetReasonPhrase(status), Detail = detail })
        {
            StatusCode = status
        };
}
