using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace technicaltest_b3.Hooks;

/// <summary>
/// Hooks do SpecFlow para gerenciamento do ciclo de vida do WebDriver
/// </summary>
[Binding]
public class WebDriverHooks
{
    private readonly ScenarioContext _scenarioContext;
    private IWebDriver? _driver;

    public WebDriverHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    /// <summary>
    /// Inicializa o WebDriver antes de cada cenário
    /// </summary>
    [BeforeScenario(Order = 0)]
    public void BeforeScenario()
    {
        // Configurar o WebDriver
        new DriverManager().SetUpDriver(new ChromeConfig());
        
        var options = new ChromeOptions();
        // Executar em modo visível para permitir preenchimento manual do captcha
        // options.AddArgument("--headless"); // Comentado para ver o navegador
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        
        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

        // Armazenar no contexto para uso nos steps
        _scenarioContext.Set(_driver, "WebDriver");
    }

    /// <summary>
    /// Fecha o WebDriver após cada cenário
    /// </summary>
    [AfterScenario(Order = 0)]
    public void AfterScenario()
    {
        if (_scenarioContext.TryGetValue("WebDriver", out IWebDriver driver))
        {
            driver?.Quit();
            driver?.Dispose();
        }
    }

    /// <summary>
    /// Captura screenshot em caso de falha do cenário
    /// </summary>
    [AfterScenario(Order = 1)]
    public void CaptureScreenshotOnFailure()
    {
        if (_scenarioContext.TestError != null)
        {
            try
            {
                if (_scenarioContext.TryGetValue("WebDriver", out IWebDriver driver))
                {
                    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    var fileName = $"Screenshot_{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    var screenshotPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots", fileName);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
                    screenshot.SaveAsFile(screenshotPath);
                    
                    Console.WriteLine($"Screenshot salvo em: {screenshotPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao capturar screenshot: {ex.Message}");
            }
        }
    }
}
