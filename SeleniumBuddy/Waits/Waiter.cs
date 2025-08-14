using OpenQA.Selenium;
using SeleniumBuddy.Abstractions;
using System.Diagnostics;

namespace SeleniumBuddy.Waits
{
    /// <summary>
    /// Default implementation of <see cref="IWaiter"/> using manual polling,
    /// resilient to transient Selenium exceptions.
    /// </summary>
    public sealed class Waiter : IWaiter
    {
        private readonly IWebDriver _driver;
        private readonly IScriptExecutor _js;
        private readonly ISeleniumBuddyOptions _options;

        public Waiter(IWebDriver driver, IScriptExecutor js, ISeleniumBuddyOptions options)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _js = js ?? throw new ArgumentNullException(nameof(js));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Until(Func<IWebDriver, bool> condition, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            if (condition is null) throw new ArgumentNullException(nameof(condition));
            var deadline = DateTime.UtcNow + (timeout ?? _options.DefaultTimeout);
            var interval = poll ?? _options.PollingInterval;

            Exception last = null;

            while (DateTime.UtcNow <= deadline)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    if (condition(_driver))
                        return;
                    last = null;
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    last = ex;
                }

                Sleep(interval, ct);
            }

            throw new WebDriverTimeoutException(BuildTimeoutMessage("custom condition", timeout, last));
        }

        public IWebElement UntilVisible(By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            if (by is null) throw new ArgumentNullException(nameof(by));
            var deadline = DateTime.UtcNow + (timeout ?? _options.DefaultTimeout);
            var interval = poll ?? _options.PollingInterval;

            Exception last = null;

            while (DateTime.UtcNow <= deadline)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var el = _driver.FindElements(by).FirstOrDefault(e => SafeDisplayed(e));
                    if (el is not null)
                        return el;
                    last = null;
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    last = ex;
                }
                Sleep(interval, ct);
            }

            throw new WebDriverTimeoutException(BuildTimeoutMessage($"visible {Describe(by)}", timeout, last));
        }

        public IWebElement UntilClickable(By by, TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            if (by is null) throw new ArgumentNullException(nameof(by));
            var deadline = DateTime.UtcNow + (timeout ?? _options.DefaultTimeout);
            var interval = poll ?? _options.PollingInterval;

            Exception last = null;

            while (DateTime.UtcNow <= deadline)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var el = _driver.FindElements(by).FirstOrDefault(e => SafeDisplayed(e) && SafeEnabled(e));
                    if (el is not null)
                        return el;
                    last = null;
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    last = ex;
                }
                Sleep(interval, ct);
            }

            throw new WebDriverTimeoutException(BuildTimeoutMessage($"clickable {Describe(by)}", timeout, last));
        }

        public void UntilPageReady(TimeSpan? timeout = null, TimeSpan? poll = null, CancellationToken ct = default)
        {
            var deadline = DateTime.UtcNow + (timeout ?? _options.DefaultTimeout);
            var interval = poll ?? _options.PollingInterval;

            Exception last = null;

            while (DateTime.UtcNow <= deadline)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var state = _js.Execute<string>("return document.readyState;");
                    if (string.Equals(state, "complete", StringComparison.OrdinalIgnoreCase))
                        return;
                    last = null;
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    last = ex;
                }
                Sleep(interval, ct);
            }

            throw new WebDriverTimeoutException(BuildTimeoutMessage("document.readyState == 'complete'", timeout, last));
        }

        // -------- helpers --------

        private static bool SafeDisplayed(IWebElement e)
        {
            try { return e.Displayed; } catch { return false; }
        }

        private static bool SafeEnabled(IWebElement e)
        {
            try { return e.Enabled; } catch { return false; }
        }

        private static bool IsTransient(Exception ex) =>
            ex is WebDriverException
            || ex is NoSuchElementException
            || ex is StaleElementReferenceException
            || ex is InvalidOperationException
            || ex is ElementNotInteractableException
            || ex is ElementClickInterceptedException
            || ex is TimeoutException;

        public static void Sleep(TimeSpan interval, CancellationToken? ct = null)
        {
            if (interval <= TimeSpan.Zero) return;

            if (ct == null)
            {
                Thread.Sleep(interval);
            }
            else
            {
                try
                {
                    var ms = (int)Math.Max(1, interval.TotalMilliseconds);
                    ct.Value.WaitHandle.WaitOne(ms);
                    ct.Value.ThrowIfCancellationRequested();
                }
                catch (ObjectDisposedException)
                {
                    Debug.WriteLine("[SeleniumBuddy] WaitHandle disposed during Sleep — exiting wait loop.");
                }
            }
        }

        private static string Describe(By by)
        {
            var s = by.ToString();
            var idx = s.IndexOf(": ", StringComparison.Ordinal);
            if (idx > 0 && idx + 2 < s.Length)
            {
                var kind = s[..idx].Trim();
                var val = s[(idx + 2)..].Trim();
                return $"{kind}('{val}')";
            }
            return s;
        }

        private static string BuildTimeoutMessage(string what, TimeSpan? timeout, Exception last) =>
            last is null
                ? $"Timed out waiting for {what} after {(timeout ?? TimeSpan.Zero).TotalSeconds:0.#}s."
                : $"Timed out waiting for {what} after {(timeout ?? TimeSpan.Zero).TotalSeconds:0.#}s. Last error: {last.GetType().Name}: {last.Message}";
    }
}
