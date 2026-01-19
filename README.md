# Technical Test B3 - Testes Automatizados dos Correios

Projeto de testes automatizados para validar funcionalidades do site dos Correios, utilizando tres abordagens:

- **xUnit + Selenium** (C#)
- **SpecFlow + Selenium** (C# - BDD)
- **Playwright** (TypeScript)

## Estrutura do Projeto

```
technicaltest_b3/
├── playwright-tests/          # Testes Playwright (TypeScript)
│   ├── tests/
│   │   └── correios.spec.ts   # Teste completo do fluxo
│   ├── package.json
│   ├── playwright.config.ts
│   └── README.md
│
├── xunit-tests/               # Testes xUnit + Selenium (C#)
│   ├── PageObjects/           # Page Objects (padrao de design)
│   │   ├── BuscaCepPage.cs
│   │   ├── CorreiosHomePage.cs
│   │   └── RastreamentoPage.cs
│   ├── Tests/
│   │   ├── CorreiosTests.cs   # 3 testes independentes
│   │   └── PriorityOrderer.cs
│   ├── technicaltest_b3.csproj
│   └── README.md
│
├── specflow-tests/            # Testes SpecFlow + Selenium (C# - BDD)
│   ├── Features/
│   │   └── Correios.feature   # Cenarios em Gherkin (portugues)
│   ├── StepDefinitions/
│   │   └── CorreiosSteps.cs   # Implementacao dos steps
│   ├── Hooks/
│   │   └── WebDriverHooks.cs  # Gerenciamento do WebDriver
│   ├── PageObjects/
│   │   ├── BuscaCepPage.cs
│   │   ├── CorreiosHomePage.cs
│   │   └── RastreamentoPage.cs
│   ├── specflow.json          # Configuracao do SpecFlow (pt-BR)
│   ├── specflow-tests.csproj
│   └── README.md
│
├── run-all-tests.bat          # Script para executar todos os testes (Windows CMD)
├── run-all-tests.ps1          # Script para executar todos os testes (PowerShell)
├── run-all-tests.sh           # Script para executar todos os testes (Linux/Mac)
└── README.md                  # Este arquivo
```

---

## Executar Todos os Testes (xUnit + SpecFlow)

Para executar todos os testes em ordem (xUnit primeiro, SpecFlow depois):

### Windows (CMD)
```cmd
run-all-tests.bat
```

### Windows (PowerShell)
```powershell
.\run-all-tests.ps1
```

### Linux/Mac (Bash)
```bash
chmod +x run-all-tests.sh
./run-all-tests.sh
```

### Ordem de Execucao
1. **xUnit** - 3 testes (CEP invalido, CEP valido, Rastreamento)
2. **SpecFlow** - 3 cenarios BDD (mesmos testes em formato Gherkin)

---

## Cenarios de Teste

Todas as implementacoes cobrem os mesmos cenarios:

| # | Cenario | Entrada | Resultado Esperado |
|---|---------|---------|-------------------|
| 1 | Buscar CEP invalido | `80700000` | Mensagem "Nao ha dados a serem exibidos" |
| 2 | Buscar CEP valido | `01013001` | Endereco "Rua Quinze de Novembro, Sao Paulo/SP" |
| 3 | Buscar rastreamento invalido | `SS987654321BR` | Mensagem "Objeto nao encontrado na base" |

---

## Testes xUnit + Selenium (C#)

### Pre-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) ou superior
- [Google Chrome](https://www.google.com/chrome/) instalado
- Conexao com a internet

### Instalacao e Execucao

```bash
# 1. Entrar na pasta dos testes xUnit
cd xunit-tests

# 2. Restaurar dependencias
dotnet restore

# 3. Compilar o projeto
dotnet build

# 4. Executar todos os testes
dotnet test
```

### Executar Testes Especificos

```bash
cd xunit-tests

# Apenas teste de CEP invalido
dotnet test --filter "Teste01_BuscarCepInvalido"

# Apenas teste de CEP valido
dotnet test --filter "Teste02_BuscarCepValido"

# Apenas teste de rastreamento
dotnet test --filter "Teste03_BuscarRastreamentoInvalido"
```

---

## Testes SpecFlow + Selenium (C# - BDD)

### Pre-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- [Google Chrome](https://www.google.com/chrome/) instalado
- Conexao com a internet

### Instalacao e Execucao

```bash
# 1. Entrar na pasta dos testes SpecFlow
cd specflow-tests

# 2. Restaurar dependencias
dotnet restore

# 3. Compilar o projeto
dotnet build

# 4. Executar todos os cenarios
dotnet test
```

### Executar por Tag

```bash
cd specflow-tests

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

### Cenarios em Gherkin (Portugues)

```gherkin
# language: pt-BR
Funcionalidade: Validacao de funcionalidades do site dos Correios

    Contexto: Acessar o site dos Correios
        Dado que estou no site dos Correios
        E aceito os cookies se existirem

    @CEP @Invalido @ID
    Cenario: Buscar CEP invalido deve retornar mensagem de erro
        Quando eu navego para a pagina de busca de CEP
        E preencho o campo de CEP com "80700000" usando seletor ID
        E preencho o captcha manualmente
        E clico no botao Buscar
        Entao devo ver a mensagem "Nao ha dados a serem exibidos"
```

---

## Testes Playwright (TypeScript)

### Pre-requisitos

- [Node.js](https://nodejs.org/) 18+ instalado
- Conexao com a internet

### Instalacao e Execucao

```bash
# 1. Entrar na pasta dos testes Playwright
cd playwright-tests

# 2. Instalar dependencias
npm install

# 3. Instalar browsers do Playwright
npx playwright install

# 4. Executar testes (modo headless)
npx playwright test
```

### Modos de Execucao

```bash
cd playwright-tests

# Modo headless (padrao - sem interface)
npx playwright test

# Modo headed (ver o browser)
npx playwright test --headed

# Modo UI (interface grafica do Playwright)
npx playwright test --ui

# Modo debug (passo a passo)
npx playwright test --debug
```

---

## Captcha Manual

Todos os testes requerem preenchimento manual do captcha:

1. O navegador Chrome abrira automaticamente
2. O teste preenchera os campos necessarios
3. **Voce tera 8-10 segundos para preencher o captcha**
4. O teste clicara no botao automaticamente
5. Se incorreto, o campo sera limpo (ate 3 tentativas)

---

## Tecnologias Utilizadas

### xUnit + Selenium (C#)
| Tecnologia | Descricao |
|------------|-----------|
| **xUnit** | Framework de testes .NET |
| **Selenium WebDriver** | Automacao de browser |
| **WebDriverManager** | Download automatico do ChromeDriver |
| **FluentAssertions** | Assertions mais legiveis |
| **Page Object Model** | Padrao de design para manutenibilidade |

### SpecFlow + Selenium (C# - BDD)
| Tecnologia | Descricao |
|------------|-----------|
| **SpecFlow** | Framework BDD para .NET |
| **Gherkin** | Linguagem de cenarios (portugues) |
| **Selenium WebDriver** | Automacao de browser |
| **Page Object Model** | Padrao de design para manutenibilidade |

### Playwright (TypeScript)
| Tecnologia | Descricao |
|------------|-----------|
| **Playwright** | Framework de automacao end-to-end |
| **TypeScript** | Linguagem tipada para JavaScript |

---

## Comparativo das Abordagens

| Aspecto | xUnit + Selenium | SpecFlow + Selenium | Playwright |
|---------|------------------|---------------------|------------|
| Linguagem | C# | C# + Gherkin | TypeScript |
| Estilo | Testes unitarios | BDD (Behavior-Driven) | End-to-end |
| Testes | 3 independentes | 3 cenarios Gherkin | 1 fluxo completo |
| Browser | Chrome | Chrome | Chromium, Firefox, WebKit |
| Padrao | Page Object Model | Page Object Model | Funcoes auxiliares |
| Assertions | FluentAssertions | FluentAssertions | expect nativo |
| Relatorios | Console | Console | HTML interativo |

---

## Seletores Demonstrados

Todos os testes demonstram os tres tipos de seletores:

| Tipo | Exemplo | Uso |
|------|---------|-----|
| **ID** | `By.Id("endereco")` | Campo de CEP |
| **XPath** | `By.XPath("//td[contains(text(), '...')]")` | Validacao de resultados |
| **CSS** | `By.CssSelector("input[type='text']")` | Campo de rastreamento |

---

## Troubleshooting

### ChromeDriver nao encontrado
O WebDriverManager baixa automaticamente. Verifique sua conexao com a internet.

### Arquivo bloqueado (MSB3027)
Feche todas as janelas do Chrome e o Test Explorer, depois execute novamente:
```bash
dotnet build
dotnet test
```

### Timeout no captcha
Aumente o tempo de espera nos arquivos de teste:
```csharp
// xUnit/SpecFlow
AguardarCaptchaEBuscar(tempoEsperaSegundos: 15);
```

### Playwright: Browsers nao instalados
```bash
npx playwright install
```

---

## Versionamento com Git

### Comandos para submeter o codigo ao repositorio

```bash
# 1. Verificar status dos arquivos modificados
git status

# 2. Adicionar todos os arquivos ao staging
git add .

# 3. Criar commit com mensagem descritiva
git commit -m "feat: implementacao dos testes automatizados dos Correios

- Testes xUnit + Selenium (3 testes)
- Testes SpecFlow + Selenium (3 cenarios BDD)
- Testes Playwright (1 fluxo completo)
- Page Object Model implementado
- Seletores por ID, XPath e CSS demonstrados
- Tratamento de captcha manual com retry"

# 4. Enviar para o repositorio remoto (branch main)
git push origin main
```

### Fluxo completo para novo repositorio

```bash
# 1. Inicializar repositorio Git (se ainda nao existir)
git init

# 2. Adicionar repositorio remoto
git remote add origin https://github.com/seu-usuario/technicaltest_b3.git

# 3. Adicionar arquivos
git add .

# 4. Criar primeiro commit
git commit -m "feat: implementacao inicial dos testes automatizados"

# 5. Definir branch principal
git branch -M main

# 6. Enviar para o repositorio
git push -u origin main
```

### Comandos uteis para versionamento diario

```bash
# Ver historico de commits
git log --oneline

# Ver alteracoes pendentes
git diff

# Criar nova branch para feature
git checkout -b feature/nova-funcionalidade

# Voltar para main e fazer merge
git checkout main
git merge feature/nova-funcionalidade

# Atualizar codigo local com remoto
git pull origin main

# Criar tag de versao
git tag -a v1.0.0 -m "Versao 1.0.0 - Testes automatizados Correios"
git push origin v1.0.0
```

### Boas praticas de commit

| Prefixo | Uso |
|---------|-----|
| `feat:` | Nova funcionalidade |
| `fix:` | Correcao de bug |
| `refactor:` | Refatoracao de codigo |
| `test:` | Adicao/alteracao de testes |
| `docs:` | Alteracao em documentacao |
| `chore:` | Manutencao geral |

---

## Autor

**Renan Bueno**

## Licenca

Este projeto e parte de um teste tecnico para B3.
