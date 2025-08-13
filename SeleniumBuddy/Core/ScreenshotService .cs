using OpenQA.Selenium;
using SeleniumBuddy.Abstractions;

namespace SeleniumBuddy.Core
{
    /// <summary>
    /// Captures screenshots using Selenium's <see cref="ITakesScreenshot"/> and saves to disk.
    /// </summary>
    public sealed class ScreenshotService : IScreenshotService
    {
        private readonly ITakesScreenshot _taker;
        private readonly string _directory;

        /// <param name="driver">WebDriver instance. If it does not implement <see cref="ITakesScreenshot"/>, capture will return null.</param>
        /// <param name="directory">
        /// Directory where screenshots are written. Defaults to "<app base>/Screenshots".
        /// </param>
        public ScreenshotService(IWebDriver driver, string directory = null)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            _taker = driver as ITakesScreenshot;
            _directory = directory ?? Path.Combine(AppContext.BaseDirectory, "Screenshots");
        }

        public string Capture(string namePrefix = null, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            if (_taker is null) return null;

            Directory.CreateDirectory(_directory);

            var prefix = string.IsNullOrWhiteSpace(namePrefix) ? "screenshot" : namePrefix!.Trim();
            var file = $"{prefix}_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}.png";
            var fullPath = Path.Combine(_directory, Sanitize(file));

            var shot = _taker.GetScreenshot();
            shot.SaveAsFile(fullPath);

            return fullPath;
        }

        private static string Sanitize(string filename)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                filename = filename.Replace(c, '_');
            return filename;
        }
    }
}
