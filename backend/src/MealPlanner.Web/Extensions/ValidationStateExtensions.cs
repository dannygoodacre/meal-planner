using DannyGoodacre.Results;
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

            foreach ((string key, IReadOnlyList<ValidationError> errors) in state.Errors)
            {
                foreach (ValidationError error in errors)
                {
                    modelState.AddModelError(key, error.Message);
                }
            }

            return new ValidationProblemDetails(modelState);
        }
    }
}
