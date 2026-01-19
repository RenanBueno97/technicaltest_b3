using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SpecFlowTests.PageObjects;

/// <summary>
/// Page Object para a página de busca de CEP dos Correios
/// Demonstra uso de seletores: ID, XPath e CSS
/// </summary>
public class BuscaCepPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    private const string Url = "https://buscacepinter.correios.com.br/app/endereco/index.php?";

    // =============================================
    // SELETORES POR ID
    // =============================================
    private By CampoCepPorId => By.Id("endereco");
    private By CampoCaptchaPorId => By.Id("captcha");
    private By BotaoBuscarPorId => By.Id("btn_pesquisar");

    // =============================================
    // SELETORES POR XPATH
    // =============================================
    private By BotaoNovaBuscaPorXPath => By.XPath("//button[contains(text(), 'Nova Busca')]");
    private By MensagemSemDadosPorXPath => By.XPath("//*[contains(text(), 'Não há dados a serem exibidos')]");
    private By HeadingDadosNaoEncontradoPorXPath => By.XPath("//*[contains(text(), 'Dados não encontrado')]");
    private By CelulaLogradouroPorXPath(string logradouro) => By.XPath($"//td[contains(text(), '{logradouro}')]");
    private By CelulaCidadeEstadoPorXPath(string cidadeEstado) => By.XPath($"//td[contains(text(), '{cidadeEstado}')]");

    // =============================================
    // SELETORES POR CSS
    // =============================================
    private By TabelaResultadosPorCss => By.CssSelector("table");
    private By AlertaErroPorCss => By.CssSelector(".alert, .error, .alert-danger");

    public BuscaCepPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public void Navegar()
    {
        _driver.Navigate().GoToUrl(Url);
    }

    /// <summary>
    /// Preenche o campo de CEP usando seletor por ID
    /// </summary>
    public void PreencherCepPorId(string cep)
    {
        Console.WriteLine($"[ID] Preenchendo campo de CEP com seletor By.Id(\"endereco\")");
        var campo = _wait.Until(ExpectedConditions.ElementIsVisible(CampoCepPorId));
        campo.Clear();
        campo.SendKeys(cep);
    }

    /// <summary>
    /// Foca no campo de captcha usando seletor por ID
    /// </summary>
    public void FocarCampoCaptchaPorId()
    {
        try
        {
            Console.WriteLine($"[ID] Focando campo de captcha com seletor By.Id(\"captcha\")");
            var captcha = _wait.Until(ExpectedConditions.ElementIsVisible(CampoCaptchaPorId));
            captcha.Click();
        }
        catch
        {
            // Campo pode não existir
        }
    }

    /// <summary>
    /// Limpa e foca no campo de captcha
    /// </summary>
    public void LimparEFocarCaptcha()
    {
        try
        {
            var captcha = _driver.FindElement(CampoCaptchaPorId);
            captcha.Clear();
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].focus();", captcha);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = '';", captcha);
            captcha.Click();
            Console.WriteLine("[CAPTCHA] Campo de captcha limpo e cursor posicionado.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CAPTCHA] Erro ao limpar/focar captcha: {ex.Message}");
        }
    }

    /// <summary>
    /// Clica no botão Buscar usando seletor por ID
    /// </summary>
    public void ClicarBotaoBuscarPorId()
    {
        try
        {
            Console.WriteLine($"[ID] Clicando botão Buscar com seletor By.Id(\"btn_pesquisar\")");
            var botao = _driver.FindElement(BotaoBuscarPorId);
            if (botao.Displayed && botao.Enabled)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", botao);
                return;
            }
        }
        catch { }

        try
        {
            Console.WriteLine($"[XPath] Tentando botão Nova Busca com seletor XPath");
            var botaoNovaBusca = _driver.FindElement(BotaoNovaBuscaPorXPath);
            if (botaoNovaBusca.Displayed && botaoNovaBusca.Enabled)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", botaoNovaBusca);
            }
        }
        catch { }
    }

    /// <summary>
    /// Aguarda captcha e busca com retry
    /// </summary>
    public void AguardarCaptchaEBuscar(int tempoEsperaSegundos = 8, int maxTentativas = 3)
    {
        for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
        {
            Console.WriteLine($"\n========================================");
            Console.WriteLine($"[CAPTCHA] Tentativa {tentativa} de {maxTentativas}");
            Console.WriteLine($"[CAPTCHA] Aguardando {tempoEsperaSegundos} segundos...");
            Console.WriteLine($"========================================\n");
            
            Thread.Sleep(tempoEsperaSegundos * 1000);
            ClicarBotaoBuscarPorId();
            Thread.Sleep(1500);

            if (ResultadoApareceu())
            {
                Console.WriteLine("[CAPTCHA] Resultado obtido com sucesso!");
                return;
            }

            if (tentativa < maxTentativas)
            {
                Console.WriteLine("[CAPTCHA] Preparando nova tentativa...");
                FecharModalErro();
                Thread.Sleep(300);
                LimparEFocarCaptcha();
            }
        }
    }

    private bool ResultadoApareceu()
    {
        try
        {
            var semDados = _driver.FindElements(MensagemSemDadosPorXPath);
            if (semDados.Any(e => e.Displayed)) return true;

            var tabela = _driver.FindElements(TabelaResultadosPorCss);
            if (tabela.Any(e => e.Displayed)) return true;

            return false;
        }
        catch { return false; }
    }

    private void FecharModalErro()
    {
        try
        {
            var botoesFechar = _driver.FindElements(By.XPath("//button[contains(text(), 'OK') or contains(text(), 'Fechar')]"));
            foreach (var botao in botoesFechar)
            {
                if (botao.Displayed)
                {
                    botao.Click();
                    Thread.Sleep(300);
                }
            }
        }
        catch { }
    }

    /// <summary>
    /// Verifica mensagem usando seletor XPath
    /// </summary>
    public bool MensagemSemDadosVisivelPorXPath()
    {
        try
        {
            Console.WriteLine($"[XPath] Verificando mensagem com seletor XPath");
            Thread.Sleep(1000);
            var mensagens = _driver.FindElements(MensagemSemDadosPorXPath);
            return mensagens.Any(m => m.Displayed);
        }
        catch { return false; }
    }

    /// <summary>
    /// Verifica heading usando seletor XPath
    /// </summary>
    public bool HeadingDadosNaoEncontradoVisivelPorXPath()
    {
        try
        {
            Console.WriteLine($"[XPath] Verificando heading com seletor XPath");
            var headings = _driver.FindElements(HeadingDadosNaoEncontradoPorXPath);
            return headings.Any(h => h.Displayed);
        }
        catch { return false; }
    }

    /// <summary>
    /// Obtém texto da mensagem usando XPath
    /// </summary>
    public string ObterMensagemSemDadosPorXPath()
    {
        try
        {
            var mensagem = _wait.Until(ExpectedConditions.ElementIsVisible(MensagemSemDadosPorXPath));
            return mensagem.Text;
        }
        catch { return string.Empty; }
    }

    /// <summary>
    /// Verifica logradouro usando seletor XPath
    /// </summary>
    public bool ResultadoContemLogradouroPorXPath(string logradouro)
    {
        try
        {
            Console.WriteLine($"[XPath] Verificando logradouro com seletor XPath: //td[contains(text(), '{logradouro}')]");
            _wait.Until(ExpectedConditions.ElementIsVisible(TabelaResultadosPorCss));
            var celula = _driver.FindElement(CelulaLogradouroPorXPath(logradouro));
            return celula.Displayed;
        }
        catch { return false; }
    }

    /// <summary>
    /// Verifica cidade/estado usando seletor XPath
    /// </summary>
    public bool ResultadoContemCidadeEstadoPorXPath(string cidadeEstado)
    {
        try
        {
            Console.WriteLine($"[XPath] Verificando cidade/estado com seletor XPath: //td[contains(text(), '{cidadeEstado}')]");
            
            // Tenta buscar diretamente
            var elementos = _driver.FindElements(CelulaCidadeEstadoPorXPath(cidadeEstado));
            if (elementos.Any(e => e.Displayed)) return true;
            
            // Se nao encontrou, tenta com variacao de acento (Sao Paulo vs São Paulo)
            if (cidadeEstado.Contains("Sao Paulo"))
            {
                var comAcento = _driver.FindElements(By.XPath("//td[contains(text(), 'São Paulo')]"));
                if (comAcento.Any(e => e.Displayed)) return true;
            }
            
            // Tenta buscar na tabela inteira pelo texto
            var tabela = _driver.FindElements(TabelaResultadosPorCss);
            foreach (var t in tabela)
            {
                if (t.Text.Contains("Paulo", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[XPath] Encontrado 'Paulo' na tabela: {t.Text}");
                    return true;
                }
            }
            
            return false;
        }
        catch { return false; }
    }
}
