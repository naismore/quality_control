using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace FirefoxTests
{
    [TestFixture, Order(2), Category("firefox")]
    public class SearchTests : BaseTest
    {
        private TestConfig config;

        protected override string BrowserName => "firefox";

        [SetUp]
        public void Setup()
        {
            config = ConfigReader.LoadConfig("..\\..\\..\\Config\\config.json");
            Driver.Navigate().GoToUrl($"{config.BaseUrl}");
        }

        [Test]
        public void Search_WithProductName_ShouldBeSuccessful()
        {
            var homePage = new HomePage(Driver);
            homePage.EnterSearch("watches");
            homePage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.That(Driver.Url, Is.EqualTo($"{config.BaseUrl}search?s=watches"));
        }


        [Test]
        public void Search_WithEmptyQuery_ShouldBeFailed()
        {
            var homePage = new HomePage(Driver);
            homePage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.That(Driver.Url, Is.EqualTo(config.BaseUrl));
        }

        [TearDown]
        public void TearDown()
        {
            if (Driver != null)
            {
                Driver.Quit();
                Driver.Dispose();
            }
        }
    }
}
