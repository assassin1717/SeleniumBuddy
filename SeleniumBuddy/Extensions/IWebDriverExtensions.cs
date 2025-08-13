using OpenQA.Selenium;
using SeleniumBuddy.Abstractions;

namespace SeleniumBuddy.Extensions
{
    public static class IWebDriverExtensions
    {
        /// <summary>
        /// Waits until the document is fully loaded (document.readyState == "complete").
        /// </summary>
        public static void WaitForPageReady(this IWebDriver driver, IScriptExecutor js, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            if (js is null) throw new ArgumentNullException(nameof(js));

            var end = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(10));
            var interval = poll ?? TimeSpan.FromMilliseconds(250);

            while (DateTime.UtcNow < end)
            {
                ct.ThrowIfCancellationRequested();
                var state = js.Execute<string>("return document.readyState;");
                if (string.Equals(state, "complete", StringComparison.OrdinalIgnoreCase))
                    return;

                Thread.Sleep(interval);
            }

            throw new WebDriverTimeoutException("Timed out waiting for document.readyState == 'complete'.");
        }

        /// <summary>
        /// Waits for a visible element located by <paramref name="by"/> and returns it.
        /// </summary>
        public static IWebElement WaitVisible(this IWebDriver driver, IWaiter waiter, By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            if (waiter is null) throw new ArgumentNullException(nameof(waiter));
            if (by is null) throw new ArgumentNullException(nameof(by));

            return waiter.UntilVisible(by, timeout, poll, ct);
        }

        /// <summary>
        /// Waits for a clickable element located by <paramref name="by"/> and returns it.
        /// </summary>
        public static IWebElement WaitClickable(this IWebDriver driver, IWaiter waiter, By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            if (waiter is null) throw new ArgumentNullException(nameof(waiter));
            if (by is null) throw new ArgumentNullException(nameof(by));

            return waiter.UntilClickable(by, timeout, poll, ct);
        }

        /// <summary>
        /// Scrolls the first element that matches <paramref name="by"/> into view using JavaScript.
        /// </summary>
        public static void ScrollIntoView(this IWebDriver driver, IScriptExecutor js, By by, bool alignToTop = true, CancellationToken ct = default)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            if (js is null) throw new ArgumentNullException(nameof(js));
            if (by is null) throw new ArgumentNullException(nameof(by));

            var element = driver.FindElement(by);
            js.Execute("arguments[0].scrollIntoView(arguments[1]);", ct, element, alignToTop);
        }

        /// <summary>
        /// Clicks an element after waiting until it is clickable.
        /// </summary>
        public static void ClickWhenVisible(this IWebDriver driver, IWaiter waiter, By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            var el = driver.WaitClickable(waiter, by, timeout, poll, ct);
            el.Click();
        }

        /// <summary>
        /// Types text into an element after waiting until it is visible (optionally clears first).
        /// </summary>
        public static void TypeWhenReady(this IWebDriver driver, IWaiter waiter, By by, string text, bool clearBefore = true, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            var el = driver.WaitVisible(waiter, by, timeout, poll, ct);
            if (clearBefore) el.Clear();
            el.SendKeys(text);
        }

        /// <summary>
        /// Clears the value via JS when <see cref="IWebElement.Clear"/> is unreliable.
        /// </summary>
        public static void ClearWithJsFallback(this IWebDriver driver, IScriptExecutor js, By by, CancellationToken ct = default)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            if (js is null) throw new ArgumentNullException(nameof(js));
            if (by is null) throw new ArgumentNullException(nameof(by));

            var el = driver.FindElement(by);
            try
            {
                el.Clear();
            }
            catch
            {
                js.Execute("arguments[0].value = '';", ct, el);
            }
        }
    }
}
