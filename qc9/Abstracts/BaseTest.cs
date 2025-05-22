using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

public abstract class BaseTest
{
    protected IWebDriver driver;
    protected string Browser { get; set; } = "chrome";

    protected BaseTest(string browser)
    {
        Browser = browser;
    }

    [SetUp]
    public void Setup()
    {
        var hubUrl = new Uri("http://localhost:4444/wd/hub");

        DriverOptions options = Browser.ToLower() switch
        {
            "chrome" => new ChromeOptions(),
            "firefox" => new FirefoxOptions(),
            _ => throw new ArgumentException("Unsupported browser")
        };

        driver = new RemoteWebDriver(hubUrl, options);
        driver.Manage().Window.Maximize();
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Quit();
        driver?.Dispose();
    }
}