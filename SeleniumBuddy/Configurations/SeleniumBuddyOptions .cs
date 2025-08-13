using SeleniumBuddy.Abstractions;

namespace SeleniumBuddy.Configurations
{
    /// <summary>
    /// Immutable options for SeleniumBuddy. Use <see cref="SeleniumBuddyOptionsBuilder"/> to create instances.
    /// </summary>
    public sealed class SeleniumBuddyOptions : ISeleniumBuddyOptions
    {
        public TimeSpan DefaultTimeout { get; }
        public TimeSpan PollingInterval { get; }
        public int RetryAttempts { get; }
        public TimeSpan RetryBaseBackoff { get; }
        public bool ScreenshotOnFailure { get; }

        public SeleniumBuddyOptions(
            TimeSpan? defaultTimeout = null,
            TimeSpan? pollingInterval = null,
            int? retryAttempts = null,
            TimeSpan? retryBaseBackoff = null,
            bool? screenshotOnFailure = null)
        {
            DefaultTimeout = defaultTimeout ?? SeleniumBuddyDefaults.DefaultTimeout;
            PollingInterval = pollingInterval ?? SeleniumBuddyDefaults.PollingInterval;
            RetryAttempts = retryAttempts ?? SeleniumBuddyDefaults.RetryAttempts;
            RetryBaseBackoff = retryBaseBackoff ?? SeleniumBuddyDefaults.RetryBaseBackoff;
            ScreenshotOnFailure = screenshotOnFailure ?? SeleniumBuddyDefaults.ScreenshotOnFailure;

            Validate();
        }

        private void Validate()
        {
            if (DefaultTimeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(DefaultTimeout), "DefaultTimeout must be > 0.");

            if (PollingInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(PollingInterval), "PollingInterval must be > 0.");

            if (RetryAttempts < 0)
                throw new ArgumentOutOfRangeException(nameof(RetryAttempts), "RetryAttempts cannot be negative.");

            if (RetryBaseBackoff < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(RetryBaseBackoff), "RetryBaseBackoff cannot be negative.");
        }

        /// <summary>Create a new builder pre-filled with these values.</summary>
        public SeleniumBuddyOptionsBuilder ToBuilder() => new SeleniumBuddyOptionsBuilder(this);
    }
}
