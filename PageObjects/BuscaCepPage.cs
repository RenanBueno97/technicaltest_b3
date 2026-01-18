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
    private readonly WebDriverWait _waitCurto;
    private const string Url = "https://buscacepinter.correios.com.br/app/endereco/index.php?";

    // Locators
    private By CampoCepOuEndereco => By.Id("endereco");
    private By CampoCaptcha => By.Id("captcha");
    private By BotaoBuscar => By.Id("btn_pesquisar");
    private By BotaoNovaBusca => By.XPath("//button[contains(text(), 'Nova Busca')]");
    private By MensagemSemDados => By.XPath("//*[contains(text(), 'Não há dados a serem exibidos')]");
    private By HeadingDadosNaoEncontrado => By.XPath("//*[contains(text(), 'Dados não encontrado')]");
    private By TabelaResultados => By.TagName("table");
    // Mensagens de erro de captcha - incluindo várias possibilidades
    private By MensagemErroCaptcha => By.XPath("//*[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'código') and (contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'verificação') or contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'incorreto') or contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'inválido'))]");

    public BuscaCepPage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        _waitCurto = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
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
    /// Limpa o campo de captcha e posiciona o cursor para nova digitação
    /// </summary>
    public void LimparEFocarCaptcha()
    {
        try
        {
            var captcha = _driver.FindElement(CampoCaptcha);
            
            // Limpar o campo
            captcha.Clear();
            
            // Usar JavaScript para garantir que o campo está focado
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].focus();", captcha);
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = '';", captcha);
            
            // Clicar no campo para garantir foco
            captcha.Click();
            
            Console.WriteLine("[CAPTCHA] Campo de captcha limpo e cursor posicionado para digitação.");
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
    /// Verifica se há mensagem de erro de captcha visível
    /// </summary>
    public bool CaptchaIncorreto()
    {
        try
        {
            // Verificar mensagens de erro de captcha conhecidas
            var erros = _driver.FindElements(MensagemErroCaptcha);
            if (erros.Any(e => e.Displayed))
            {
                Console.WriteLine("[CAPTCHA] Detectada mensagem de erro de captcha!");
                return true;
            }
            
            // Verificar por "Captcha inválido" ou similar
            var captchaInvalido = _driver.FindElements(By.XPath("//*[contains(text(), 'Captcha inválido') or contains(text(), 'captcha inválido')]"));
            if (captchaInvalido.Any(e => e.Displayed))
            {
                Console.WriteLine("[CAPTCHA] Detectada mensagem: 'Captcha inválido'");
                return true;
            }
            
            // Verificar por texto genérico de erro com captcha
            var todosElementos = _driver.FindElements(By.XPath("//*[contains(text(), 'inválido') or contains(text(), 'incorreto')]"));
            foreach (var elem in todosElementos)
            {
                if (elem.Displayed && elem.Text.ToLower().Contains("captcha"))
                {
                    Console.WriteLine($"[CAPTCHA] Detectada mensagem de erro: '{elem.Text}'");
                    return true;
                }
            }
            
            // Verificar também por alert ou modal de erro
            var alertas = _driver.FindElements(By.CssSelector(".alert, .error, .alert-danger, .alert-error, .swal2-popup"));
            foreach (var alerta in alertas)
            {
                if (alerta.Displayed && (alerta.Text.ToLower().Contains("captcha") || alerta.Text.ToLower().Contains("código")))
                {
                    Console.WriteLine($"[CAPTCHA] Detectado alerta de erro: '{alerta.Text}'");
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
    /// Verifica se o resultado da busca já apareceu (positivo ou negativo)
    /// </summary>
    private bool ResultadoApareceu()
    {
        try
        {
            // Verifica se a mensagem de "não encontrado" apareceu
            var semDados = _driver.FindElements(MensagemSemDados);
            if (semDados.Any(e => e.Displayed))
                return true;

            // Verifica se a tabela de resultados apareceu
            var tabela = _driver.FindElements(TabelaResultados);
            if (tabela.Any(e => e.Displayed))
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Aguarda o tempo para o usuário preencher o captcha e clica em buscar.
    /// Se o captcha estiver incorreto, limpa o campo e permite nova tentativa.
    /// </summary>
    /// <param name="tempoEsperaSegundos">Tempo de espera para preencher o captcha</param>
    /// <param name="maxTentativas">Número máximo de tentativas</param>
    public void AguardarCaptchaEBuscar(int tempoEsperaSegundos = 10, int maxTentativas = 3)
    {
        for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
        {
            Console.WriteLine($"\n========================================");
            Console.WriteLine($"[CAPTCHA] Tentativa {tentativa} de {maxTentativas}");
            Console.WriteLine($"[CAPTCHA] Aguardando {tempoEsperaSegundos} segundos para você preencher o captcha...");
            Console.WriteLine($"========================================\n");
            
            // Aguardar o usuário preencher o captcha
            Thread.Sleep(tempoEsperaSegundos * 1000);

            // Clicar no botão de buscar
            Console.WriteLine("[CAPTCHA] Clicando no botão Buscar...");
            ClicarBotaoBuscar();

            // Aguardar processamento
            Thread.Sleep(1500);

            // PRIMEIRO: Verificar se o captcha estava incorreto
            if (CaptchaIncorreto())
            {
                Console.WriteLine("[CAPTCHA] ERRO DETECTADO: Captcha incorreto!");
                
                if (tentativa < maxTentativas)
                {
                    Console.WriteLine("[CAPTCHA] Limpando campo e preparando para nova tentativa...");
                    
                    // Fechar qualquer modal/alert se existir
                    FecharModalErro();
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
                FecharModalErro();
                Thread.Sleep(300);
                LimparEFocarCaptcha();
            }
        }
        
        Console.WriteLine($"[CAPTCHA] Não foi possível obter resultado após {maxTentativas} tentativas.");
    }
    
    /// <summary>
    /// Clica no botão de buscar usando JavaScript se necessário
    /// </summary>
    private void ClicarBotaoBuscar()
    {
        try
        {
            // Primeiro tenta o botão "Buscar" pelo ID
            var botao = _driver.FindElement(BotaoBuscar);
            if (botao.Displayed && botao.Enabled)
            {
                // Usar JavaScript para garantir o clique
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", botao);
                Console.WriteLine("[CAPTCHA] Botão 'Buscar' clicado com sucesso!");
                return;
            }
        }
        catch { }

        try
        {
            // Tenta o botão "Nova Busca"
            var botaoNovaBusca = _driver.FindElement(BotaoNovaBusca);
            if (botaoNovaBusca.Displayed && botaoNovaBusca.Enabled)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", botaoNovaBusca);
                Console.WriteLine("[CAPTCHA] Botão 'Nova Busca' clicado com sucesso!");
                return;
            }
        }
        catch { }
        
        Console.WriteLine("[CAPTCHA] AVISO: Nenhum botão de busca encontrado!");
    }

    /// <summary>
    /// Fecha modal de erro se existir
    /// </summary>
    private void FecharModalErro()
    {
        try
        {
            var botoesFechar = _driver.FindElements(By.XPath("//button[contains(text(), 'OK') or contains(text(), 'Fechar') or contains(text(), 'X')]"));
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
    /// Verifica se a mensagem de CEP não encontrado está visível
    /// </summary>
    public bool MensagemCepNaoEncontradoVisivel()
    {
        try
        {
            // Aguardar um pouco para a página carregar o resultado
            Thread.Sleep(1000);
            
            // Verificar se a mensagem "Não há dados a serem exibidos" está visível
            var mensagens = _driver.FindElements(MensagemSemDados);
            bool mensagemVisivel = mensagens.Any(m => m.Displayed);
            
            // Verificar se "Dados não encontrado" está visível
            var headings = _driver.FindElements(HeadingDadosNaoEncontrado);
            bool headingVisivel = headings.Any(h => h.Displayed);
            
            return mensagemVisivel || headingVisivel;
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
