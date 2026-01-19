using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SpecFlowTests.Hooks;

/// <summary>
/// Hooks do SpecFlow para gerenciar o ciclo de vida do WebDriver
/// </summary>
[Binding]
public class WebDriverHooks
{
    private readonly ScenarioContext _scenarioContext;

    public WebDriverHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        Console.WriteLine("\n========================================");
        Console.WriteLine($"[SPECFLOW] Iniciando cenário: {_scenarioContext.ScenarioInfo.Title}");
        Console.WriteLine($"[SPECFLOW] Tags: {string.Join(", ", _scenarioContext.ScenarioInfo.Tags)}");
        Console.WriteLine("========================================\n");

        // Configurar o WebDriver
        new DriverManager().SetUpDriver(new ChromeConfig());
        
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        
        var driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

        _scenarioContext["WebDriver"] = driver;
    }

    [AfterScenario]
    public void AfterScenario()
    {
        if (_scenarioContext.TryGetValue("WebDriver", out IWebDriver driver))
        {
            Console.WriteLine("\n========================================");
            Console.WriteLine($"[SPECFLOW] Finalizando cenário: {_scenarioContext.ScenarioInfo.Title}");
            Console.WriteLine($"[SPECFLOW] Status: {_scenarioContext.ScenarioExecutionStatus}");
            Console.WriteLine("========================================\n");

            driver?.Quit();
            driver?.Dispose();
        }
    }

    [AfterStep]
    public void AfterStep()
    {
        // Log do step executado
        var stepInfo = _scenarioContext.StepContext.StepInfo;
        Console.WriteLine($"[STEP] {stepInfo.StepDefinitionType}: {stepInfo.Text}");
    }
}
