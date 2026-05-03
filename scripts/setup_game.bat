@echo off
:: Script rapido para setup rapido do Canicross
:: Pulando importacoes desnecessarias

echo ==========================================
echo  Canicross - Quick Setup
echo ==========================================

set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"
set PROJECT_PATH=C:\Users\alexa\Projetos\cani-game\Canicross

echo.
echo Abrindo Unity com cena configurada...
echo.

%UNITY_PATH% ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod Canicross.Editor.GameSetupWizard.SetupGameScene

echo.
echo Setup completo! Abra o Unity Editor para jogar.
pause