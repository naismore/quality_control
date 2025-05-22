using OpenQA.Selenium;

public class HomePage
{
    private IWebDriver _driver;
    private By searchInput = By.Id("typeahead");
    private By submitButton = By.CssSelector("input[type='submit']");
    private By buckerButton = By.CssSelector("a[href='cart/show']");
    private By createOrderButton = By.CssSelector("a[href='cart / view']");
    private By modalBucketWindow = By.CssSelector(".modal-dialog");

    public HomePage(IWebDriver driver)
    {
        this._driver = driver;
    }

    public void EnterSearch(string query)
    {
        _driver.FindElement(searchInput).SendKeys(query);
    }

    public void ClickOnBucketButton()
    {
        _driver.FindElement(buckerButton).Click();
    }

    public bool modalBucketWindowIsActive()
    {
        return _driver.FindElements(modalBucketWindow).Count > 0;
    }

    public void Submit()
    {
        _driver.FindElement(submitButton).Click();
    }

    public void ClickOnCreateOrderButton()
    {
        _driver.FindElement(createOrderButton).Click();
    }
}