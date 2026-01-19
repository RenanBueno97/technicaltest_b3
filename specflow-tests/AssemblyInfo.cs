using Xunit;

// Aplica o orderer customizado para ordenar os testes pelo nome (01, 02, 03)
[assembly: TestCaseOrderer("SpecFlowTests.Support.ScenarioOrderer", "SpecFlowTests")]
