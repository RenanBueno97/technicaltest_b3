# Ambiente de Testes Playwright

Este projeto contém um ambiente configurado para criar e executar testes com Playwright.

## 📋 Pré-requisitos

- Node.js (versão 16 ou superior)
- npm ou yarn

## 🚀 Instalação

1. Instale as dependências:
```bash
npm install
```

2. Instale os navegadores do Playwright:
```bash
npx playwright install
```

## 🧪 Executar Testes

### ⚠️ IMPORTANTE: Testes dos Correios (correios.spec.ts)

**Os testes do arquivo `correios.spec.ts` DEVEM ser executados no modo headed (com navegador visível)**, pois requerem preenchimento manual de Captcha durante a execução.

#### Executar teste dos Correios no modo headed
```bash
npm run test:headed tests/correios.spec.ts
```

#### Como funciona:
1. O teste abrirá o navegador visível
2. Durante a execução, o teste aguardará **10 segundos** para você preencher o Captcha manualmente
3. Você terá até **3 tentativas** para preencher o Captcha corretamente
4. O teste continua automaticamente após o Captcha ser validado

#### Fluxo testado:
- ✅ Busca de CEP inválido (80700000)
- ✅ Busca de CEP válido (01013001 - Rua Quinze de Novembro, São Paulo/SP)
- ✅ Rastreamento de objeto (código SS987654321BR)

### Executar todos os testes
```bash
npm test
```

### Executar testes com interface gráfica (headed mode)
```bash
npm run test:headed
```

### Executar testes com UI Mode (recomendado para desenvolvimento)
```bash
npm run test:ui
```

### Executar testes em modo debug
```bash
npm run test:debug
```

### Gerar código de teste (Codegen)
```bash
npm run test:codegen
```

### Ver relatório de testes
```bash
npm run test:report
```

## 📁 Estrutura do Projeto

```
playwright-tests/
├── tests/                  # Diretório com os testes
│   └── correios.spec.ts    # Testes dos Correios (CEP e Rastreamento)
├── playwright.config.ts     # Configuração do Playwright
├── package.json            # Dependências e scripts
└── README.md               # Este arquivo
```

## 📚 Documentação

Para mais informações, consulte a [documentação oficial do Playwright](https://playwright.dev/).
