using System.Globalization;
using System.Text;
namespace BookApi.Extensions
{
    public static class StringExtensions
    {
        public static string NormalizeString(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            string normalized = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();
              
        }
    }
}
