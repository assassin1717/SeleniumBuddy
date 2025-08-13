using OpenQA.Selenium;

namespace SeleniumBuddy.Extensions
{

    public static class ByExtensions
    {
        /// <summary>
        /// Best-effort human friendly description for logging (e.g., "By.Id('loginBtn')").
        /// </summary>
        public static string Describe(this By by)
        {
            if (by is null) throw new ArgumentNullException(nameof(by));

            var s = by.ToString(); // Usually "By.Id: loginBtn" or "By.XPath: //div[...]"
            if (string.IsNullOrEmpty(s)) return "By(unknown)";

            // Normalize common "By.X: value" to "By.X('value')"
            var idx = s.IndexOf(": ", StringComparison.Ordinal);
            if (idx > 0 && idx + 2 < s.Length)
            {
                var kind = s.Substring(0, idx).Trim();
                var val = s.Substring(idx + 2).Trim();
                return $"{kind}('{val}')";
            }
            return s;
        }
    }
}
