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
    // Campo de captcha - tentar vários seletores possíveis
    private By CampoCaptcha => By.XPath("//input[contains(@id, 'captcha') or contains(@name, 'captcha') or contains(@class, 'captcha')]");
    private By BotaoConsultar => By.XPath("//button[contains(text(), 'Consultar')]");
    private By MensagemObjetoNaoEncontrado => By.XPath("//*[contains(text(), 'Objeto não encontrado na base')]");
    private By BotaoOk => By.XPath("//button[contains(text(), 'OK') or text()='OK']");
    // Mensagens de erro de captcha - incluindo "Captcha inválido"
    private By MensagemCaptchaInvalido => By.XPath("//*[contains(text(), 'Captcha inválido') or contains(text(), 'captcha inválido') or contains(text(), 'CAPTCHA inválido')]");

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
        Thread.Sleep(1000);
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
    /// Limpa o campo de captcha e posiciona o cursor para nova digitação
    /// </summary>
    public void LimparEFocarCaptcha()
    {
        try
        {
            // Tentar encontrar o campo de captcha de várias formas
            IWebElement? campoCaptcha = null;
            
            // Primeiro tenta pelo XPath definido
            var captchas = _driver.FindElements(CampoCaptcha);
            foreach (var captcha in captchas)
            {
                if (captcha.Displayed)
                {
                    campoCaptcha = captcha;
                    break;
                }
            }
            
            // Se não encontrou, tenta por outros métodos
            if (campoCaptcha == null)
            {
                // Procurar input com borda vermelha (indicando erro)
                var inputs = _driver.FindElements(By.CssSelector("input[type='text']"));
                foreach (var input in inputs)
                {
                    if (input.Displayed)
                    {
                        var style = input.GetCssValue("border-color");
                        if (style.Contains("red") || style.Contains("255, 0, 0"))
                        {
                            campoCaptcha = input;
                            break;
                        }
                    }
                }
            }
            
            // Se ainda não encontrou, pega o segundo input de texto (o primeiro é o código)
            if (campoCaptcha == null)
            {
                var inputs = _driver.FindElements(By.CssSelector("input[type='text']"));
                if (inputs.Count > 1 && inputs[1].Displayed)
                {
                    campoCaptcha = inputs[1];
                }
            }
            
            if (campoCaptcha != null)
            {
                // Limpar o campo
                campoCaptcha.Clear();
                
                // Usar JavaScript para garantir que o campo está focado e limpo
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].focus();", campoCaptcha);
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = '';", campoCaptcha);
                
                // Clicar no campo para garantir foco
                campoCaptcha.Click();
                
                Console.WriteLine("[CAPTCHA] Campo de captcha limpo e cursor posicionado para digitação.");
            }
            else
            {
                Console.WriteLine("[CAPTCHA] AVISO: Campo de captcha não encontrado para limpar.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CAPTCHA] Erro ao limpar/focar captcha: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Limpa o campo de captcha para nova tentativa (método legado)
    /// </summary>
    public void LimparCaptcha()
    {
        LimparEFocarCaptcha();
    }

    /// <summary>
    /// Verifica se há mensagem de erro de captcha "Captcha inválido"
    /// </summary>
    public bool CaptchaIncorreto()
    {
        try
        {
            // Verificar mensagem "Captcha inválido"
            var erros = _driver.FindElements(MensagemCaptchaInvalido);
            if (erros.Any(e => e.Displayed))
            {
                Console.WriteLine("[CAPTCHA] Detectada mensagem: 'Captcha inválido'");
                return true;
            }

            // Verificar também por texto em qualquer elemento
            var todosElementos = _driver.FindElements(By.XPath("//*[contains(text(), 'inválido')]"));
            foreach (var elem in todosElementos)
            {
                if (elem.Displayed && elem.Text.ToLower().Contains("captcha"))
                {
                    Console.WriteLine($"[CAPTCHA] Detectada mensagem de erro: '{elem.Text}'");
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Verifica se o resultado da consulta apareceu
    /// </summary>
    private bool ResultadoApareceu()
    {
        try
        {
            var mensagem = _driver.FindElements(MensagemObjetoNaoEncontrado);
            if (mensagem.Any(e => e.Displayed))
                return true;

            // Verificar se há resultado de rastreamento (pode ter outras mensagens)
            var resultados = _driver.FindElements(By.CssSelector(".resultado, .timeline, .evento"));
            if (resultados.Any(e => e.Displayed))
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Aguarda o tempo para o usuário preencher o captcha e clica em consultar.
    /// Se o captcha estiver incorreto, limpa o campo e permite nova tentativa.
    /// </summary>
    /// <param name="tempoEsperaSegundos">Tempo de espera para preencher o captcha</param>
    /// <param name="maxTentativas">Número máximo de tentativas</param>
    public void AguardarCaptchaEConsultar(int tempoEsperaSegundos = 10, int maxTentativas = 3)
    {
        for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
        {
            Console.WriteLine($"\n========================================");
            Console.WriteLine($"[CAPTCHA] Tentativa {tentativa} de {maxTentativas}");
            Console.WriteLine($"[CAPTCHA] Aguardando {tempoEsperaSegundos} segundos para você preencher o captcha...");
            Console.WriteLine($"========================================\n");

            // Aguardar o usuário preencher o captcha
            Thread.Sleep(tempoEsperaSegundos * 1000);

            // Clicar no botão de consultar
            Console.WriteLine("[CAPTCHA] Clicando no botão Consultar...");
            ClicarBotaoConsultar();

            // Aguardar processamento
            Thread.Sleep(1500);

            // PRIMEIRO: Verificar se o captcha estava incorreto
            if (CaptchaIncorreto())
            {
                Console.WriteLine("[CAPTCHA] ERRO DETECTADO: Captcha incorreto!");
                
                if (tentativa < maxTentativas)
                {
                    Console.WriteLine("[CAPTCHA] Limpando campo e preparando para nova tentativa...");
                    
                    // Fechar modal de erro se existir
                    ClicarOk();
                    Thread.Sleep(300);
                    
                    // Limpar o campo de captcha e focar para nova digitação
                    LimparEFocarCaptcha();
                    
                    Console.WriteLine("[CAPTCHA] Campo limpo! Digite o novo captcha...");
                    continue; // Volta para o início do loop
                }
                else
                {
                    Console.WriteLine($"[CAPTCHA] Máximo de {maxTentativas} tentativas atingido.");
                    return;
                }
            }

            // SEGUNDO: Verificar se o resultado apareceu (sucesso)
            if (ResultadoApareceu())
            {
                Console.WriteLine("[CAPTCHA] Resultado obtido com sucesso!");
                return;
            }
            
            // Se não apareceu resultado nem erro de captcha, aguardar mais um pouco
            Thread.Sleep(1000);
            
            if (ResultadoApareceu())
            {
                Console.WriteLine("[CAPTCHA] Resultado obtido com sucesso!");
                return;
            }
            
            // Se ainda não tem resultado, preparar próxima tentativa
            if (tentativa < maxTentativas)
            {
                Console.WriteLine($"[CAPTCHA] Resultado não apareceu. Preparando tentativa {tentativa + 1}...");
                ClicarOk();
                Thread.Sleep(300);
                LimparEFocarCaptcha();
            }
        }
        
        Console.WriteLine($"[CAPTCHA] Não foi possível obter resultado após {maxTentativas} tentativas.");
    }
    
    /// <summary>
    /// Clica no botão de consultar usando JavaScript se necessário
    /// </summary>
    private void ClicarBotaoConsultar()
    {
        try
        {
            var botao = _driver.FindElement(BotaoConsultar);
            if (botao.Displayed && botao.Enabled)
            {
                // Usar JavaScript para garantir o clique
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", botao);
                Console.WriteLine("[CAPTCHA] Botão 'Consultar' clicado com sucesso!");
                return;
            }
        }
        catch { }
        
        Console.WriteLine("[CAPTCHA] AVISO: Botão 'Consultar' não encontrado!");
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
            var botoes = _driver.FindElements(BotaoOk);
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
        catch
        {
            // Modal pode não existir
        }
    }
}
