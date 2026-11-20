using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Elogbook.Core.Helpers
{
    /// <summary>
    /// Helper class for string manipulation operations
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Converts a Vietnamese string to snake_case slug format.
        /// Removes Vietnamese diacritics and replaces spaces and special characters with underscores.
        /// Example: "Tr??ng Ca" ? "truong_ca"
        /// </summary>
        /// <param name="input">Input string to convert</param>
        /// <returns>Snake case slug string, or empty string if input is null or whitespace</returns>
        public static string ToSnakeCaseSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) 
                return string.Empty;

            // 1. Manually convert Đ/đ to D/d (since FormD doesn't auto-separate Đ)
            string text = input.Replace("Đ", "D").Replace("đ", "d");

            // 2. Remove Vietnamese diacritics using NormalizationForm.FormD
            string normalized = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            string unaccented = sb.ToString().Normalize(NormalizationForm.FormC);

            // 3. Convert to lowercase
            string lower = unaccented.ToLowerInvariant();

            // 4. Replace spaces and special characters with underscores
            string result = Regex.Replace(lower, @"[^a-z0-9]+", "_");

            // 5. Trim excess underscores from start/end
            return result.Trim('_');
        }
    }
}
