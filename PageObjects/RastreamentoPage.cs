using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace technicaltest_b3.PageObjects;

/// <summary>
/// Page Object para a página de rastreamento dos Correios
/// </summary>
public class RastreamentoPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private const string Url = "https://rastreamento.correios.com.br/app/index.php";

    // Locators
    private By CampoCodigoRastreio => By.CssSelector("input[type='text']");
    private By BotaoConsultar => By.XPath("//button[contains(text(), 'Consultar')]");
    private By MensagemObjetoNaoEncontrado => By.XPath("//*[contains(text(), 'Objeto não encontrado na base')]");
    private By BotaoOk => By.XPath("//button[contains(text(), 'OK') or text()='OK']");

    public RastreamentoPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    /// <summary>
    /// Navega para a página de rastreamento
    /// </summary>
    public void Navegar()
    {
        _driver.Navigate().GoToUrl(Url);
        // Aguardar carregamento completo
        Thread.Sleep(2000);
    }

    /// <summary>
    /// Preenche o código de rastreio
    /// </summary>
    public void PreencherCodigoRastreio(string codigo)
    {
        var campo = _wait.Until(ExpectedConditions.ElementIsVisible(CampoCodigoRastreio));
        campo.Clear();
        campo.Click();
        campo.SendKeys(codigo);
        campo.SendKeys(Keys.Tab);
    }

    /// <summary>
    /// Aguarda o tempo para o usuário preencher o captcha e clica em consultar
    /// </summary>
    /// <param name="tempoEsperaSegundos">Tempo de espera para preencher o captcha</param>
    public void AguardarCaptchaEConsultar(int tempoEsperaSegundos = 10)
    {
        // Aguardar o usuário preencher o captcha
        Thread.Sleep(tempoEsperaSegundos * 1000);

        var botao = _wait.Until(ExpectedConditions.ElementToBeClickable(BotaoConsultar));
        botao.Click();

        // Aguardar processamento
        Thread.Sleep(2000);
    }

    /// <summary>
    /// Verifica se a mensagem de objeto não encontrado está visível
    /// </summary>
    public bool MensagemObjetoNaoEncontradoVisivel()
    {
        try
        {
            var mensagem = _wait.Until(ExpectedConditions.ElementIsVisible(MensagemObjetoNaoEncontrado));
            return mensagem.Displayed;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtém o texto da mensagem de objeto não encontrado
    /// </summary>
    public string ObterMensagemObjetoNaoEncontrado()
    {
        try
        {
            var mensagem = _wait.Until(ExpectedConditions.ElementIsVisible(MensagemObjetoNaoEncontrado));
            return mensagem.Text;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Clica no botão OK para fechar o modal de erro
    /// </summary>
    public void ClicarOk()
    {
        try
        {
            var botao = _wait.Until(ExpectedConditions.ElementToBeClickable(BotaoOk));
            botao.Click();
        }
        catch
        {
            // Modal pode não existir
        }
    }
}
