import { test, expect, Page } from '@playwright/test';

// Função auxiliar para lidar com captcha - permite até 3 tentativas
async function preencherCaptchaEBuscar(
  page: Page,
  botaoNome: string,
  maxTentativas: number = 3
) {
  let tentativas = 0;
  
  while (tentativas < maxTentativas) {
    try {
      // Aguardar o botão estar visível antes de aguardar o captcha
      // Se for "Buscar", tentar também "Nova Busca" caso o primeiro não exista
      let botao = page.getByRole('button', { name: botaoNome });
      try {
        await botao.waitFor({ state: 'visible', timeout: 3000 });
      } catch {
        // Se "Buscar" não for encontrado, tentar "Nova Busca"
        if (botaoNome === 'Buscar') {
          botao = page.getByRole('button', { name: 'Nova Busca' });
          await botao.waitFor({ state: 'visible', timeout: 10000 });
        } else {
          throw new Error(`Botão "${botaoNome}" não encontrado`);
        }
      }
      
      // Aguardar tempo para preencher o captcha manualmente
      await page.waitForTimeout(10000);
      
      // Verificar se o botão ainda está visível (pode ter desaparecido durante a espera)
      const botaoVisivel = await botao.isVisible().catch(() => false);
      if (!botaoVisivel) {
        // Se o botão não estiver mais visível, tentar encontrá-lo novamente
        await botao.waitFor({ state: 'visible', timeout: 5000 });
      }
      
      // Clicar no botão de buscar/consultar
      await botao.click();
      
      // Aguardar um pouco para a resposta aparecer (erro de captcha ou processamento)
      await page.waitForTimeout(2000);
      
      // Verificar se há mensagem de erro de captcha
      const erroCaptcha = page.getByText(/código de verificação|verificação incorreta|código incorreto|captcha inválido|código inválido/i).first();
      const erroCaptchaVisivel = await erroCaptcha.isVisible().catch(() => false);
      
      if (!erroCaptchaVisivel) {
        // Captcha correto, retornar - o código principal aguardará os resultados
        return true;
      }
      
      tentativas++;
      
      if (tentativas >= maxTentativas) {
        throw new Error(`Captcha incorreto após ${maxTentativas} tentativas. Teste falhou.`);
      }
      
      // Se ainda há tentativas, aguardar e tentar novamente
      await page.waitForTimeout(1000);
    } catch (error: any) {
      // Se a página foi fechada ou houve erro crítico, relançar o erro
      if (error.message && (error.message.includes('closed') || error.message.includes('Target page'))) {
        throw error;
      }
      // Outros erros podem indicar problema com o botão ou página
      tentativas++;
      if (tentativas >= maxTentativas) {
        throw error;
      }
    }
  }
  
  return false;
}

// Teste único contínuo cobrindo todos os tópicos do fluxo
test('Fluxo completo dos Correios - CEP inválido, CEP válido e Rastreamento', async ({ page }) => {
  // 1. Entre no site dos correios
  await page.goto('https://www.correios.com.br/');
  
  // Aguardar e clicar no botão "Aceito" do cookie banner, se existir
  try {
    const acceptButton = page.getByText('Aceito', { exact: false }).first();
    await acceptButton.waitFor({ state: 'visible', timeout: 5000 });
    await acceptButton.click();
    await page.waitForTimeout(500);
  } catch {
    // Se o botão não aparecer, continua o teste normalmente
  }
  
  // 2. Procure pelo CEP 80700000
  await page.goto('https://buscacepinter.correios.com.br/app/endereco/index.php?');
  await page.getByRole('textbox', { name: 'Digite um CEP ou um Endereço:*' }).fill('80700000');
  await page.getByRole('textbox', { name: 'Digite o texto contido na' }).dblclick();

  // Preencher captcha e buscar (máximo 3 tentativas)
  await preencherCaptchaEBuscar(page, 'Buscar', 3);
  
  // Aguardar que a resposta apareça antes de validar
  await page.waitForTimeout(3000);
  
  // Aguardar que a página processe a resposta
  try {
    await page.waitForLoadState('networkidle', { timeout: 5000 }).catch(() => {});
  } catch {
    // Continuar mesmo se networkidle não for alcançado
  }
  
  // 3. Confirmar que o CEP não existe
  await test.step('Validar que CEP 80700000 não existe', async () => {
    const mensagemSemDados = page.getByText('Não há dados a serem exibidos');
    const headingDadosNaoEncontrado = page.getByRole('heading', { name: 'Dados não encontrado' });
    
    // Aguardar que os elementos apareçam
    await mensagemSemDados.waitFor({ state: 'visible', timeout: 15000 });
    await headingDadosNaoEncontrado.waitFor({ state: 'visible', timeout: 15000 });
    
    await expect(mensagemSemDados).toBeVisible();
    await expect(headingDadosNaoEncontrado).toBeVisible();
    
    // Validar o texto das mensagens
    const textoMensagem = await mensagemSemDados.textContent();
    const textoHeading = await headingDadosNaoEncontrado.textContent();
    
    expect(textoMensagem).toContain('Não há dados a serem exibidos');
    expect(textoHeading).toContain('Dados não encontrado');
  });
  
  // 4. Voltar à tela inicial
  await page.goto('https://www.correios.com.br/');    
  await page.waitForTimeout(1500);  
  await page.goto('https://buscacepinter.correios.com.br/app/endereco/index.php?');    
  // 5. Procure pelo CEP 01013-001 (usando 01013001)
  await page.getByRole('textbox', { name: 'Digite um CEP ou um Endereço:*' }).fill('01013001');
  await page.getByRole('textbox', { name: 'Digite o texto contido na' }).dblclick();
  
  // Preencher captcha e buscar (máximo 3 tentativas)
  await preencherCaptchaEBuscar(page, 'Buscar', 3);
  
  // Aguardar que a tabela de resultados apareça
  await page.waitForTimeout(2000);
  await page.locator('table, tbody').first().waitFor({ state: 'visible', timeout: 10000 }).catch(() => {});
  
  await page.getByRole('cell', { name: 'Rua Quinze de Novembro - lado' }).dblclick();
  await page.getByRole('cell', { name: 'São Paulo/SP' }).click();
  
  // 6. Confirmar que o resultado seja em "Rua Quinze de Novembro, São Paulo/SP"
  await test.step('Validar que CEP 01013001 retorna "Rua Quinze de Novembro, São Paulo/SP"', async () => {
    const ruaCell = page.getByRole('cell', { name: /Rua Quinze de Novembro/ });
    const estadoCell = page.getByRole('cell', { name: 'São Paulo/SP' });
    
    // Verificar se os elementos estão visíveis
    await expect(ruaCell).toBeVisible();
    await expect(estadoCell).toBeVisible();
    
    // Validar o conteúdo dos campos
    const ruaText = await ruaCell.textContent();
    const estadoText = await estadoCell.textContent();
    
    expect(ruaText).toContain('Rua Quinze de Novembro');
    expect(estadoText).toBe('São Paulo/SP');
  });
  
  // 7. Voltar à tela inicial
  await page.goto('https://www.correios.com.br/');
  await page.waitForTimeout(1500);
  
  // 8. Procurar no rastreamento de código o número "SS987654321BR"
  await page.goto('https://rastreamento.correios.com.br/app/index.php');
  
  // Aguardar a página carregar completamente
  await page.waitForLoadState('networkidle');
  
  // Aguardar o campo de código de rastreio estar visível antes de preencher
  const campoRastreio = page.locator('input[type="text"]').first();
  await campoRastreio.waitFor({ state: 'visible', timeout: 15000 });
  await campoRastreio.click();
  await campoRastreio.fill('SS987654321BR');
  await campoRastreio.press('Tab');

  // Preencher captcha e consultar (máximo 3 tentativas)
  await preencherCaptchaEBuscar(page, 'Consultar', 3);
  
  // Aguardar que a resposta apareça antes de validar
  await page.waitForTimeout(2000);
  
  // 9. Confirmar que o código não está correto
  await test.step('Validar que código SS987654321BR não foi encontrado', async () => {
    const mensagemObjetoNaoEncontrado = page.getByText('Objeto não encontrado na base');
    await mensagemObjetoNaoEncontrado.waitFor({ state: 'visible', timeout: 10000 });
    await expect(mensagemObjetoNaoEncontrado).toBeVisible();
    
    const textoObjeto = await mensagemObjetoNaoEncontrado.textContent();
    expect(textoObjeto).toContain('Objeto não encontrado na base');
    
    await page.getByText('OK').click();
  });
  
  // 10. Fechar o browser (fechamento automático pelo Playwright)
});