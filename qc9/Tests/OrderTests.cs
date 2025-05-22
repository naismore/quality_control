using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

[TestFixture, Category("firefox"), Category("chrome")]
public class OrderTests : BaseTest
{
    private TestConfig config;

    public OrderTests(string browser) : base(browser)
    {
    }

    [SetUp]
    public new void Setup()
    {
        base.Setup();
        config = ConfigReader.LoadConfig("..\\..\\..\\Config\\config.json");
        driver.Navigate().GoToUrl($"{config.BaseUrl}");
    }

    [Test]
    public void CreateOrder_WithProductInBucket_ShouldBeSuccess()
    {
        var homePage = new HomePage(driver);
        homePage.ClickOnBucketButton();
        homePage.ClickOnCreateOrderButton();
        Thread.Sleep(2000);
        homePage.Submit();
        Thread.Sleep(2000);
        ClassicAssert.That(driver.Url, Is.EqualTo($"{config.BaseUrl}search?s=watches"));
    }


    [Test]
    public void Search_WithEmptyQuery_ShouldBeFailed()
    {
        var homePage = new HomePage(driver);
        homePage.Submit();
        Thread.Sleep(2000);
        ClassicAssert.That(driver.Url, Is.EqualTo(config.BaseUrl));
    }

    [TearDown]
    public void TearDown()
    {
        if (driver != null)
        {
            driver.Quit();
            driver.Dispose();
            driver = null;
        }
    }
}
