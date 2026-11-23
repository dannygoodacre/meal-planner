using System.Globalization;
using DannyGoodacre.Results;

namespace MealPlanner.Application;

internal static class ValidationStateExtensions
{
    extension(ValidationState validationState)
    {
        public bool IsLessThanOrEqualTo(decimal value, string name, decimal upperLimit, string? upperLimitName = null)
        {
            if (value <= upperLimit)
            {
                return true;
            }

            string limit = upperLimit.ToString(CultureInfo.InvariantCulture);

            validationState.AddError(name, new ValidationError(
                $"Must be less than or equal to {upperLimitName ?? limit}.",
                "LessThanOrEqualTo",
                new Dictionary<string, string> { ["upperLimit"] = limit }));

            return false;
        }

        public bool IsMinimumLength(string value, int minimumLength, string name)
        {
            if (!validationState.IsNotNullEmptyOrWhitespace(value, name))
            {
                return false;
            }

            if (value.Length >= minimumLength)
            {
                return true;
            }

            validationState.AddError(name, new ValidationError(
                $"Must be at least {minimumLength} characters long.",
                "MinimumLength",
                new Dictionary<string, string> { ["minimumLength"] = minimumLength.ToString(CultureInfo.InvariantCulture) }));

            return false;
        }

        public bool IsNotEmpty<T>(List<T> value, string name)
        {
            if (value.Count > 0)
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must not be empty.", "NotEmpty"));

            return false;
        }

        public bool IsEmpty<T>(List<T> value, string name)
        {
            if (value.Count == 0)
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must be empty.", "Empty"));

            return false;
        }

        public bool IsNonNegative(decimal value, string name)
        {
            if (value >= 0)
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must be greater than or equal to 0.", "NonNegative"));

            return false;
        }

        public bool IsNotNullEmptyOrWhitespace(string? value, string name)
            => !validationState.AddErrorIf(
                string.IsNullOrWhiteSpace(value),
                name,
                new ValidationError("Must not be null or empty.", "Required"));

        public bool IsNull(object? value, string name)
        {
            if (value is null)
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must be null.", "Null"));

            return false;
        }

        public bool IsNotNull(object? value, string name)
        {
            if (value is not null)
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must not be null.", "NotNull"));

            return false;
        }

        public bool IsPositive(decimal value, string name)
        {
            if (value > 0)
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must be greater than 0.", "Positive"));

            return false;
        }

        public bool IsNonEmptyGuid(Guid value, string name)
        {
            if (value != Guid.Empty)
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must not be empty.", "NotEmpty"));

            return false;
        }

        public bool IsValidEnum<T>(T value, string name) where T : struct, Enum
        {
            if (Enum.IsDefined(value))
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must be defined.", "InvalidEnum"));

            return false;
        }

        public bool IsValidUrl(string value, string name)
        {
            if (!validationState.IsNotNullEmptyOrWhitespace(value, name))
            {
                return false;
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out Uri? result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
            {
                return true;
            }

            validationState.AddError(name, new ValidationError("Must be a valid URL.", "InvalidUrl"));

            return false;
        }
    }
}
