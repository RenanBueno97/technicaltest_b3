namespace technicaltest_b3.Support;

/// <summary>
/// Configurações centralizadas para os testes
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// URL base do site dos Correios
    /// </summary>
    public const string CorreiosBaseUrl = "https://www.correios.com.br/";
    
    /// <summary>
    /// URL da página de busca de CEP
    /// </summary>
    public const string BuscaCepUrl = "https://buscacepinter.correios.com.br/app/endereco/index.php?";
    
    /// <summary>
    /// URL da página de rastreamento
    /// </summary>
    public const string RastreamentoUrl = "https://rastreamento.correios.com.br/app/index.php";
    
    /// <summary>
    /// Tempo padrão de espera para o captcha (em segundos)
    /// </summary>
    public const int CaptchaWaitTimeSeconds = 10;
    
    /// <summary>
    /// Timeout padrão para elementos (em segundos)
    /// </summary>
    public const int DefaultTimeoutSeconds = 15;
    
    /// <summary>
    /// Número máximo de tentativas para o captcha
    /// </summary>
    public const int MaxCaptchaRetries = 3;
}
