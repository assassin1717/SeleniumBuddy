using OpenQA.Selenium;

namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// High-level, resilient interactions that combine waits + retries.
    /// </summary>
    public interface IInteractions
    {
        /// <summary>Scrolls the element into view and clicks it once visible/clickable.</summary>
        void ClickWhenVisible(By by, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>Clears (optional) and types text once the element is ready for input.</summary>
        void TypeWhenReady(By by, string text, bool clearBefore = true, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>Scrolls the element into the viewport (center if possible).</summary>
        void ScrollIntoView(By by, bool alignToTop = true, CancellationToken ct = default);
        bool IsVisible(By by, TimeSpan? timeout = null, CancellationToken ct = default);
        bool IsInvisible(By by, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>
        /// Clicks an opener element, then scans all elements matching <paramref name="optionsBy"/>,
        /// picks the first whose VisibleText contains <paramref name="searchText"/> (case-insensitive),
        /// clicks it and returns true. Returns false if not found within timeout.
        /// </summary>
        bool SelectFromPopup(By openerBy, By optionsBy, string searchText, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>
        /// Selects an option on a native &lt;select&gt; element. Exactly one selector must be provided.
        /// </summary>
        void SelectNative(By selectBy, string? byVisibleText = null, string? byValue = null, int? byIndex = null, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>Clears the element's value using JS as a fallback when .Clear() fails.</summary>
        void ClearWithFallback(By by, CancellationToken ct = default);
    }
}
