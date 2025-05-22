using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ChromeTests
{
    [TestFixture, Category("chrome")]
    public class OrderTests : BaseTest
    {
        private TestConfig config;

        protected override string BrowserName => "chrome";

        [SetUp]
        public new void Setup()
        {
            config = ConfigReader.LoadConfig("..\\..\\..\\Config\\config.json");
            Driver.Navigate().GoToUrl($"{config.BaseUrl}");
        }

        [Test]
        public void CreateOrder_WithProductInBucket_ShouldBeSuccess()
        {
            var homePage = new HomePage(Driver);
            homePage.ClickOnBucketButton();
            homePage.ClickOnCreateOrderButton();
            Thread.Sleep(2000);
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