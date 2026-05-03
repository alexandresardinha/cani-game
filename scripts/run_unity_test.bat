@echo off
:: Script para rodar teste automatico do Canicross no Unity
:: Otimizado para velocidade: usa -nographics em batchmode

echo ==========================================
echo  Canicross - Auto Test Runner
echo ==========================================

set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"
set PROJECT_PATH=C:\Users\alexa\Projetos\cani-game\Canicross
set RESULTS_PATH=%PROJECT_PATH%\TestResults

echo.
echo Unity: %UNITY_PATH%
echo Projeto: %PROJECT_PATH%
echo.

:: Cria pasta de resultados
mkdir "%RESULTS_PATH%" 2>nul

echo [1/3] Limpando resultados anteriores...
del /Q "%RESULTS_PATH%\*.*" 2>nul

echo [2/3] Rodando Unity em batch mode (nographics)...
%UNITY_PATH% ^
  -batchmode ^
  -nographics ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod Canicross.Editor.AutoTester.RunTest ^
  -logFile "%RESULTS_PATH%\unity_log.txt" ^
  -forgetProjectPath ^
  -quit

echo [3/3] Verificando resultados...
echo.

if exist "%RESULTS_PATH%\dog_test_result.png" (
  echo ✅ SUCESSO! Screenshot encontrado.
  echo 📁 Resultados em: %RESULTS_PATH%
  echo.
  dir /B "%RESULTS_PATH%"
) else (
  echo ❌ ERRO: Screenshot nao gerado.
  echo Verifique o log: %RESULTS_PATH%\unity_log.txt
)

echo.
echo ==========================================
echo  Fim do teste
echo ==========================================
pause