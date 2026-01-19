# Script para executar todos os testes em ordem
# 1. xUnit (xunit-tests)
# 2. SpecFlow (specflow-tests)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EXECUTANDO TODOS OS TESTES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Executa testes xUnit primeiro
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "  1. TESTES xUNIT + SELENIUM" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

Set-Location -Path "xunit-tests"
dotnet test --logger "console;verbosity=normal"
$xunitResult = $LASTEXITCODE

Set-Location -Path ".."

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "  2. TESTES SPECFLOW + SELENIUM (BDD)" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""

# Executa testes SpecFlow em seguida
Set-Location -Path "specflow-tests"
dotnet test --logger "console;verbosity=normal"
$specflowResult = $LASTEXITCODE

Set-Location -Path ".."

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RESUMO DOS TESTES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($xunitResult -eq 0) {
    Write-Host "  xUnit:    APROVADO" -ForegroundColor Green
} else {
    Write-Host "  xUnit:    FALHOU" -ForegroundColor Red
}

if ($specflowResult -eq 0) {
    Write-Host "  SpecFlow: APROVADO" -ForegroundColor Green
} else {
    Write-Host "  SpecFlow: FALHOU" -ForegroundColor Red
}

Write-Host "========================================" -ForegroundColor Cyan

# Retorna codigo de erro se algum teste falhou
if ($xunitResult -ne 0 -or $specflowResult -ne 0) {
    exit 1
}
exit 0
