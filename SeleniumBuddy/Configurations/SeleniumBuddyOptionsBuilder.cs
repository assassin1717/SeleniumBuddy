using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumBuddy.Configurations
{
    /// <summary>
    /// Fluent builder for <see cref="SeleniumBuddyOptions"/>.
    /// </summary>
    public sealed class SeleniumBuddyOptionsBuilder
    {
        private TimeSpan _defaultTimeout = SeleniumBuddyDefaults.DefaultTimeout;
        private TimeSpan _pollingInterval = SeleniumBuddyDefaults.PollingInterval;
        private int _retryAttempts = SeleniumBuddyDefaults.RetryAttempts;
        private TimeSpan _retryBaseBackoff = SeleniumBuddyDefaults.RetryBaseBackoff;
        private bool _screenshotOnFailure = SeleniumBuddyDefaults.ScreenshotOnFailure;

        public SeleniumBuddyOptionsBuilder() { }

        public SeleniumBuddyOptionsBuilder(SeleniumBuddyOptions seed)
        {
            _defaultTimeout = seed.DefaultTimeout;
            _pollingInterval = seed.PollingInterval;
            _retryAttempts = seed.RetryAttempts;
            _retryBaseBackoff = seed.RetryBaseBackoff;
            _screenshotOnFailure = seed.ScreenshotOnFailure;
        }

        public SeleniumBuddyOptionsBuilder WithDefaultTimeout(TimeSpan value)
        {
            _defaultTimeout = value;
            return this;
        }

        public SeleniumBuddyOptionsBuilder WithPollingInterval(TimeSpan value)
        {
            _pollingInterval = value;
            return this;
        }

        public SeleniumBuddyOptionsBuilder WithRetryAttempts(int value)
        {
            _retryAttempts = value;
            return this;
        }

        public SeleniumBuddyOptionsBuilder WithRetryBaseBackoff(TimeSpan value)
        {
            _retryBaseBackoff = value;
            return this;
        }

        public SeleniumBuddyOptionsBuilder WithScreenshotOnFailure(bool enabled = true)
        {
            _screenshotOnFailure = enabled;
            return this;
        }

        public SeleniumBuddyOptions Build() =>
            new SeleniumBuddyOptions(
                defaultTimeout: _defaultTimeout,
                pollingInterval: _pollingInterval,
                retryAttempts: _retryAttempts,
                retryBaseBackoff: _retryBaseBackoff,
                screenshotOnFailure: _screenshotOnFailure
            );
    }
}
