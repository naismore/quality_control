using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V134.Audits;
using OpenQA.Selenium.Support.UI;

public class ItemPage
{
    private IWebDriver _driver;
    private By submitButton = By.Id("productAdd");
    private By selectColorElement = By.CssSelector("div.available select");
    private By modalBucketWindow = By.CssSelector(".modal-dialog");

    public ItemPage(IWebDriver driver)
    {
        this._driver = driver;
    }

    public void Submit()
    {
        _driver.FindElement(submitButton).Click();
    }

    public void SelectColor()
    {
        Random random = new Random();
        var selectElement = _driver.FindElement(selectColorElement);
        var select = new SelectElement(selectElement);
        select.SelectByIndex(random.Next(0, 4));
    }

    public bool IsAddingToBucketSuccess()
    {
        return _driver.FindElements(modalBucketWindow).Count > 0;
    }
}