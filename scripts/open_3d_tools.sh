#!/bin/bash
# Abre as ferramentas de Photo-to-3D gratuitas no Chrome do Windows

echo "=== Abrindo ferramentas gratuitas de Photo-to-3D ==="
echo ""

# 1. Hunyuan3D-2 (Tencent) - Melhor qualidade, gratuito, exporta .obj
"/mnt/c/Program Files/Google/Chrome/Application/chrome.exe" \
    "https://huggingface.co/spaces/tencent/Hunyuan3D-2" &

echo "[1/2] Hunyuan3D-2 aberto no Chrome do Windows"
echo "    - Gratuito, exporta .obj/.glb"
echo "    - Melhor para animais e personagens"
echo ""

sleep 3

# 2. InstantMesh (TencentARC) - Alternativa rápida
"/mnt/c/Program Files/Google/Chrome/Application/chrome.exe" \
    "https://huggingface.co/spaces/TencentARC/InstantMesh" &

echo "[2/2] InstantMesh aberto no Chrome do Windows"
echo "    - Gratuito, exporta .obj"
echo "    - Mais rápido, qualidade um pouco menor"
echo ""

echo "=== Instruções ==="
echo "1. No Hunyuan3D-2, clique na aba 'Image-to-3D'"
echo "2. Arraste 1 foto do Adão (frente ou lado)"
echo "3. Clique 'Generate' e aguarde ~30 segundos"
echo "4. Clique no botão de download para baixar o .obj/.glb"
echo ""
echo "=== Dica de prompt (se pedir) ==="
echo "   'A medium-sized caramel-colored short-haired dog, pointed ears, running pose'"
echo ""
echo "Fotos do Adão prontas em:"
echo "  C:\\Users\\alexa\\Projetos\\cani-game\\Characters\\Adao_Dog\\Reference\\Selected_For_AI\\"
