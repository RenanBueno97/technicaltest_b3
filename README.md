# Technical Test B3 - Testes Automatizados dos Correios

Projeto de testes automatizados utilizando **xUnit** e **Selenium WebDriver** para validar funcionalidades do site dos Correios.

## Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) ou superior
- [Google Chrome](https://www.google.com/chrome/) instalado
- Conexão com a internet

## Estrutura do Projeto

```
technicaltest_b3/
├── PageObjects/
│   ├── BuscaCepPage.cs         # Page Object para busca de CEP
│   ├── CorreiosHomePage.cs     # Page Object para home dos Correios
│   └── RastreamentoPage.cs     # Page Object para rastreamento
├── Tests/
│   ├── CorreiosTests.cs        # Testes xUnit
│   └── PriorityOrderer.cs      # Ordenador de testes
├── Support/
│   └── TestConfiguration.cs    # Configurações
└── technicaltest_b3.csproj     # Arquivo do projeto
```

## Testes Implementados

| # | Teste | Descrição |
|---|-------|-----------|
| 1 | `Teste01_BuscarCepInvalido_DeveRetornarMensagemDeErro` | Busca CEP inválido (80700000) e valida mensagem de erro |
| 2 | `Teste02_BuscarCepValido_DeveRetornarEnderecoCorreto` | Busca CEP válido (01013001) e valida endereço retornado |
| 3 | `Teste03_BuscarRastreamentoInvalido_DeveRetornarMensagemDeErro` | Busca código de rastreamento inválido e valida mensagem |

## Como Executar

### 1. Clonar o repositório

```bash
git clone https://github.com/RenanBueno97/technicaltest_b3.git
cd technicaltest_b3
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### 3. Compilar o projeto

```bash
dotnet build
```

### 4. Executar os testes

```bash
dotnet test
```

## Execução com Captcha Manual

Os testes requerem preenchimento manual do captcha. Durante a execução:

1. O navegador Chrome abrirá automaticamente
2. O teste preencherá os campos necessários
3. **Você terá 8 segundos para preencher o captcha manualmente**
4. O teste clicará no botão de busca automaticamente
5. Se o captcha estiver incorreto, o campo será limpo e você terá mais 8 segundos (até 3 tentativas)

### Logs no Console

Durante a execução, você verá mensagens como:

```
========================================
[CAPTCHA] Tentativa 1 de 3
[CAPTCHA] Aguardando 8 segundos para você preencher o captcha...
========================================

[CAPTCHA] Clicando no botão Buscar...
[CAPTCHA] Resultado obtido com sucesso!
```

## Executar Testes Específicos

```bash
# Executar apenas o teste de CEP inválido
dotnet test --filter "Teste01_BuscarCepInvalido"

# Executar apenas o teste de CEP válido
dotnet test --filter "Teste02_BuscarCepValido"

# Executar apenas o teste de rastreamento
dotnet test --filter "Teste03_BuscarRastreamentoInvalido"
```

## Executar com Mais Detalhes

```bash
# Execução com verbosidade normal
dotnet test --logger "console;verbosity=normal"

# Execução com verbosidade detalhada
dotnet test --logger "console;verbosity=detailed"
```

## Tecnologias Utilizadas

- **xUnit** - Framework de testes
- **Selenium WebDriver** - Automação de browser
- **WebDriverManager** - Gerenciamento automático do ChromeDriver
- **FluentAssertions** - Assertions mais legíveis
- **Page Object Model** - Padrão de design para organização

## Configuração do Tempo de Espera

O tempo de espera para preenchimento do captcha pode ser ajustado no arquivo `Tests/CorreiosTests.cs`:

```csharp
// Alterar de 8 para outro valor (em segundos)
_buscaCepPage.AguardarCaptchaEBuscar(tempoEsperaSegundos: 8);
```

## Troubleshooting

### Erro: ChromeDriver não encontrado
O WebDriverManager baixa automaticamente o ChromeDriver. Certifique-se de ter conexão com a internet.

### Erro: Arquivo bloqueado
Se aparecer erro de arquivo bloqueado durante o build, feche todas as janelas do Chrome e o Test Explorer, depois execute novamente.

### Timeout nos testes
Se precisar de mais tempo para preencher o captcha, aumente o valor de `tempoEsperaSegundos`.

## Autor

Renan Bueno

## Licença

Este projeto é parte de um teste técnico para B3.
