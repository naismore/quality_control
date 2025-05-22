using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using NUnit.Framework;

public abstract class BaseTest
{
    protected IWebDriver Driver { get; private set; }

    protected abstract string BrowserName { get; }

    [SetUp]
    public void SetupDriver()
    {
        var hubUrl = new Uri("http://localhost:4444/wd/hub");
        if (BrowserName.ToLower() == "firefox")
        {
            var options = new FirefoxOptions
            {
                BrowserExecutableLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe", // Указываем путь к Firefox
                AcceptInsecureCertificates = true
            };
            Driver = new RemoteWebDriver(hubUrl, options);
        }
        else
        {
            // Конфигурация для Chrome
            var options = new ChromeOptions();
            Driver = new RemoteWebDriver(hubUrl, options);
        }
        Driver.Manage().Window.Maximize();
    }

    [TearDown]
    public void TearDownDriver()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}