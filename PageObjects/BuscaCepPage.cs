using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace technicaltest_b3.PageObjects;

/// <summary>
/// Page Object para a página de busca de CEP dos Correios
/// </summary>
public class BuscaCepPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private const string Url = "https://buscacepinter.correios.com.br/app/endereco/index.php?";

    // Locators
    private By CampoCepOuEndereco => By.Id("endereco");
    private By CampoCaptcha => By.Id("captcha");
    private By BotaoBuscar => By.Id("btn_pesquisar");
    private By BotaoNovaBusca => By.XPath("//button[contains(text(), 'Nova Busca')]");
    private By MensagemSemDados => By.XPath("//*[contains(text(), 'Não há dados a serem exibidos')]");
    private By HeadingDadosNaoEncontrado => By.XPath("//h2[contains(text(), 'Dados não encontrado')]");
    private By TabelaResultados => By.TagName("table");

    public BuscaCepPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    /// <summary>
    /// Navega para a página de busca de CEP
    /// </summary>
    public void Navegar()
    {
        _driver.Navigate().GoToUrl(Url);
    }

    /// <summary>
    /// Preenche o campo de CEP ou endereço
    /// </summary>
    public void PreencherCep(string cep)
    {
        var campo = _wait.Until(ExpectedConditions.ElementIsVisible(CampoCepOuEndereco));
        campo.Clear();
        campo.SendKeys(cep);
    }

    /// <summary>
    /// Foca no campo de captcha para o usuário preencher manualmente
    /// </summary>
    public void FocarCampoCaptcha()
    {
        try
        {
            var captcha = _wait.Until(ExpectedConditions.ElementIsVisible(CampoCaptcha));
            captcha.Click();
        }
        catch
        {
            // Campo pode não existir em alguns casos
        }
    }

    /// <summary>
    /// Aguarda o tempo para o usuário preencher o captcha e clica em buscar
    /// </summary>
    /// <param name="tempoEsperaSegundos">Tempo de espera para preencher o captcha</param>
    public void AguardarCaptchaEBuscar(int tempoEsperaSegundos = 10)
    {
        // Aguardar o usuário preencher o captcha
        Thread.Sleep(tempoEsperaSegundos * 1000);

        try
        {
            var botao = _wait.Until(ExpectedConditions.ElementToBeClickable(BotaoBuscar));
            botao.Click();
        }
        catch
        {
            // Tentar botão "Nova Busca" se "Buscar" não existir
            var botaoNovaBusca = _wait.Until(ExpectedConditions.ElementToBeClickable(BotaoNovaBusca));
            botaoNovaBusca.Click();
        }

        // Aguardar processamento
        Thread.Sleep(2000);
    }

    /// <summary>
    /// Verifica se a mensagem de CEP não encontrado está visível
    /// </summary>
    public bool MensagemCepNaoEncontradoVisivel()
    {
        try
        {
            var mensagem = _wait.Until(ExpectedConditions.ElementIsVisible(MensagemSemDados));
            var heading = _wait.Until(ExpectedConditions.ElementIsVisible(HeadingDadosNaoEncontrado));
            return mensagem.Displayed && heading.Displayed;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtém o texto da mensagem de dados não encontrados
    /// </summary>
    public string ObterMensagemSemDados()
    {
        try
        {
            var mensagem = _wait.Until(ExpectedConditions.ElementIsVisible(MensagemSemDados));
            return mensagem.Text;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Verifica se a tabela de resultados contém o logradouro esperado
    /// </summary>
    public bool ResultadoContemLogradouro(string logradouro)
    {
        try
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(TabelaResultados));
            var celula = _driver.FindElement(By.XPath($"//td[contains(text(), '{logradouro}')]"));
            return celula.Displayed;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Verifica se a tabela de resultados contém a cidade/estado esperados
    /// </summary>
    public bool ResultadoContemCidadeEstado(string cidadeEstado)
    {
        try
        {
            var celula = _driver.FindElement(By.XPath($"//td[contains(text(), '{cidadeEstado}')]"));
            return celula.Displayed;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtém o texto do logradouro da tabela de resultados
    /// </summary>
    public string ObterLogradouro()
    {
        try
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(TabelaResultados));
            var celula = _driver.FindElement(By.XPath("//tbody/tr[1]/td[1]"));
            return celula.Text;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Obtém o texto da cidade/estado da tabela de resultados
    /// </summary>
    public string ObterCidadeEstado()
    {
        try
        {
            var celula = _driver.FindElement(By.XPath("//tbody/tr[1]/td[3]"));
            return celula.Text;
        }
        catch
        {
            return string.Empty;
        }
    }
}
