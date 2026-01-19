# Testes xUnit + Selenium - Correios

Testes automatizados utilizando **xUnit** e **Selenium WebDriver** para validar funcionalidades do site dos Correios.

## Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) ou superior
- [Google Chrome](https://www.google.com/chrome/) instalado
- Conexão com a internet

## Estrutura

```
xunit-tests/
├── PageObjects/
│   ├── BuscaCepPage.cs         # Page Object para busca de CEP
│   ├── CorreiosHomePage.cs     # Page Object para home dos Correios
│   └── RastreamentoPage.cs     # Page Object para rastreamento
├── Tests/
│   ├── CorreiosTests.cs        # 3 testes xUnit
│   └── PriorityOrderer.cs      # Ordenador de execução
└── technicaltest_b3.csproj     # Arquivo do projeto
```

## Testes Implementados

| Ordem | Método | Descrição |
|-------|--------|-----------|
| 1 | `Teste01_BuscarCepInvalido_DeveRetornarMensagemDeErro` | Busca CEP `80700000` → Valida mensagem de erro |
| 2 | `Teste02_BuscarCepValido_DeveRetornarEnderecoCorreto` | Busca CEP `01013001` → Valida endereço retornado |
| 3 | `Teste03_BuscarRastreamentoInvalido_DeveRetornarMensagemDeErro` | Busca código `SS987654321BR` → Valida mensagem |

## Como Executar

### Restaurar dependências

```bash
dotnet restore
```

### Compilar

```bash
dotnet build
```

### Executar todos os testes

```bash
dotnet test
```

### Executar teste específico

```bash
# CEP inválido
dotnet test --filter "Teste01_BuscarCepInvalido"

# CEP válido
dotnet test --filter "Teste02_BuscarCepValido"

# Rastreamento
dotnet test --filter "Teste03_BuscarRastreamentoInvalido"
```

### Executar com verbosidade

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Captcha Manual

Os testes requerem preenchimento manual do captcha:

1. ✅ O navegador Chrome abrirá automaticamente
2. ✅ O teste preencherá os campos necessários
3. ⏱️ **Você terá 8 segundos para preencher o captcha**
4. ✅ O teste clicará no botão automaticamente
5. 🔄 Se incorreto, o campo será limpo (até 3 tentativas)

### Logs no Console

```
========================================
[CAPTCHA] Tentativa 1 de 3
[CAPTCHA] Aguardando 8 segundos para você preencher o captcha...
========================================

[CAPTCHA] Clicando no botão Buscar...
[CAPTCHA] Botão 'Buscar' clicado com sucesso!
[CAPTCHA] Resultado obtido com sucesso!
```

## Tecnologias

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| xunit | 2.9.2 | Framework de testes |
| Selenium.WebDriver | 4.26.1 | Automação de browser |
| WebDriverManager | 2.17.4 | Gerenciamento do ChromeDriver |
| FluentAssertions | 6.12.2 | Assertions legíveis |

## Arquitetura

O projeto utiliza o padrão **Page Object Model**:

```
CorreiosTests (Testes)
    ├── BuscaCepPage (Page Object)
    │   ├── Navegar()
    │   ├── PreencherCep()
    │   ├── AguardarCaptchaEBuscar()
    │   └── MensagemCepNaoEncontradoVisivel()
    │
    └── RastreamentoPage (Page Object)
        ├── Navegar()
        ├── PreencherCodigoRastreio()
        ├── AguardarCaptchaEConsultar()
        └── MensagemObjetoNaoEncontradoVisivel()
```

## Configurações

### Alterar tempo de espera do captcha

No arquivo `Tests/CorreiosTests.cs`:

```csharp
// Alterar de 8 para outro valor (em segundos)
_buscaCepPage.AguardarCaptchaEBuscar(tempoEsperaSegundos: 15);
```

### Alterar número de tentativas

```csharp
_buscaCepPage.AguardarCaptchaEBuscar(tempoEsperaSegundos: 8, maxTentativas: 5);
```
