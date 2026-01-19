using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SpecFlowTests.PageObjects;

/// <summary>
/// Page Object para a página inicial dos Correios
/// </summary>
public class CorreiosHomePage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private const string Url = "https://www.correios.com.br/";

    public CorreiosHomePage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Navega para a página inicial dos Correios
    /// </summary>
    public void Navegar()
    {
        _driver.Navigate().GoToUrl(Url);
    }

    /// <summary>
    /// Aceita os cookies se o banner aparecer
    /// </summary>
    public void AceitarCookiesSeExistir()
    {
        try
        {
            var acceptButton = _wait.Until(d =>
            {
                var elements = d.FindElements(By.XPath("//*[contains(text(), 'Aceito')]"));
                return elements.FirstOrDefault(e => e.Displayed);
            });
            
            acceptButton?.Click();
            Thread.Sleep(500);
        }
        catch (WebDriverTimeoutException)
        {
            // Se o botão não aparecer, continua o teste normalmente
        }
    }
}
