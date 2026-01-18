import { defineConfig, devices } from '@playwright/test';

/**
 * Leia a documentação do Playwright: https://playwright.dev/docs/test-configuration
 * Mais informações: https://playwright.dev/docs/test-configuration
 */
export default defineConfig({
  testDir: './tests',
  
  /* Timeout máximo para cada teste */
  timeout: 120 * 1000, // 2 minutos para acomodar múltiplos waitForTimeout de captcha
  
  expect: {
    /* Timeout para assertions */
    timeout: 5000
  },
  
  /* Executar testes em paralelo */
  fullyParallel: true,
  
  /* Falhar o build se você deixar test.only no código */
  forbidOnly: !!process.env.CI,
  
  /* Retry em CI */
  retries: process.env.CI ? 2 : 0,
  
  /* Limite de workers para testes paralelos */
  workers: process.env.CI ? 1 : undefined,
  
  /* Configuração do reporter */
  reporter: 'html',
  
  /* Opções compartilhadas para todos os projetos */
  use: {
    /* URL base para usar em navegação */
    // baseURL: 'http://127.0.0.1:3000',
    
    /* Coletar trace quando retentar o teste falho */
    trace: 'on-first-retry',
    
    /* Screenshot apenas quando falhar */
    screenshot: 'only-on-failure',
    
    /* Vídeo apenas quando falhar */
    video: 'retain-on-failure',
  },

  /* Configurar projetos para vários navegadores */
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },

    // {
    //   name: 'firefox',
    //   use: { ...devices['Desktop Firefox'] },
    // },

    // {
    //   name: 'webkit',
    //   use: { ...devices['Desktop Safari'] },
    // },

    /* Testes em dispositivos móveis */
    // {
    //   name: 'Mobile Chrome',
    //   use: { ...devices['Pixel 5'] },
    // },
    // {
    //   name: 'Mobile Safari',
    //   use: { ...devices['iPhone 12'] },
    // },
  ],

  /* Executar servidor local antes de iniciar os testes */
  // webServer: {
  //   command: 'npm run start',
  //   url: 'http://127.0.0.1:3000',
  //   reuseExistingServer: !process.env.CI,
  // },
});
