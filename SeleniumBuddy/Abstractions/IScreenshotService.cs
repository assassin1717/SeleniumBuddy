namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// Captures screenshots (e.g., on failure or for reports).
    /// </summary>
    public interface IScreenshotService
    {
        /// <summary>
        /// Captures a screenshot and returns the absolute file path (or null if capture is not supported).
        /// Implementations may also stream bytes elsewhere instead of saving to disk.
        /// </summary>
        string Capture(string namePrefix = null, CancellationToken ct = default);
    }
}
