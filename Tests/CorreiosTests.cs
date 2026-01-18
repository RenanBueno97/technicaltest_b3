using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using technicaltest_b3.PageObjects;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Xunit;

namespace technicaltest_b3.Tests;

/// <summary>
/// Testes xUnit para validar funcionalidades do site dos Correios
/// Baseado no teste Playwright: correios.spec.ts
/// </summary>
[TestCaseOrderer("technicaltest_b3.Tests.PriorityOrderer", "technicaltest_b3")]
public class CorreiosTests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly CorreiosHomePage _homePage;
    private readonly BuscaCepPage _buscaCepPage;
    private readonly RastreamentoPage _rastreamentoPage;

    public CorreiosTests()
    {
        // Configurar o WebDriver
        new DriverManager().SetUpDriver(new ChromeConfig());
        
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        
        _driver = new ChromeDriver(options);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

        // Inicializar Page Objects
        _homePage = new CorreiosHomePage(_driver);
        _buscaCepPage = new BuscaCepPage(_driver);
        _rastreamentoPage = new RastreamentoPage(_driver);
    }

    /// <summary>
    /// Teste 1: Buscar CEP inválido e validar mensagens de erro
    /// </summary>
    [Fact]
    [TestPriority(1)]
    public void Teste01_BuscarCepInvalido_DeveRetornarMensagemDeErro()
    {
        // Arrange
        const string cepInvalido = "80700000";

        // Act
        _buscaCepPage.Navegar();
        _homePage.AceitarCookiesSeExistir();
        _buscaCepPage.PreencherCep(cepInvalido);
        _buscaCepPage.FocarCampoCaptcha();
        _buscaCepPage.AguardarCaptchaEBuscar(tempoEsperaSegundos: 8);

        // Assert
        _buscaCepPage.MensagemCepNaoEncontradoVisivel().Should().BeTrue(
            "a mensagem 'Não há dados a serem exibidos' deve aparecer para CEP inválido");
        
        var mensagem = _buscaCepPage.ObterMensagemSemDados();
        mensagem.Should().Contain("Não há dados a serem exibidos");
    }

    /// <summary>
    /// Teste 2: Buscar CEP válido e validar o retorno
    /// </summary>
    [Fact]
    [TestPriority(2)]
    public void Teste02_BuscarCepValido_DeveRetornarEnderecoCorreto()
    {
        // Arrange
        const string cepValido = "01013001";
        const string logradouroEsperado = "Rua Quinze de Novembro";
        const string cidadeEstadoEsperado = "São Paulo/SP";

        // Act
        _buscaCepPage.Navegar();
        _homePage.AceitarCookiesSeExistir();
        _buscaCepPage.PreencherCep(cepValido);
        _buscaCepPage.FocarCampoCaptcha();
        _buscaCepPage.AguardarCaptchaEBuscar(tempoEsperaSegundos: 8);

        // Assert
        _buscaCepPage.ResultadoContemLogradouro(logradouroEsperado).Should().BeTrue(
            $"o resultado deve conter o logradouro '{logradouroEsperado}'");
        
        _buscaCepPage.ResultadoContemCidadeEstado(cidadeEstadoEsperado).Should().BeTrue(
            $"o resultado deve conter a cidade/estado '{cidadeEstadoEsperado}'");
    }

    /// <summary>
    /// Teste 3: Buscar rastreamento inválido e validar mensagem de não encontrado
    /// </summary>
    [Fact]
    [TestPriority(3)]
    public void Teste03_BuscarRastreamentoInvalido_DeveRetornarMensagemDeErro()
    {
        // Arrange
        const string codigoInvalido = "SS987654321BR";

        // Act
        _rastreamentoPage.Navegar();
        _homePage.AceitarCookiesSeExistir();
        _rastreamentoPage.PreencherCodigoRastreio(codigoInvalido);
        _rastreamentoPage.AguardarCaptchaEConsultar(tempoEsperaSegundos: 8);

        // Assert
        _rastreamentoPage.MensagemObjetoNaoEncontradoVisivel().Should().BeTrue(
            "a mensagem 'Objeto não encontrado na base' deve aparecer para código inválido");
        
        var mensagem = _rastreamentoPage.ObterMensagemObjetoNaoEncontrado();
        mensagem.Should().Contain("Objeto não encontrado na base");
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
