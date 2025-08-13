using OpenQA.Selenium;
using SeleniumBuddy.Abstractions;

namespace SeleniumBuddy.Extensions
{
    public static class IWebElementExtensions
    {
        /// <summary>
        /// True if element is displayed and enabled (basic "clickable" heuristic).
        /// </summary>
        public static bool IsClickable(this IWebElement element)
        {
            if (element is null) throw new ArgumentNullException(nameof(element));
            try
            {
                return element.Displayed && element.Enabled;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Scrolls this element into the viewport using JavaScript.
        /// </summary>
        public static void ScrollIntoView(this IWebElement element, IScriptExecutor js, bool alignToTop = true, CancellationToken ct = default)
        {
            if (element is null) throw new ArgumentNullException(nameof(element));
            if (js is null) throw new ArgumentNullException(nameof(js));
            js.Execute("arguments[0].scrollIntoView(arguments[1]);", ct, element, alignToTop);
        }

        /// <summary>
        /// Clears the element and types the given text. Uses a JS fallback if .Clear() fails.
        /// </summary>
        public static void SetText(this IWebElement element, string text, IScriptExecutor js = null, bool clearBefore = true, CancellationToken ct = default)
        {
            if (element is null) throw new ArgumentNullException(nameof(element));
            if (clearBefore)
            {
                try { element.Clear(); }
                catch
                {
                    js?.Execute("arguments[0].value = '';", ct, element);
                }
            }
            element.SendKeys(text ?? string.Empty);
        }
    }
}
