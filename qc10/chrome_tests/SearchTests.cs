using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ChromeTests
{
    [TestFixture, Order(2), Category("chrome")]
    public class SearchTests : BaseTest
    {
        private TestConfig _config;

        protected override string BrowserName => "chrome";

        [SetUp]
        public void Setup()
        {
            base.SetupDriver();
            _config = ConfigReader.LoadConfig("..\\..\\..\\Config\\config.json");
            Driver.Navigate().GoToUrl($"{_config.BaseUrl}");
        }

        [Test]
        public void Search_WithProductName_ShouldBeSuccessful()
        {
            var homePage = new HomePage(Driver);
            homePage.EnterSearch("watches");
            homePage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.That(Driver.Url, Is.EqualTo($"{_config.BaseUrl}search?s=watches"));
        }


        [Test]
        public void Search_WithEmptyQuery_ShouldBeFailed()
        {
            var homePage = new HomePage(Driver);
            homePage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.That(Driver.Url, Is.EqualTo(_config.BaseUrl));
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
