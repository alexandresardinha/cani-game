@echo off
:: Canicross - Visual Test Loop
:: Runs Unity visual tests in a loop until all issues are resolved

echo ==========================================
echo  Canicross - Visual Test Loop
echo ==========================================

set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\6000.4.5f1\Editor\Unity.exe"
set PROJECT_PATH=C:\Users\alexa\Projetos\cani-game\Canicross
set PYTHON_PATH=python

set LOOP_COUNT=0
set MAX_LOOPS=5

:LOOP
set /a LOOP_COUNT+=1
echo.
echo === Loop %LOOP_COUNT% / %MAX_LOOPS% ===
echo.

:: Clean previous test results
echo [1/4] Limpando resultados anteriores...
rmdir /S /Q "%PROJECT_PATH%\..\TestResults\Visual" 2>nul
mkdir "%PROJECT_PATH%\..\TestResults\Visual" 2>nul

:: Run Unity visual tests
echo [2/4] Rodando testes visuais no Unity...
%UNITY_PATH% ^
  -batchmode ^
  -nographics ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod Canicross.Editor.VisualTester.RunVisualTests ^
  -logFile "%PROJECT_PATH%\..\TestResults\unity_visual_log.txt" ^
  -forgetProjectPath ^
  -quit

if errorlevel 1 (
    echo ERRO: Unity retornou erro.
    goto :END
)

:: Analyze results
echo [3/4] Analisando screenshots...
%PYTHON_PATH% "%PROJECT_PATH%\..\scripts\analyze_visual_tests.py"

if errorlevel 1 (
    echo [4/4] Problemas encontrados! Tentando correcoes automaticas...
    
    :: Try automatic fixes
    %PYTHON_PATH% "%PROJECT_PATH%\..\scripts\auto_fix_visual.py"
    
    if %LOOP_COUNT% GEQ %MAX_LOOPS% (
        echo.
        echo LIMITE DE TENTATIVAS ATINGIDO (%MAX_LOOPS%).
        echo Verifique os problemas manualmente em:
        echo   %PROJECT_PATH%\..\TestResults\Visual\visual_report.html
        goto :END
    )
    
    echo.
    echo Aguardando 3 segundos antes da proxima tentativa...
    timeout /t 3 /nobreak >nul
    goto :LOOP
) else (
    echo [4/4] TODOS OS TESTES PASSARAM!
    echo.
    echo Relatorio visual gerado em:
    echo   %PROJECT_PATH%\..\TestResults\Visual\visual_report.html
    goto :END
)

:END
echo.
echo ==========================================
echo  Fim do loop de testes
echo ==========================================
pause
