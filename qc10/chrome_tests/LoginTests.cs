using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ChromeTests
{
    [TestFixture, Order(1), Category("chrome")]
    public class LoginTests : BaseTest
    {
        private TestConfig _config;

        protected override string BrowserName => "chrome";

        [SetUp]
        public void Setup()
        {
            _config = ConfigReader.LoadConfig("..\\..\\..\\Config\\config.json");
            Driver.Navigate().GoToUrl($"{_config.BaseUrl}user/login");
        }

        [Test, Order(5)]
        public void Login_WithCorrectUserData_ShouldSuccess()
        {
            var loginPage = new LoginPage(Driver);
            loginPage.EnterLogin(_config.CorrectUser.Login);
            loginPage.EnterPassword(_config.CorrectUser.Password);
            loginPage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.True(loginPage.IsLoginSuccess(), "Плашка с успешным входом не появилась");
        }

        [Test, Order(1)]
        public void Login_WithIncorrectUserData_ShouldBeFailed()
        {
            var loginPage = new LoginPage(Driver);
            loginPage.EnterLogin(_config.IncorrectUser.Login);
            loginPage.EnterPassword(_config.IncorrectUser.Password);
            loginPage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.True(loginPage.IsLoginFailed(), "Плашка с ошибкой входа не появилась");
        }


        [Test, Order(2)]
        public void Login_WithCorrectUserLoginAndIncorrectPassword_ShouldBeFailed()
        {
            var loginPage = new LoginPage(Driver);
            loginPage.EnterLogin(_config.CorrectUser.Login);
            loginPage.EnterPassword(_config.IncorrectUser.Password);
            loginPage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.True(loginPage.IsLoginFailed(), "Плашка с ошибкой входа не появилась");
        }


        [Test, Order(3)]
        public void Login_WithIncorrectUserLoginAndCorrectPassword_ShouldBeFailed()
        {
            var loginPage = new LoginPage(Driver);
            loginPage.EnterLogin(_config.IncorrectUser.Login);
            loginPage.EnterPassword(_config.CorrectUser.Password);
            loginPage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.True(loginPage.IsLoginFailed(), "Плашка с ошибкой входа не появилась");
        }

        [Test, Order(4)]
        public void Login_WithEmptyData_ShouldBeFailed()
        {
            var loginPage = new LoginPage(Driver);
            loginPage.Submit();
            Thread.Sleep(2000);
            ClassicAssert.False(loginPage.IsLoginSuccess(), "Плашка с успешным входом появилась");
            ClassicAssert.False(loginPage.IsLoginFailed(), "Плашка с ошибкой входа появилась");
            ClassicAssert.That(Driver.Url, Is.EqualTo($"{_config.BaseUrl}user/login"));
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

