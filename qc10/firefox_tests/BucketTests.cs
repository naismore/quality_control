using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace FirefoxTests
{
    [TestFixture, Order(3), Category("firefox")]
    public class BucketTests : BaseTest
    {
        private TestConfig config;

        protected override string BrowserName => "firefox";

        [SetUp]
        public void TestSetup()
        {
            base.SetupDriver();
            config = ConfigReader.LoadConfig("..\\..\\..\\Config\\config.json");
            Driver.Navigate().GoToUrl($"{config.BaseUrl}product/casio-mrp-700-1avef");
        }

        [Test]
        public void Add_ToBucketWithoutColorSelect_ShouldBeSuccess()
        {
            var itemPage = new ItemPage(Driver);
            itemPage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.True(itemPage.IsAddingToBucketSuccess(), "Модальное окно не появилось");
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


