using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SpecFlowTests.Support;

/// <summary>
/// Ordena os testes pelo nome do cenário (alfabeticamente)
/// Como os cenários começam com "01 -", "02 -", "03 -", isso garante a ordem correta
/// </summary>
public class ScenarioOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        return testCases.OrderBy(tc => tc.DisplayName);
    }
}
