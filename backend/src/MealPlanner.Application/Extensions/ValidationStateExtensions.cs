using System.Globalization;
using DannyGoodacre.Primitives;

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

            validationState.AddError(name, $"Must be less than or equal to {upperLimitName ?? upperLimit.ToString(CultureInfo.InvariantCulture)}.");

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

            validationState.AddError(name, $"Must be at least {minimumLength} characters long.");

            return false;
        }

        public bool IsNotEmpty<T>(List<T> value, string name)
        {
            if (value.Count > 0)
            {
                return true;
            }

            validationState.AddError(name, "Must not be empty.");

            return false;
        }

        public bool IsEmpty<T>(List<T> value, string name)
        {
            if (value.Count == 0)
            {
                return true;
            }

            validationState.AddError(name, "Must be empty.");

            return false;
        }

        public bool IsNonNegative(decimal value, string name)
        {
            if (value >= 0)
            {
                return true;
            }

            validationState.AddError(name, "Must be greater than or equal to 0.");

            return false;
        }

        public bool IsNotNullEmptyOrWhitespace(string? value, string name)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            validationState.AddError(name, "Must not be null, empty, or whitespace.");

            return false;
        }

        public bool IsNull(object? value, string name)
        {
            if (value is null)
            {
                return true;
            }

            validationState.AddError(name, "Must be null.");

            return false;
        }

        public bool IsNotNull(object? value, string name)
        {
            if (value is not null)
            {
                return true;
            }

            validationState.AddError(name, "Must not be null.");

            return false;
        }

        public bool IsPositive(decimal value, string name)
        {
            if (value > 0)
            {
                return true;
            }

            validationState.AddError(name, "Must be greater than 0.");

            return false;
        }

        public bool IsNonEmptyGuid(Guid value, string name)
        {
            if (value != Guid.Empty)
            {
                return true;
            }

            validationState.AddError(name, "Must not be empty.");

            return false;
        }

        public bool IsValidEnum<T>(T value, string name) where T : struct, Enum
        {
            if (Enum.IsDefined(value))
            {
                return true;
            }

            validationState.AddError(name, "Must be defined.");

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

            validationState.AddError(name, "Must be a valid URL.");

            return false;
        }
    }
}
