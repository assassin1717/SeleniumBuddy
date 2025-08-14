using OpenQA.Selenium;

namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// Encapsulates robust waits (visibility, clickability, document readiness).
    /// </summary>
    public interface IWaiter
    {
        /// <summary>
        /// Waits until the provided <paramref name="condition"/> delegate returns true,  
        /// or throws a timeout exception if the condition is not met within the specified duration.
        /// </summary>
        void Until(Func<IWebDriver, bool> condition, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default);

        /// <summary>
        /// Waits until an element located by <paramref name="by"/> becomes visible in the DOM.  
        /// Returns the matching <see cref="IWebElement"/> if found within the timeout; otherwise throws a timeout exception.
        /// </summary>
        IWebElement UntilVisible(By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default);

        /// <summary>
        /// Waits until an element located by <paramref name="by"/> is both visible and enabled (clickable).  
        /// Returns the <see cref="IWebElement"/> if found; otherwise throws a timeout exception.
        /// </summary>
        IWebElement UntilClickable(By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default);

        /// <summary>
        /// Waits until the page's <c>document.readyState</c> equals "complete",  
        /// indicating that the DOM is fully loaded and ready for interaction.
        /// </summary>
        void UntilPageReady(TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default);

    }
}
