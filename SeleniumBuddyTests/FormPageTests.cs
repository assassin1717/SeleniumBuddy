using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumBuddy.Abstractions;
using SeleniumBuddy.Configurations;
using SeleniumBuddy.Core;
using SeleniumBuddy.Interactions;
using SeleniumBuddy.Waits;

namespace SeleniumBuddyTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class FormPageTests
    {
        private IWebDriver _driver;
        private IWaiter _waits = default!;
        private IInteractions _buddy = default!;

        [SetUp]
        public void SetUp()
        {
            var chrome = new ChromeOptions();
            chrome.AddArgument("--headless=new");
            chrome.AddArgument("--window-size=1280,900");

            _driver = new ChromeDriver(chrome);

            var opts = new SeleniumBuddyOptionsBuilder()
                .WithDefaultTimeout(TimeSpan.FromSeconds(5))
                .WithPollingInterval(TimeSpan.FromMilliseconds(200))
                .WithRetryAttempts(2)
                .WithScreenshotOnFailure(true)
                .Build();

            var js = new ScriptExecutor(_driver);
            _waits = new Waiter(_driver, js, opts);
            var retry = new RetryPolicy(opts);
            var shots = new ScreenshotService(_driver);

            _buddy = new Interactions(_driver, _waits, js, retry, opts, shots);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _driver?.Dispose();
            }
            catch
            {

            }
        }

        [Test]
        [Category("Form")]
        public void Submit_Form_Succeeds()
        {
            _driver.Navigate().GoToUrl("https://fronttestlab.web.app/form");
            _waits.UntilPageReady();

            _buddy.TypeWhenReady(By.CssSelector("[data-testid='input-name'] > input"), "Tiago Barbosa");
            _buddy.TypeWhenReady(By.CssSelector("[data-testid='input-email'] > input"), "tiago@example.com");
            _buddy.TypeWhenReady(By.CssSelector("[data-testid='input-password'] > input"), "HelloFromSeleniumBuddy!");
            var selected = _buddy.SelectFromPopup(
                openerBy: By.CssSelector("[data-testid='dropdown-role']"),
                optionsBy: By.CssSelector("[data-testid='dropdown-role'] .item"),
                searchText: "Developer",
                timeout: TimeSpan.FromSeconds(8));
            Assert.IsTrue(selected, "Expected to select 'Developer' from the dropdown.");
            _buddy.ClickWhenVisible(By.CssSelector("[data-testid='radio-male']"));
            _buddy.ClickWhenVisible(By.CssSelector("[data-testid='checkbox-terms']"));

            _buddy.IsInvisible(By.CssSelector("[data-testid='success-message']"));

            _buddy.ScrollIntoView(By.CssSelector("[data-testid='submit-button']"));
            _buddy.ClickWhenVisible(By.CssSelector("[data-testid='submit-button']"));

            _buddy.IsVisible(By.CssSelector("[data-testid='success-message']"));

            Waiter.Sleep(TimeSpan.FromSeconds(5));
        }

        [Test]
        [Category("Form")]
        public void Submit_ShowsError_When_EmailInvalid()
        {
            _driver.Navigate().GoToUrl("https://fronttestlab.web.app/form");
            _waits.UntilPageReady();

            _buddy.TypeWhenReady(By.CssSelector("[data-testid='input-name'] > input"), "Tiago Barbosa");
            _buddy.TypeWhenReady(By.CssSelector("[data-testid='input-email'] > input"), "tiago@example.com");

            _buddy.ClickWhenVisible(By.CssSelector("[data-testid='submit-button']"));

            _buddy.IsInvisible(By.CssSelector("#root > div.ui.container > div > form:nth-child(2) > div:nth-child(1) > div.ui.pointing.above.prompt.label"));
            _buddy.IsInvisible(By.CssSelector("#root > div.ui.container > div > form:nth-child(2) > div:nth-child(2) > div.ui.pointing.above.prompt.label"));
            _buddy.IsVisible(By.CssSelector("#root > div.ui.container > div > form:nth-child(2) > div:nth-child(3) > div.ui.pointing.above.prompt.label"));    
        }
    }
}