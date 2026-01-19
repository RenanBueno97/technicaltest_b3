# language: pt-BR
Funcionalidade: Validacao de funcionalidades do site dos Correios
    Como usuario do site dos Correios
    Eu quero buscar CEPs e rastrear objetos
    Para confirmar que as funcionalidades estao operando corretamente

    Contexto: Acessar o site dos Correios
        Dado que estou no site dos Correios
        E aceito os cookies se existirem

    @Ordem1 @CEP @Invalido @ID
    Cenario: 01 - Buscar CEP invalido deve retornar mensagem de erro
        Quando eu navego para a pagina de busca de CEP
        E preencho o campo de CEP com "80700000" usando seletor ID
        E preencho o captcha manualmente
        E clico no botao Buscar
        Entao devo ver a mensagem "Nao ha dados a serem exibidos"
        E devo ver o titulo "Dados nao encontrado"

    @Ordem2 @CEP @Valido @XPath
    Cenario: 02 - Buscar CEP valido deve retornar endereco correto
        Quando eu navego para a pagina de busca de CEP
        E preencho o campo de CEP com "01013001" usando seletor ID
        E preencho o captcha manualmente
        E clico no botao Buscar
        Entao o resultado deve conter "Rua Quinze de Novembro" usando seletor XPath
        E o resultado deve conter "Sao Paulo" usando seletor XPath

    @Ordem3 @Rastreamento @Invalido @CSS
    Cenario: 03 - Buscar rastreamento invalido deve retornar mensagem de nao encontrado
        Quando eu navego para a pagina de rastreamento
        E preencho o campo de codigo com "SS987654321BR" usando seletor CSS
        E preencho o captcha manualmente
        E clico no botao Consultar
        Entao devo ver a mensagem "Objeto nao encontrado na base"
        E clico no botao OK para fechar o modal
