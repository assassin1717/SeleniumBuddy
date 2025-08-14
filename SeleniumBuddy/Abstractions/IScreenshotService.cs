namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// Captures screenshots (e.g., on failure or for reports).
    /// </summary>
    public interface IScreenshotService
    {
        /// <summary>
        /// Captures a screenshot and returns the absolute file path where it was saved,  
        /// or <c>null</c> if screenshot capture is not supported in the current context.  
        /// Implementations may optionally stream the image bytes to another destination  
        /// instead of saving to disk.
        /// </summary>
        string Capture(string namePrefix = null, bool isFail = true, CancellationToken ct = default);

    }
}
