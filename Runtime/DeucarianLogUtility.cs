using System.Text.RegularExpressions;

namespace Deucarian.Logging
{
    /// <summary>
    /// Utility helpers for Deucarian logging.
    /// </summary>
    public static class DeucarianLogUtility
    {
        private const string RedactedValue = "[REDACTED]";
        private const RegexOptions Options = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

        private static readonly Regex UrlQueryRegex = new Regex(
            @"\b(https?://[^\s?]+)\?[^\s]+",
            Options);

        private static readonly Regex JsonStringSecretRegex = new Regex(
            @"([""']?)(token|access_token|password|secret|api_key|authorization)([""']?\s*:\s*[""'])([^""'\r\n]*)([""'])",
            Options);

        private static readonly Regex KeyValueSecretRegex = new Regex(
            @"\b(token|access_token|password|secret|api_key|authorization)(\s*=\s*)([^&\s,;]+)",
            Options);

        private static readonly Regex HeaderSecretRegex = new Regex(
            @"\b(authorization|password|secret|api_key|access_token|token)(\s*:\s*)([^\r\n,;]+)",
            Options);

        /// <summary>
        /// Redacts obvious sensitive values from a log message on a best-effort basis.
        /// </summary>
        /// <param name="input">Input text that may contain sensitive data.</param>
        /// <returns>Text with obvious secrets redacted, the original value when no changes are needed, or an empty string for null input.</returns>
        /// <remarks>
        /// This helper is intentionally simple and is not a security boundary. Avoid logging secrets in the first place.
        /// </remarks>
        public static string RedactSensitiveData(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input ?? string.Empty;
            }

            string redacted = UrlQueryRegex.Replace(input, "$1?[REDACTED_QUERY]");
            redacted = JsonStringSecretRegex.Replace(redacted, "$1$2$3" + RedactedValue + "$5");
            redacted = KeyValueSecretRegex.Replace(redacted, "$1$2" + RedactedValue);
            redacted = HeaderSecretRegex.Replace(redacted, "$1$2" + RedactedValue);
            return redacted;
        }
    }
}
