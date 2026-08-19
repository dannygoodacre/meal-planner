using DannyGoodacre.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MealPlanner.Web;

internal static class ValidationStateExtensions
{
    extension(ValidationState state)
    {
        public ValidationProblemDetails ToValidationProblemDetails()
        {
            ModelStateDictionary modelState = new();

            foreach ((string key, IReadOnlyList<string> errors) in state.Errors)
            {
                foreach (string error in errors)
                {
                    modelState.AddModelError(key, error);
                }
            }

            return new ValidationProblemDetails(modelState);
        }
    }
}
