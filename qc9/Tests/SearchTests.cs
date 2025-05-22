using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

[TestFixture, Order(2), Category("firefox"), Category("chrome")]
public class SearchTests : BaseTest
{
    private TestConfig config;

    public SearchTests(string browser) : base(browser)
    { }

    [SetUp]
    public void Setup()
    {
        base.Setup();
        config = ConfigReader.LoadConfig("..\\..\\..\\Config\\config.json");
        driver.Navigate().GoToUrl($"{config.BaseUrl}");
    }

    [Test]
    public void Search_WithProductName_ShouldBeSuccessful()
    {
        var homePage = new HomePage(driver);
        homePage.EnterSearch("watches");
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
