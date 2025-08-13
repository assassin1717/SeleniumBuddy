using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumBuddy.Abstractions;
using SeleniumBuddy.Exceptions;
using SeleniumBuddy.Waits;
using System.Diagnostics;

namespace SeleniumBuddy.Interactions
{
    /// <summary>
    /// High-level, resilient interactions built on top of waits, retry policy, and optional screenshots.
    /// </summary>
    public sealed class Interactions : IInteractions
    {
        private readonly IWebDriver _driver;
        private readonly IWaiter _waiter;
        private readonly IScriptExecutor _js;
        private readonly IRetryPolicy _retry;
        private readonly IScreenshotService _shots;
        private readonly ISeleniumBuddyOptions _options;

        public Interactions(
            IWebDriver driver,
            IWaiter waiter,
            IScriptExecutor js,
            IRetryPolicy retry,
            ISeleniumBuddyOptions options,
            IScreenshotService screenshots = null)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _waiter = waiter ?? throw new ArgumentNullException(nameof(waiter));
            _js = js ?? throw new ArgumentNullException(nameof(js));
            _retry = retry ?? throw new ArgumentNullException(nameof(retry));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _shots = screenshots;
        }

        public void ClickWhenVisible(By by, TimeSpan? timeout = null, CancellationToken ct = default)
            => ExecuteWithRescue(nameof(ClickWhenVisible), by, async token =>
            {
                var el = _waiter.UntilClickable(by, timeout ?? _options.DefaultTimeout, _options.PollingInterval, token);
                el.Click();
                await Task.CompletedTask;
            }, ct);

        public void TypeWhenReady(By by, string text, bool clearBefore = true, TimeSpan? timeout = null, CancellationToken ct = default)
            => ExecuteWithRescue(nameof(TypeWhenReady), by, async token =>
            {
                var el = _waiter.UntilVisible(by, timeout ?? _options.DefaultTimeout, _options.PollingInterval, token);
                if (clearBefore)
                {
                    try { el.Clear(); }
                    catch { _js.Execute("arguments[0].value='';", token, el); }
                }
                el.SendKeys(text ?? string.Empty);
                await Task.CompletedTask;
            }, ct);

        public void ScrollIntoView(By by, bool alignToTop = true, CancellationToken ct = default)
            => ExecuteWithRescue(nameof(ScrollIntoView), by, async token =>
            {
                var el = _driver.FindElement(by);
                _js.Execute("arguments[0].scrollIntoView(arguments[1]);", token, el, alignToTop);
                await Task.CompletedTask;
            }, ct);

        public void ClearWithFallback(By by, CancellationToken ct = default)
            => ExecuteWithRescue(nameof(ClearWithFallback), by, async token =>
            {
                var el = _driver.FindElement(by);
                try { el.Clear(); }
                catch { _js.Execute("arguments[0].value='';", token, el); }
                await Task.CompletedTask;
            }, ct);

        public bool IsVisible(By by, TimeSpan? timeout = null, CancellationToken ct = default)
        {
            var until = DateTime.UtcNow + (timeout ?? _options.DefaultTimeout);
            while (DateTime.UtcNow <= until)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var el = _driver.FindElements(by).FirstOrDefault(e => SafeDisplayed(e));
                    if (el is not null) return true;
                }
                catch (WebDriverException) { }

                Waiter.Sleep(_options.PollingInterval, ct);
            }
            return false;
        }

        public bool IsInvisible(By by, TimeSpan? timeout = null, CancellationToken ct = default)
        {
            var until = DateTime.UtcNow + (timeout ?? _options.DefaultTimeout);
            while (DateTime.UtcNow <= until)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var existsAndVisible = _driver.FindElements(by).Any(e => SafeDisplayed(e));
                    if (!existsAndVisible) return true;
                }
                catch (WebDriverException)
                {
                    // If querying throws transiently, treat as not yet invisible → keep polling
                }
                Waiter.Sleep(_options.PollingInterval, ct);
            }
            return false;
        }

        public bool SelectFromPopup(By openerBy, By optionsBy, string searchText, TimeSpan? timeout = null, CancellationToken ct = default)
    => ExecuteWithRescueBool(nameof(SelectFromPopup), openerBy, async token =>
    {
        if (string.IsNullOrWhiteSpace(searchText))
            throw new ArgumentException("searchText must be non-empty.", nameof(searchText));

        var to = timeout ?? _options.DefaultTimeout;
        var until = DateTime.UtcNow + to;

        var opener = _waiter.UntilClickable(openerBy, to, _options.PollingInterval, token);
        opener.Click();

        while (DateTime.UtcNow <= until)
        {
            token.ThrowIfCancellationRequested();

            var candidates = _driver.FindElements(optionsBy)
                                    .Where(SafeDisplayed)
                                    .ToList();

            if (candidates.Count > 0)
            {
                var match = candidates.FirstOrDefault(e =>
                {
                    var text = GetVisibleText(e);
                    return text.IndexOf(searchText.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
                });

                if (match is not null)
                {
                    _js.Execute("arguments[0].scrollIntoView(true);", token, match);
                    match.Click();
                    return true;
                }
            }

            Waiter.Sleep(_options.PollingInterval, token);
        }

        return false;
    }, ct);


        public void SelectNative(By selectBy, string byVisibleText = null, string byValue = null, int? byIndex = null, TimeSpan? timeout = null, CancellationToken ct = default)
            => ExecuteWithRescue(nameof(SelectNative), selectBy, async token =>
            {
                var provided = (byVisibleText is not null ? 1 : 0) + (byValue is not null ? 1 : 0) + (byIndex is not null ? 1 : 0);
                if (provided != 1)
                    throw new ArgumentException("Exactly one of byVisibleText, byValue, byIndex must be provided.");

                var el = _waiter.UntilVisible(selectBy, timeout ?? _options.DefaultTimeout, _options.PollingInterval, token);
                var select = new SelectElement(el);

                if (byVisibleText is not null) select.SelectByText(byVisibleText);
                else if (byValue is not null) select.SelectByValue(byValue);
                else if (byIndex is not null) select.SelectByIndex(byIndex.Value);

                await Task.CompletedTask;
            }, ct);

        // -------- helpers --------

        private void ExecuteWithRescue(string actionName, By by, Func<CancellationToken, Task> action, CancellationToken ct)
        {
            try
            {
                _retry.ExecuteAsync(action, ct).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                string shotPath = null;

                if (_options.ScreenshotOnFailure && _shots is not null)
                {
                    try
                    {
                        var safe = Sanitize($"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{actionName}_{Describe(by)}");
                        shotPath = _shots.Capture(safe, ct);
                    }
                    catch (Exception capEx)
                    {
                        Trace.WriteLine($"[SeleniumBuddy] Screenshot capture failed: {capEx.Message}");
                    }
                }

                throw new InteractionFailedException(actionName, by, shotPath, ex);
            }
        }

        private bool ExecuteWithRescueBool(string actionName, By by, Func<CancellationToken, Task<bool>> action, CancellationToken ct)
        {
            try
            {
                return _retry.ExecuteAsync(action, ct).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                string shotPath = null;
                if (_options.ScreenshotOnFailure && _shots is not null)
                {
                    try
                    {
                        var safe = Sanitize($"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{actionName}_{Describe(by)}");
                        shotPath = _shots.Capture(safe, ct);
                    }
                    catch (Exception capEx)
                    {
                        Trace.WriteLine($"[SeleniumBuddy] Screenshot capture failed: {capEx.GetType().Name}: {capEx.Message}");
                    }
                }
                throw new InteractionFailedException(actionName, by, shotPath, ex);
            }
        }

        private static string Describe(By by)
        {
            var s = by?.ToString() ?? "By(unknown)";
            var idx = s.IndexOf(": ", StringComparison.Ordinal);
            if (idx > 0 && idx + 2 < s.Length)
            {
                var kind = s.Substring(0, idx).Trim();
                var val = s.Substring(idx + 2).Trim();
                return $"{kind}({val})";
            }
            return s;
        }

        private static string Sanitize(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Replace(' ', '_');
        }

        private static string GetVisibleText(IWebElement e)
        {
            try
            {
                var t = e.Text;
                if (!string.IsNullOrWhiteSpace(t)) return t.Trim();
            }
            catch { /* ignore transient */ }

            try
            {
                var aria = e.GetAttribute("aria-label");
                if (!string.IsNullOrWhiteSpace(aria)) return aria.Trim();
            }
            catch { /* ignore transient */ }

            try
            {
                var title = e.GetAttribute("title");
                if (!string.IsNullOrWhiteSpace(title)) return title.Trim();
            }
            catch { /* ignore transient */ }

            return string.Empty;
        }

        private static bool SafeDisplayed(IWebElement e)
        {
            try { return e.Displayed; }
            catch (StaleElementReferenceException) { return false; }
            catch (NoSuchElementException) { return false; }
            catch (WebDriverException) { return false; }
        }
    }
}
