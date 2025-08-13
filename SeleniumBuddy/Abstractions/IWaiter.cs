using OpenQA.Selenium;

namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// Encapsulates robust waits (visibility, clickability, document readiness).
    /// </summary>
    public interface IWaiter
    {
        /// <summary>Waits until a custom condition returns true or times out (throws).</summary>
        void Until(Func<IWebDriver, bool> condition, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default);

        /// <summary>Waits until an element located by <paramref name="by"/> is visible. Returns the element or throws on timeout.</summary>
        IWebElement UntilVisible(By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default);

        /// <summary>Waits until an element located by <paramref name="by"/> is enabled and displayed (clickable). Returns it or throws.</summary>
        IWebElement UntilClickable(By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default);

        /// <summary>Waits until document.readyState == "complete".</summary>
        void UntilPageReady(TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default);
    }
}
