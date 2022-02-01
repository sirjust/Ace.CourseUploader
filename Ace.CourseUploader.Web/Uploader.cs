using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Ace.CourseUploader.Web
{
    public class Uploader : IUploader
    {
        IWebDriver _driver;
        WebDriverWait _wait;
        IConfiguration _configuration;

        public Uploader(IWebDriver driver, WebDriverWait wait)
        {
            _driver = driver;
            _wait = wait;

            _configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets<Uploader>()
            .Build();
        }

        public void Login()
        {
            _driver.Url = _configuration["LoginUrl"];

            _driver.FindElement(By.Id("user_login")).SendKeys(_configuration["Id"]);
            _driver.FindElement(By.Id("user_pass")).SendKeys(_configuration["Password"]);

            _driver.FindElement(By.Id("wp-submit")).Click();
        }
    }
}
