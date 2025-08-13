using OpenQA.Selenium;
using SeleniumBuddy.Abstractions;

namespace SeleniumBuddy.Core
{
    /// <summary>
    /// Safe JavaScript executor wrapper around Selenium's <see cref="IJavaScriptExecutor"/>.
    /// </summary>
    public sealed class ScriptExecutor : IScriptExecutor
    {
        private readonly IWebDriver _driver;
        private readonly IJavaScriptExecutor _js;

        public ScriptExecutor(IWebDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _js = driver as IJavaScriptExecutor
                ?? throw new NotSupportedException("Driver does not support JavaScript execution (IJavaScriptExecutor).");
        }

        public object Execute(string script, CancellationToken ct = default, params object[] args)
        {
            if (script is null) throw new ArgumentNullException(nameof(script));
            ct.ThrowIfCancellationRequested();
            return _js.ExecuteScript(script, args);
        }

        public T Execute<T>(string script, CancellationToken ct = default, params object[] args)
        {
            var result = Execute(script, ct, args);
            if (result is null) return default;
            try
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch
            {
                // If direct conversion fails, attempt simple cast
                return result is T cast ? cast : default;
            }
        }
    }
}
