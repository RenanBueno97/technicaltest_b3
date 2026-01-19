#!/bin/bash
# Script para executar todos os testes em ordem
# 1. xUnit (xunit-tests)
# 2. SpecFlow (specflow-tests)

echo "========================================"
echo "  EXECUTANDO TODOS OS TESTES"
echo "========================================"
echo ""

# Executa testes xUnit primeiro
echo "========================================"
echo "  1. TESTES xUNIT + SELENIUM"
echo "========================================"
echo ""

cd xunit-tests
dotnet test --logger "console;verbosity=normal"
XUNIT_RESULT=$?
cd ..

echo ""
echo "========================================"
echo "  2. TESTES SPECFLOW + SELENIUM (BDD)"
echo "========================================"
echo ""

# Executa testes SpecFlow em seguida
cd specflow-tests
dotnet test --logger "console;verbosity=normal"
SPECFLOW_RESULT=$?
cd ..

echo ""
echo "========================================"
echo "  RESUMO DOS TESTES"
echo "========================================"

if [ $XUNIT_RESULT -eq 0 ]; then
    echo "  xUnit:    APROVADO"
else
    echo "  xUnit:    FALHOU"
fi

if [ $SPECFLOW_RESULT -eq 0 ]; then
    echo "  SpecFlow: APROVADO"
else
    echo "  SpecFlow: FALHOU"
fi

echo "========================================"

# Retorna codigo de erro se algum teste falhou
if [ $XUNIT_RESULT -ne 0 ] || [ $SPECFLOW_RESULT -ne 0 ]; then
    exit 1
fi
exit 0
