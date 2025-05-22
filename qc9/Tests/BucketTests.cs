using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

[TestFixture, Order(3), Category("firefox"), Category("chrome")]
public class BucketTests : BaseTest
{
    private TestConfig config;
    private readonly string _browser;

    public BucketTests(string browser) : base(browser)
    { }

    [SetUp]
    public void Setup()
    {
        base.Setup();
        config = ConfigReader.LoadConfig("..\\..\\..\\Config\\config.json");
        driver.Navigate().GoToUrl($"{config.BaseUrl}product/casio-mrp-700-1avef");
    }

    [Test]
    public void Add_ToBucketWithoutColorSelect_ShouldBeSuccess()
    {
        var itemPage = new ItemPage(driver);
        itemPage.Submit();
        Thread.Sleep(2000);
        ClassicAssert.True(itemPage.IsAddingToBucketSuccess(), "Модальное окно не появилось");
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
