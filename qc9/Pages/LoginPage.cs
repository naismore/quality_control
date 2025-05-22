using OpenQA.Selenium;

public class LoginPage
{
    private IWebDriver _driver;

    private By loginInput = By.Name("login");
    private By passwordInput = By.Name("password");
    private By submitButton = By.CssSelector("button[type='submit']");
    private By successBanner = By.CssSelector(".alert-success");
    private By dangerBanner = By.CssSelector(".alert-danger");

    public LoginPage(IWebDriver driver)
    {
        this._driver = driver;
    }

    public void EnterLogin(string login)
    {
        _driver.FindElement(loginInput).SendKeys(login);
    }

    public void EnterPassword(string password)
    {
        _driver.FindElement(passwordInput).SendKeys(password);
    }

    public void Submit()
    {
        _driver.FindElement(submitButton).Click();
    }

    public bool IsLoginSuccess()
    {
        return _driver.FindElements(successBanner).Count > 0;
    }

    public bool IsLoginFailed()
    {
        return _driver.FindElements(dangerBanner).Count > 0;
    }
}
