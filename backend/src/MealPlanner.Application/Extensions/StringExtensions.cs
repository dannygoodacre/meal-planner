using System.Globalization;
using System.Text;

namespace MealPlanner.Application;

public static class StringExtensions
{
    extension(string value)
    {
        public string ToNormalizedString()
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);

            var normalizedCharacters = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);

            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedCharacters)
            {
                stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
