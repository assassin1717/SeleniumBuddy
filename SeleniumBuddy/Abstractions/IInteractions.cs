using OpenQA.Selenium;

namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// High-level, resilient interactions that combine waits + retries.
    /// </summary>
    public interface IInteractions
    {
        /// <summary>
        /// Scrolls the target element into the viewport (if needed) and clicks it as soon as it becomes visible and clickable.
        /// </summary>
        void ClickWhenVisible(By by, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>
        /// Waits until the element is ready for text input, optionally clears its existing value, and then types the specified text.
        /// </summary>
        void TypeWhenReady(By by, string text, bool clearBefore = true, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>
        /// Scrolls the target element into the viewport. The position is controlled by <paramref name="alignToTop"/>.
        /// </summary>
        void ScrollIntoView(By by, bool alignToTop = true, CancellationToken ct = default);

        /// <summary>
        /// Determines whether the element is visible within the given timeout.
        /// </summary>
        bool IsVisible(By by, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>
        /// Determines whether the element is invisible (either hidden or removed from the DOM) within the given timeout.
        /// </summary>
        bool IsInvisible(By by, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>
        /// Clicks the element identified by <paramref name="openerBy"/> to open a popup or dropdown,  
        /// then searches all elements matching <paramref name="optionsBy"/> for the first whose visible text contains  
        /// <paramref name="searchText"/> (case-insensitive). If found, it clicks the match <c>true</c>.  
        /// </summary>
        void SelectFromPopup(By openerBy, By optionsBy, string searchText, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>
        /// Selects an option from a native HTML <c>&lt;select&gt;</c> element. Exactly one selector parameter must be provided:  
        /// by visible text, by value, or by index.
        /// </summary>
        void SelectNative(By selectBy, string byVisibleText = null, string byValue = null, int? byIndex = null, TimeSpan? timeout = null, CancellationToken ct = default);

        /// <summary>
        /// Clears the element's value, using the standard Selenium <c>.Clear()</c> method first.  
        /// If that fails, a JavaScript-based fallback is used to set the value to an empty string.
        /// </summary>
        void ClearWithFallback(By by, CancellationToken ct = default);

    }
}
