using FluentAssertions;
using OpenQA.Selenium;
using SpecFlowTests.PageObjects;
using TechTalk.SpecFlow;
using System.Globalization;
using System.Text;

namespace SpecFlowTests.StepDefinitions;

/// <summary>
/// Step Definitions para os cenarios de teste dos Correios
/// Demonstra uso de seletores: ID, XPath e CSS
/// </summary>
[Binding]
public class CorreiosSteps
{
    /// <summary>
    /// Remove acentos de uma string para comparacao
    /// </summary>
    private static string RemoverAcentos(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;
        
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        
        foreach (var c in normalizado)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        
        return sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
    }
    private readonly ScenarioContext _scenarioContext;
    private IWebDriver _driver = null!;
    private CorreiosHomePage _homePage = null!;
    private BuscaCepPage _buscaCepPage = null!;
    private RastreamentoPage _rastreamentoPage = null!;

    public CorreiosSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private void InitializePageObjects()
    {
        _driver = _scenarioContext.Get<IWebDriver>("WebDriver");
        _homePage = new CorreiosHomePage(_driver);
        _buscaCepPage = new BuscaCepPage(_driver);
        _rastreamentoPage = new RastreamentoPage(_driver);
    }

    // =============================================
    // CONTEXTO
    // =============================================

    [Given(@"que estou no site dos Correios")]
    public void DadoQueEstouNoSiteDosCorreios()
    {
        InitializePageObjects();
        _homePage.Navegar();
    }

    [Given(@"aceito os cookies se existirem")]
    public void DadoAceitoOsCookiesSeExistirem()
    {
        _homePage.AceitarCookiesSeExistir();
    }

    // =============================================
    // NAVEGACAO
    // =============================================

    [When(@"eu navego para a pagina de busca de CEP")]
    public void QuandoEuNavegoParaAPaginaDeBuscaDeCep()
    {
        _buscaCepPage.Navegar();
        _homePage.AceitarCookiesSeExistir();
    }

    [When(@"eu navego para a pagina de rastreamento")]
    public void QuandoEuNavegoParaAPaginaDeRastreamento()
    {
        _rastreamentoPage.Navegar();
        _homePage.AceitarCookiesSeExistir();
    }

    // =============================================
    // PREENCHIMENTO DE CAMPOS
    // =============================================

    [When(@"preencho o campo de CEP com ""(.*)"" usando seletor ID")]
    public void QuandoPreenchoOCampoDeCepUsandoSeletorId(string cep)
    {
        Console.WriteLine("\n[DEMONSTRACAO] Usando seletor por ID: By.Id(\"endereco\")");
        _buscaCepPage.PreencherCepPorId(cep);
    }

    [When(@"preencho o campo de codigo com ""(.*)"" usando seletor CSS")]
    public void QuandoPreenchoOCampoDeCodigoUsandoSeletorCss(string codigo)
    {
        Console.WriteLine("\n[DEMONSTRACAO] Usando seletor por CSS: By.CssSelector(\"input[type='text']\")");
        _rastreamentoPage.PreencherCodigoRastreioPorCss(codigo);
    }

    // =============================================
    // CAPTCHA
    // =============================================

    [When(@"preencho o captcha manualmente")]
    public void QuandoPreenchoOCaptchaManualmente()
    {
        // O captcha e preenchido durante o metodo AguardarCaptchaE...
        // Este step apenas foca no campo
        if (_scenarioContext.ScenarioInfo.Title.Contains("rastreamento"))
        {
            // Para rastreamento, o foco e feito automaticamente
        }
        else
        {
            _buscaCepPage.FocarCampoCaptchaPorId();
        }
    }

    // =============================================
    // BOTOES
    // =============================================

    [When(@"clico no botao Buscar")]
    public void QuandoClicoNoBotaoBuscar()
    {
        Console.WriteLine("\n[DEMONSTRACAO] Clicando botao com seletor por ID: By.Id(\"btn_pesquisar\")");
        _buscaCepPage.AguardarCaptchaEBuscar(tempoEsperaSegundos: 8);
    }

    [When(@"clico no botao Consultar")]
    public void QuandoClicoNoBotaoConsultar()
    {
        Console.WriteLine("\n[DEMONSTRACAO] Clicando botao com seletor por XPath");
        _rastreamentoPage.AguardarCaptchaEConsultar(tempoEsperaSegundos: 8);
    }

    [Then(@"clico no botao OK para fechar o modal")]
    public void EntaoClicoNoBotaoOkParaFecharOModal()
    {
        Console.WriteLine("\n[DEMONSTRACAO] Clicando botao OK com seletor por XPath");
        _rastreamentoPage.ClicarBotaoOkPorXPath();
    }

    // =============================================
    // VALIDACOES - XPATH
    // =============================================

    [Then(@"devo ver a mensagem ""(.*)""")]
    public void EntaoDevoverAMensagem(string mensagemEsperada)
    {
        Console.WriteLine($"\n[DEMONSTRACAO] Verificando mensagem com seletor XPath");
        
        if (mensagemEsperada.ToLower().Contains("objeto"))
        {
            _rastreamentoPage.MensagemObjetoNaoEncontradoVisivelPorXPath()
                .Should().BeTrue($"a mensagem '{mensagemEsperada}' deve estar visivel");
            
            var mensagem = _rastreamentoPage.ObterMensagemObjetoNaoEncontradoPorXPath();
            RemoverAcentos(mensagem).Should().Contain(RemoverAcentos(mensagemEsperada));
        }
        else
        {
            _buscaCepPage.MensagemSemDadosVisivelPorXPath()
                .Should().BeTrue($"a mensagem '{mensagemEsperada}' deve estar visivel");
            
            var mensagem = _buscaCepPage.ObterMensagemSemDadosPorXPath();
            RemoverAcentos(mensagem).Should().Contain(RemoverAcentos(mensagemEsperada));
        }
    }

    [Then(@"devo ver o titulo ""(.*)""")]
    public void EntaoDevoverOTitulo(string tituloEsperado)
    {
        Console.WriteLine($"\n[DEMONSTRACAO] Verificando titulo com seletor XPath");
        _buscaCepPage.HeadingDadosNaoEncontradoVisivelPorXPath()
            .Should().BeTrue($"o titulo '{tituloEsperado}' deve estar visivel");
    }

    [Then(@"o resultado deve conter ""(.*)"" usando seletor XPath")]
    public void EntaoOResultadoDeveConterUsandoSeletorXPath(string textoEsperado)
    {
        Console.WriteLine($"\n[DEMONSTRACAO] Verificando resultado com seletor XPath: //td[contains(text(), '{textoEsperado}')]");
        
        if (textoEsperado.Contains("Paulo"))
        {
            // E cidade/estado
            _buscaCepPage.ResultadoContemCidadeEstadoPorXPath(textoEsperado)
                .Should().BeTrue($"o resultado deve conter '{textoEsperado}'");
        }
        else
        {
            // E logradouro
            _buscaCepPage.ResultadoContemLogradouroPorXPath(textoEsperado)
                .Should().BeTrue($"o resultado deve conter '{textoEsperado}'");
        }
    }
}
