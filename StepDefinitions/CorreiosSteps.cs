using FluentAssertions;
using OpenQA.Selenium;
using technicaltest_b3.PageObjects;
using TechTalk.SpecFlow;

namespace technicaltest_b3.StepDefinitions;

/// <summary>
/// Step Definitions para os cenários de teste dos Correios
/// </summary>
[Binding]
public class CorreiosSteps
{
    private readonly ScenarioContext _scenarioContext;
    private IWebDriver Driver => _scenarioContext.Get<IWebDriver>("WebDriver");
    private CorreiosHomePage HomePage => new(Driver);
    private BuscaCepPage BuscaCepPage => new(Driver);
    private RastreamentoPage RastreamentoPage => new(Driver);

    public CorreiosSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    #region Given Steps

    [Given(@"que eu estou no site dos Correios")]
    public void DadoQueEuEstouNoSiteDosCorreios()
    {
        HomePage.Navegar();
    }

    [Given(@"eu aceito os cookies se aparecerem")]
    public void DadoEuAceitoOsCookiesSeAparecerem()
    {
        HomePage.AceitarCookiesSeExistir();
    }

    [Given(@"que eu volto para a página inicial")]
    public void DadoQueEuVoltoParaAPaginaInicial()
    {
        HomePage.Navegar();
        Thread.Sleep(1500);
    }

    #endregion

    #region When Steps

    [When(@"eu navego para a página de busca de CEP")]
    public void QuandoEuNavegoParaAPaginaDeBuscaDeCep()
    {
        BuscaCepPage.Navegar();
    }

    [When(@"eu preencho o CEP ""(.*)""")]
    public void QuandoEuPreenchoOCep(string cep)
    {
        BuscaCepPage.PreencherCep(cep);
        _scenarioContext["CEP"] = cep;
    }

    [When(@"eu preencho o captcha manualmente")]
    public void QuandoEuPreenchoOCaptchaManualmente()
    {
        // Apenas aguarda - o captcha deve ser preenchido manualmente
        BuscaCepPage.FocarCampoCaptcha();
    }

    [When(@"eu clico no botão de buscar")]
    public void QuandoEuClicoNoBotaoDeBuscar()
    {
        BuscaCepPage.AguardarCaptchaEBuscar();
    }

    [When(@"eu navego para a página de rastreamento")]
    public void QuandoEuNavegoParaAPaginaDeRastreamento()
    {
        RastreamentoPage.Navegar();
    }

    [When(@"eu preencho o código de rastreio ""(.*)""")]
    public void QuandoEuPreenchoOCodigoDeRastreio(string codigo)
    {
        RastreamentoPage.PreencherCodigoRastreio(codigo);
        _scenarioContext["CodigoRastreio"] = codigo;
    }

    [When(@"eu clico no botão de consultar")]
    public void QuandoEuClicoNoBotaoDeConsultar()
    {
        RastreamentoPage.AguardarCaptchaEConsultar();
    }

    #endregion

    #region Then Steps

    [Then(@"devo ver a mensagem ""(.*)""")]
    public void EntaoDevVerAMensagem(string mensagemEsperada)
    {
        // Verificar se é mensagem de CEP ou rastreamento
        if (mensagemEsperada.Contains("Não há dados"))
        {
            var mensagem = BuscaCepPage.ObterMensagemSemDados();
            mensagem.Should().Contain(mensagemEsperada);
        }
        else if (mensagemEsperada.Contains("Objeto não encontrado"))
        {
            RastreamentoPage.MensagemObjetoNaoEncontradoVisivel().Should().BeTrue();
            var mensagem = RastreamentoPage.ObterMensagemObjetoNaoEncontrado();
            mensagem.Should().Contain(mensagemEsperada);
            RastreamentoPage.ClicarOk();
        }
    }

    [Then(@"devo ver o título ""(.*)""")]
    public void EntaoDevVerOTitulo(string tituloEsperado)
    {
        BuscaCepPage.MensagemCepNaoEncontradoVisivel().Should().BeTrue(
            $"o título '{tituloEsperado}' deve estar visível");
    }

    [Then(@"devo ver o logradouro ""(.*)""")]
    public void EntaoDevVerOLogradouro(string logradouroEsperado)
    {
        BuscaCepPage.ResultadoContemLogradouro(logradouroEsperado).Should().BeTrue(
            $"o logradouro '{logradouroEsperado}' deve estar visível na tabela");
        
        var logradouro = BuscaCepPage.ObterLogradouro();
        logradouro.Should().Contain(logradouroEsperado);
    }

    [Then(@"devo ver a cidade/estado ""(.*)""")]
    public void EntaoDevVerACidadeEstado(string cidadeEstadoEsperado)
    {
        BuscaCepPage.ResultadoContemCidadeEstado(cidadeEstadoEsperado).Should().BeTrue(
            $"a cidade/estado '{cidadeEstadoEsperado}' deve estar visível na tabela");
        
        var cidadeEstado = BuscaCepPage.ObterCidadeEstado();
        cidadeEstado.Should().Be(cidadeEstadoEsperado);
    }

    [Then(@"o resultado deve ser ""(.*)""")]
    public void EntaoOResultadoDeveSer(string resultadoEsperado)
    {
        if (resultadoEsperado.Contains("Não há dados"))
        {
            BuscaCepPage.MensagemCepNaoEncontradoVisivel().Should().BeTrue(
                "a mensagem de dados não encontrados deve estar visível");
        }
        else
        {
            // Resultado esperado no formato "Logradouro - Cidade/Estado"
            var partes = resultadoEsperado.Split(" - ");
            if (partes.Length == 2)
            {
                var logradouro = partes[0];
                var cidadeEstado = partes[1];

                BuscaCepPage.ResultadoContemLogradouro(logradouro).Should().BeTrue(
                    $"o logradouro '{logradouro}' deve estar visível");
                BuscaCepPage.ResultadoContemCidadeEstado(cidadeEstado).Should().BeTrue(
                    $"a cidade/estado '{cidadeEstado}' deve estar visível");
            }
        }
    }

    #endregion
}
