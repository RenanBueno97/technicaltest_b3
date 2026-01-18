# language: pt-BR
Funcionalidade: Validação de CEP e Rastreamento dos Correios
    Como usuário do site dos Correios
    Eu quero consultar CEPs e rastrear encomendas
    Para obter informações de endereços e status de entregas

Contexto: Usuário acessa o site dos Correios
    Dado que eu estou no site dos Correios
    E eu aceito os cookies se aparecerem

@CEP @Negativo
Cenário: Consultar CEP inválido deve retornar mensagem de erro
    Quando eu navego para a página de busca de CEP
    E eu preencho o CEP "80700000"
    E eu preencho o captcha manualmente
    E eu clico no botão de buscar
    Então devo ver a mensagem "Não há dados a serem exibidos"
    E devo ver o título "Dados não encontrado"

@CEP @Positivo
Cenário: Consultar CEP válido deve retornar o endereço correto
    Quando eu navego para a página de busca de CEP
    E eu preencho o CEP "01013001"
    E eu preencho o captcha manualmente
    E eu clico no botão de buscar
    Então devo ver o logradouro "Rua Quinze de Novembro"
    E devo ver a cidade/estado "São Paulo/SP"

@Rastreamento @Negativo
Cenário: Consultar código de rastreamento inválido deve retornar mensagem de erro
    Quando eu navego para a página de rastreamento
    E eu preencho o código de rastreio "SS987654321BR"
    E eu preencho o captcha manualmente
    E eu clico no botão de consultar
    Então devo ver a mensagem "Objeto não encontrado na base"

@FluxoCompleto @E2E
Cenário: Fluxo completo - CEP inválido, CEP válido e Rastreamento inválido
    # Parte 1: CEP Inválido
    Quando eu navego para a página de busca de CEP
    E eu preencho o CEP "80700000"
    E eu preencho o captcha manualmente
    E eu clico no botão de buscar
    Então devo ver a mensagem "Não há dados a serem exibidos"
    
    # Parte 2: Voltar e consultar CEP Válido
    Dado que eu volto para a página inicial
    Quando eu navego para a página de busca de CEP
    E eu preencho o CEP "01013001"
    E eu preencho o captcha manualmente
    E eu clico no botão de buscar
    Então devo ver o logradouro "Rua Quinze de Novembro"
    E devo ver a cidade/estado "São Paulo/SP"
    
    # Parte 3: Voltar e consultar Rastreamento Inválido
    Dado que eu volto para a página inicial
    Quando eu navego para a página de rastreamento
    E eu preencho o código de rastreio "SS987654321BR"
    E eu preencho o captcha manualmente
    E eu clico no botão de consultar
    Então devo ver a mensagem "Objeto não encontrado na base"

@CEP @DataDriven
Esquema do Cenário: Consultar diferentes CEPs
    Quando eu navego para a página de busca de CEP
    E eu preencho o CEP "<cep>"
    E eu preencho o captcha manualmente
    E eu clico no botão de buscar
    Então o resultado deve ser "<resultado>"

    Exemplos:
        | cep      | resultado                                        |
        | 80700000 | Não há dados a serem exibidos                    |
        | 01013001 | Rua Quinze de Novembro - São Paulo/SP            |
        | 00000000 | Não há dados a serem exibidos                    |
