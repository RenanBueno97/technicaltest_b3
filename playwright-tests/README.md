# Testes Playwright - Correios

Testes automatizados utilizando **Playwright** para validar funcionalidades do site dos Correios.

## Pré-requisitos

- [Node.js](https://nodejs.org/) 18+ instalado
- Conexão com a internet

## Instalação

```bash
# 1. Instalar dependências
npm install

# 2. Instalar browsers do Playwright
npx playwright install
```

## Estrutura

```
playwright-tests/
├── tests/
│   └── correios.spec.ts    # Teste completo do fluxo
├── playwright.config.ts    # Configuração do Playwright
├── package.json            # Dependências e scripts
└── README.md               # Este arquivo
```

## Cenários de Teste

O arquivo `correios.spec.ts` cobre todos os cenários em um único fluxo:

| Etapa | Cenário | Validação |
|-------|---------|-----------|
| 1 | Buscar CEP `80700000` | Mensagem "Não há dados a serem exibidos" |
| 2 | Buscar CEP `01013001` | Endereço "Rua Quinze de Novembro, São Paulo/SP" |
| 3 | Rastrear `SS987654321BR` | Mensagem "Objeto não encontrado na base" |

## Executar Testes

### ⚠️ IMPORTANTE: Captcha Manual

Os testes **DEVEM ser executados no modo headed** (navegador visível) para preenchimento manual do captcha.

### Modo Headed (Recomendado)

```bash
# Executar com navegador visível
npx playwright test --headed

# Ou usando o script npm
npm run test:headed
```

### Outros Modos

```bash
# Modo headless (sem interface - NÃO recomendado para este teste)
npx playwright test

# Modo UI (interface gráfica do Playwright)
npx playwright test --ui

# Modo debug (passo a passo)
npx playwright test --debug

# Gerar código de teste (Codegen)
npx playwright codegen
```

## Captcha Manual

Durante a execução dos testes:

1. ✅ O navegador abrirá automaticamente
2. ✅ O teste preencherá os campos (CEP ou código)
3. ⏱️ **Você terá 10 segundos para preencher o captcha**
4. ✅ O teste clicará no botão automaticamente
5. 🔄 Se incorreto, haverá até 3 tentativas

## Relatórios

### Gerar e visualizar relatório HTML

```bash
# Executar testes
npx playwright test --headed

# Abrir relatório no browser
npx playwright show-report

# Ou usando o script npm
npm run test:report
```

## Scripts Disponíveis

| Script | Comando | Descrição |
|--------|---------|-----------|
| `npm test` | `npx playwright test` | Executa testes (headless) |
| `npm run test:headed` | `npx playwright test --headed` | Executa com browser visível |
| `npm run test:ui` | `npx playwright test --ui` | Interface gráfica |
| `npm run test:debug` | `npx playwright test --debug` | Modo debug |
| `npm run test:report` | `npx playwright show-report` | Abre relatório |
| `npm run test:codegen` | `npx playwright codegen` | Gera código |

## Configurações

### Alterar timeout global

No arquivo `playwright.config.ts`:

```typescript
export default defineConfig({
  timeout: 60000, // 60 segundos
  // ...
});
```

### Alterar tempo de espera do captcha

No arquivo `tests/correios.spec.ts`, na função `preencherCaptchaEBuscar`:

```typescript
await page.waitForTimeout(15000); // 15 segundos
```

## Tecnologias

- **Playwright** - Framework de automação end-to-end
- **TypeScript** - Linguagem tipada para JavaScript
- **Chromium** - Browser padrão para execução

## Documentação

Para mais informações, consulte a [documentação oficial do Playwright](https://playwright.dev/).
