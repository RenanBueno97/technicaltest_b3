@echo off
REM Script para executar todos os testes em ordem
REM 1. xUnit (xunit-tests)
REM 2. SpecFlow (specflow-tests)

echo ========================================
echo   EXECUTANDO TODOS OS TESTES
echo ========================================
echo.

REM Executa testes xUnit primeiro
echo ========================================
echo   1. TESTES xUNIT + SELENIUM
echo ========================================
echo.

cd xunit-tests
call dotnet test --logger "console;verbosity=normal"
set XUNIT_RESULT=%ERRORLEVEL%
cd ..

echo.
echo ========================================
echo   2. TESTES SPECFLOW + SELENIUM (BDD)
echo ========================================
echo.

REM Executa testes SpecFlow em seguida
cd specflow-tests
call dotnet test --logger "console;verbosity=normal"
set SPECFLOW_RESULT=%ERRORLEVEL%
cd ..

echo.
echo ========================================
echo   RESUMO DOS TESTES
echo ========================================

if %XUNIT_RESULT%==0 (
    echo   xUnit:    APROVADO
) else (
    echo   xUnit:    FALHOU
)

if %SPECFLOW_RESULT%==0 (
    echo   SpecFlow: APROVADO
) else (
    echo   SpecFlow: FALHOU
)

echo ========================================

REM Retorna codigo de erro se algum teste falhou
if %XUNIT_RESULT% NEQ 0 exit /b 1
if %SPECFLOW_RESULT% NEQ 0 exit /b 1
exit /b 0
