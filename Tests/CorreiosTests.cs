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
        // Executar em modo visível para permitir preenchimento manual do captcha
        // options.AddArgument("--headless"); // Comentado para ver o navegador
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
    /// Teste: Validar que CEP 80700000 não existe
    /// </summary>
    [Fact]
    [Trait("Category", "CEP")]
    public void DeveRetornarMensagemDeErroParaCepInvalido()
    {
        // Arrange
        const string cepInvalido = "80700000";

        // Act
        _homePage.Navegar();
        _homePage.AceitarCookiesSeExistir();
        
        _buscaCepPage.Navegar();
        _buscaCepPage.PreencherCep(cepInvalido);
        _buscaCepPage.FocarCampoCaptcha();
        _buscaCepPage.AguardarCaptchaEBuscar();

        // Assert
        _buscaCepPage.MensagemCepNaoEncontradoVisivel().Should().BeTrue(
            "a mensagem 'Não há dados a serem exibidos' deve aparecer para CEP inválido");
        
        var mensagem = _buscaCepPage.ObterMensagemSemDados();
        mensagem.Should().Contain("Não há dados a serem exibidos");
    }

    /// <summary>
    /// Teste: Validar que CEP 01013001 retorna "Rua Quinze de Novembro, São Paulo/SP"
    /// </summary>
    [Fact]
    [Trait("Category", "CEP")]
    public void DeveRetornarEnderecoCorretoParaCepValido()
    {
        // Arrange
        const string cepValido = "01013001";
        const string logradouroEsperado = "Rua Quinze de Novembro";
        const string cidadeEstadoEsperado = "São Paulo/SP";

        // Act
        _homePage.Navegar();
        _homePage.AceitarCookiesSeExistir();
        
        _buscaCepPage.Navegar();
        _buscaCepPage.PreencherCep(cepValido);
        _buscaCepPage.FocarCampoCaptcha();
        _buscaCepPage.AguardarCaptchaEBuscar();

        // Assert
        _buscaCepPage.ResultadoContemLogradouro(logradouroEsperado).Should().BeTrue(
            $"o resultado deve conter o logradouro '{logradouroEsperado}'");
        
        _buscaCepPage.ResultadoContemCidadeEstado(cidadeEstadoEsperado).Should().BeTrue(
            $"o resultado deve conter a cidade/estado '{cidadeEstadoEsperado}'");
        
        var logradouro = _buscaCepPage.ObterLogradouro();
        logradouro.Should().Contain(logradouroEsperado);
        
        var cidadeEstado = _buscaCepPage.ObterCidadeEstado();
        cidadeEstado.Should().Be(cidadeEstadoEsperado);
    }

    /// <summary>
    /// Teste: Validar que código SS987654321BR não foi encontrado
    /// </summary>
    [Fact]
    [Trait("Category", "Rastreamento")]
    public void DeveRetornarMensagemDeErroParaCodigoRastreioInvalido()
    {
        // Arrange
        const string codigoInvalido = "SS987654321BR";

        // Act
        _homePage.Navegar();
        _homePage.AceitarCookiesSeExistir();
        
        _rastreamentoPage.Navegar();
        _rastreamentoPage.PreencherCodigoRastreio(codigoInvalido);
        _rastreamentoPage.AguardarCaptchaEConsultar();

        // Assert
        _rastreamentoPage.MensagemObjetoNaoEncontradoVisivel().Should().BeTrue(
            "a mensagem 'Objeto não encontrado na base' deve aparecer para código inválido");
        
        var mensagem = _rastreamentoPage.ObterMensagemObjetoNaoEncontrado();
        mensagem.Should().Contain("Objeto não encontrado na base");
        
        _rastreamentoPage.ClicarOk();
    }

    /// <summary>
    /// Teste completo do fluxo: CEP inválido, CEP válido e Rastreamento
    /// (Equivalente ao teste Playwright original)
    /// </summary>
    [Fact]
    [Trait("Category", "FluxoCompleto")]
    public void FluxoCompleto_CepInvalido_CepValido_Rastreamento()
    {
        // 1. Entrar no site dos correios
        _homePage.Navegar();
        _homePage.AceitarCookiesSeExistir();

        // 2. Procurar pelo CEP 80700000 (inválido)
        _buscaCepPage.Navegar();
        _buscaCepPage.PreencherCep("80700000");
        _buscaCepPage.FocarCampoCaptcha();
        _buscaCepPage.AguardarCaptchaEBuscar();

        // 3. Confirmar que o CEP não existe
        _buscaCepPage.MensagemCepNaoEncontradoVisivel().Should().BeTrue(
            "a mensagem de CEP não encontrado deve aparecer");

        // 4. Voltar à tela inicial
        _homePage.Navegar();

        // 5. Procurar pelo CEP 01013-001 (válido)
        _buscaCepPage.Navegar();
        _buscaCepPage.PreencherCep("01013001");
        _buscaCepPage.FocarCampoCaptcha();
        _buscaCepPage.AguardarCaptchaEBuscar();

        // 6. Confirmar que o resultado seja em "Rua Quinze de Novembro, São Paulo/SP"
        _buscaCepPage.ResultadoContemLogradouro("Rua Quinze de Novembro").Should().BeTrue();
        _buscaCepPage.ResultadoContemCidadeEstado("São Paulo/SP").Should().BeTrue();

        // 7. Voltar à tela inicial
        _homePage.Navegar();

        // 8. Procurar no rastreamento de código o número "SS987654321BR"
        _rastreamentoPage.Navegar();
        _rastreamentoPage.PreencherCodigoRastreio("SS987654321BR");
        _rastreamentoPage.AguardarCaptchaEConsultar();

        // 9. Confirmar que o código não está correto
        _rastreamentoPage.MensagemObjetoNaoEncontradoVisivel().Should().BeTrue(
            "a mensagem de objeto não encontrado deve aparecer");
        
        _rastreamentoPage.ClicarOk();

        // 10. Fechar o browser (automático pelo Dispose)
    }

    public void Dispose()
    {
        _driver?.Quit();
        _driver?.Dispose();
    }
}
