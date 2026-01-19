# Testes SpecFlow + Selenium - Correios

Testes automatizados utilizando **SpecFlow** (BDD) e **Selenium WebDriver** para validar funcionalidades do site dos Correios.

## Pre-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- [Google Chrome](https://www.google.com/chrome/) instalado
- Conexao com a internet

## Estrutura

```
specflow-tests/
├── Features/
│   └── Correios.feature        # Cenarios em Gherkin (portugues)
├── StepDefinitions/
│   └── CorreiosSteps.cs        # Implementacao dos steps
├── Hooks/
│   └── WebDriverHooks.cs       # Gerenciamento do WebDriver
├── PageObjects/
│   ├── BuscaCepPage.cs         # Page Object para busca de CEP
│   ├── CorreiosHomePage.cs     # Page Object para home
│   └── RastreamentoPage.cs     # Page Object para rastreamento
├── specflow.json               # Configuracao do SpecFlow (pt-BR)
└── specflow-tests.csproj       # Arquivo do projeto
```

## Cenarios de Teste (Gherkin)

Os cenarios estao escritos em **portugues brasileiro**:

### Cenario 1: CEP Invalido (Tag: @ID)
```gherkin
@CEP @Invalido @ID
Cenario: Buscar CEP invalido deve retornar mensagem de erro
    Quando eu navego para a pagina de busca de CEP
    E preencho o campo de CEP com "80700000" usando seletor ID
    E preencho o captcha manualmente
    E clico no botao Buscar
    Entao devo ver a mensagem "Nao ha dados a serem exibidos"
```

### Cenario 2: CEP Valido (Tag: @XPath)
```gherkin
@CEP @Valido @XPath
Cenario: Buscar CEP valido deve retornar endereco correto
    ...
    Entao o resultado deve conter "Rua Quinze de Novembro" usando seletor XPath
    E o resultado deve conter "Sao Paulo" usando seletor XPath
```

### Cenario 3: Rastreamento Invalido (Tag: @CSS)
```gherkin
@Rastreamento @Invalido @CSS
Cenario: Buscar rastreamento invalido deve retornar mensagem de nao encontrado
    ...
    E preencho o campo de codigo com "SS987654321BR" usando seletor CSS
```

## Seletores Demonstrados

| Tipo | Exemplo | Cenario |
|------|---------|---------|
| **ID** | `By.Id("endereco")` | CEP Invalido |
| **XPath** | `By.XPath("//td[contains(text(), '...')]")` | CEP Valido |
| **CSS** | `By.CssSelector("input[type='text']")` | Rastreamento |

## Como Executar

### Restaurar dependencias

```bash
cd specflow-tests
dotnet restore
```

### Compilar

```bash
dotnet build
```

### Executar todos os cenarios

```bash
dotnet test
```

### Executar por tag

```bash
# Apenas cenarios com seletor ID
dotnet test --filter "Category=ID"

# Apenas cenarios com seletor XPath
dotnet test --filter "Category=XPath"

# Apenas cenarios com seletor CSS
dotnet test --filter "Category=CSS"

# Apenas cenarios de CEP
dotnet test --filter "Category=CEP"

# Apenas cenarios de rastreamento
dotnet test --filter "Category=Rastreamento"
```

### Executar com verbosidade

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Captcha Manual

Os testes requerem preenchimento manual do captcha:

1. O navegador Chrome abrira automaticamente
2. O teste preenchera os campos necessarios
3. **Voce tera 8 segundos para preencher o captcha**
4. O teste clicara no botao automaticamente
5. Se incorreto, o campo sera limpo (ate 3 tentativas)

## Logs no Console

```
========================================
[SPECFLOW] Iniciando cenario: Buscar CEP invalido deve retornar mensagem de erro
[SPECFLOW] Tags: CEP, Invalido, ID
========================================

[STEP] Given: que estou no site dos Correios
[STEP] Given: aceito os cookies se existirem
[STEP] When: eu navego para a pagina de busca de CEP

[DEMONSTRACAO] Usando seletor por ID: By.Id("endereco")
[ID] Preenchendo campo de CEP com seletor By.Id("endereco")

========================================
[CAPTCHA] Tentativa 1 de 3
[CAPTCHA] Aguardando 8 segundos...
========================================

[CAPTCHA] Resultado obtido com sucesso!

[DEMONSTRACAO] Verificando mensagem com seletor XPath
[XPath] Verificando mensagem com seletor XPath

========================================
[SPECFLOW] Finalizando cenario: Buscar CEP invalido deve retornar mensagem de erro
[SPECFLOW] Status: OK
========================================
```

## Tecnologias

| Pacote | Versao | Descricao |
|--------|--------|-----------|
| SpecFlow | 3.9.74 | Framework BDD |
| SpecFlow.xUnit | 3.9.74 | Integracao com xUnit |
| Selenium.WebDriver | 4.26.1 | Automacao de browser |
| WebDriverManager | 2.17.4 | Gerenciamento do ChromeDriver |
| FluentAssertions | 6.12.2 | Assertions legiveis |

## Arquitetura BDD

```
Feature (Gherkin)
    ↓
Step Definitions
    ↓
Page Objects
    ↓
Selenium WebDriver
    ↓
Browser (Chrome)
```

## Configuracao do Idioma

O arquivo `specflow.json` configura o idioma para portugues:

```json
{
  "language": {
    "feature": "pt-BR"
  }
}
```

Isso permite usar palavras-chave em portugues nos arquivos `.feature`:
- `Funcionalidade` (Feature)
- `Cenario` (Scenario)
- `Dado` (Given)
- `Quando` (When)
- `Entao` (Then)
- `E` (And)
