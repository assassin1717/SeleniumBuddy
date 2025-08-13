using OpenQA.Selenium;

namespace SeleniumBuddy.Exceptions
{
    public sealed class InteractionFailedException : Exception
    {
        public string ActionName { get; }
        public By Locator { get; }
        public string ScreenshotPath { get; }

        public InteractionFailedException(
            string actionName,
            By locator,
            string screenshotPath,
            Exception inner)
            : base(BuildMessage(actionName, locator, screenshotPath), inner)
        {
            ActionName = actionName;
            Locator = locator ?? throw new ArgumentNullException(nameof(locator));
            ScreenshotPath = screenshotPath;
        }

        private static string BuildMessage(string actionName, By locator, string path)
            => path is null
                ? $"Interaction '{actionName}' failed for {locator}."
                : $"Interaction '{actionName}' failed for {locator}. Screenshot: {path}";
    }
}
