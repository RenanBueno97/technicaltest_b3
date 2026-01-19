using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SpecFlowTests.PageObjects;

/// <summary>
/// Page Object para a página de rastreamento dos Correios
/// Demonstra uso de seletores: ID, XPath e CSS
/// </summary>
public class RastreamentoPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private const string Url = "https://rastreamento.correios.com.br/app/index.php";

    // =============================================
    // SELETORES POR CSS
    // =============================================
    private By CampoCodigoRastreioPorCss => By.CssSelector("input[type='text']");
    private By ResultadosPorCss => By.CssSelector(".resultado, .timeline, .evento");

    // =============================================
    // SELETORES POR XPATH
    // =============================================
    private By BotaoConsultarPorXPath => By.XPath("//button[contains(text(), 'Consultar')]");
    private By MensagemObjetoNaoEncontradoPorXPath => By.XPath("//*[contains(text(), 'Objeto não encontrado na base')]");
    private By BotaoOkPorXPath => By.XPath("//button[contains(text(), 'OK') or text()='OK']");
    private By MensagemCaptchaInvalidoPorXPath => By.XPath("//*[contains(text(), 'Captcha inválido') or contains(text(), 'captcha inválido')]");
    private By CampoCaptchaPorXPath => By.XPath("//input[contains(@id, 'captcha') or contains(@name, 'captcha')]");

    public RastreamentoPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public void Navegar()
    {
        _driver.Navigate().GoToUrl(Url);
        Thread.Sleep(1000);
    }

    /// <summary>
    /// Preenche o código de rastreio usando seletor CSS
    /// </summary>
    public void PreencherCodigoRastreioPorCss(string codigo)
    {
        Console.WriteLine($"[CSS] Preenchendo campo com seletor By.CssSelector(\"input[type='text']\")");
        var campo = _wait.Until(ExpectedConditions.ElementIsVisible(CampoCodigoRastreioPorCss));
        campo.Clear();
        campo.Click();
        campo.SendKeys(codigo);
        campo.SendKeys(Keys.Tab);
    }

    /// <summary>
    /// Limpa e foca no campo de captcha
    /// </summary>
    public void LimparEFocarCaptcha()
    {
        try
        {
            IWebElement? campoCaptcha = null;
            
            var captchas = _driver.FindElements(CampoCaptchaPorXPath);
            foreach (var captcha in captchas)
            {
                if (captcha.Displayed)
                {
                    campoCaptcha = captcha;
                    break;
                }
            }
            
            if (campoCaptcha == null)
            {
                var inputs = _driver.FindElements(CampoCodigoRastreioPorCss);
                if (inputs.Count > 1 && inputs[1].Displayed)
                {
                    campoCaptcha = inputs[1];
                }
            }
            
            if (campoCaptcha != null)
            {
                campoCaptcha.Clear();
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].focus();", campoCaptcha);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = '';", campoCaptcha);
                campoCaptcha.Click();
                Console.WriteLine("[CAPTCHA] Campo de captcha limpo e cursor posicionado.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CAPTCHA] Erro ao limpar/focar captcha: {ex.Message}");
        }
    }

    /// <summary>
    /// Clica no botão Consultar usando seletor XPath
    /// </summary>
    public void ClicarBotaoConsultarPorXPath()
    {
        try
        {
            Console.WriteLine($"[XPath] Clicando botão Consultar com seletor XPath");
            var botao = _driver.FindElement(BotaoConsultarPorXPath);
            if (botao.Displayed && botao.Enabled)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", botao);
            }
        }
        catch { }
    }

    /// <summary>
    /// Verifica se captcha está incorreto
    /// </summary>
    public bool CaptchaIncorreto()
    {
        try
        {
            var erros = _driver.FindElements(MensagemCaptchaInvalidoPorXPath);
            if (erros.Any(e => e.Displayed))
            {
                Console.WriteLine("[CAPTCHA] Detectada mensagem: 'Captcha inválido'");
                return true;
            }
            return false;
        }
        catch { return false; }
    }

    /// <summary>
    /// Aguarda captcha e consulta com retry
    /// </summary>
    public void AguardarCaptchaEConsultar(int tempoEsperaSegundos = 8, int maxTentativas = 3)
    {
        for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
        {
            Console.WriteLine($"\n========================================");
            Console.WriteLine($"[CAPTCHA] Tentativa {tentativa} de {maxTentativas}");
            Console.WriteLine($"[CAPTCHA] Aguardando {tempoEsperaSegundos} segundos...");
            Console.WriteLine($"========================================\n");

            Thread.Sleep(tempoEsperaSegundos * 1000);
            ClicarBotaoConsultarPorXPath();
            Thread.Sleep(1500);

            if (CaptchaIncorreto())
            {
                Console.WriteLine("[CAPTCHA] ERRO DETECTADO: Captcha incorreto!");
                if (tentativa < maxTentativas)
                {
                    ClicarBotaoOkPorXPath();
                    Thread.Sleep(300);
                    LimparEFocarCaptcha();
                    continue;
                }
            }

            if (ResultadoApareceu())
            {
                Console.WriteLine("[CAPTCHA] Resultado obtido com sucesso!");
                return;
            }

            Thread.Sleep(1000);
            if (ResultadoApareceu()) return;

            if (tentativa < maxTentativas)
            {
                ClicarBotaoOkPorXPath();
                Thread.Sleep(300);
                LimparEFocarCaptcha();
            }
        }
    }

    private bool ResultadoApareceu()
    {
        try
        {
            var mensagem = _driver.FindElements(MensagemObjetoNaoEncontradoPorXPath);
            if (mensagem.Any(e => e.Displayed)) return true;

            var resultados = _driver.FindElements(ResultadosPorCss);
            if (resultados.Any(e => e.Displayed)) return true;

            return false;
        }
        catch { return false; }
    }

    /// <summary>
    /// Verifica mensagem de objeto não encontrado usando XPath
    /// </summary>
    public bool MensagemObjetoNaoEncontradoVisivelPorXPath()
    {
        try
        {
            Console.WriteLine($"[XPath] Verificando mensagem com seletor XPath");
            var mensagem = _wait.Until(ExpectedConditions.ElementIsVisible(MensagemObjetoNaoEncontradoPorXPath));
            return mensagem.Displayed;
        }
        catch { return false; }
    }

    /// <summary>
    /// Obtém texto da mensagem usando XPath
    /// </summary>
    public string ObterMensagemObjetoNaoEncontradoPorXPath()
    {
        try
        {
            var mensagem = _wait.Until(ExpectedConditions.ElementIsVisible(MensagemObjetoNaoEncontradoPorXPath));
            return mensagem.Text;
        }
        catch { return string.Empty; }
    }

    /// <summary>
    /// Clica no botão OK usando seletor XPath
    /// </summary>
    public void ClicarBotaoOkPorXPath()
    {
        try
        {
            Console.WriteLine($"[XPath] Clicando botão OK com seletor XPath");
            var botoes = _driver.FindElements(BotaoOkPorXPath);
            foreach (var botao in botoes)
            {
                if (botao.Displayed)
                {
                    botao.Click();
                    Thread.Sleep(300);
                    return;
                }
            }
        }
        catch { }
    }
}
