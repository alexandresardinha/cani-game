#!/bin/bash
# Script para baixar os highlights do Instagram
# Requer: yt-dlp instalado + estar logado no Instagram no Chromium (WSL snap)
#
# Passo a passo:
#   1. chromium-browser https://www.instagram.com/accounts/login/
#   2. Faça login no Instagram
#   3. Feche o Chromium
#   4. Execute este script

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
BASE_DIR="$(dirname "$SCRIPT_DIR")"
DEST_DIR="$BASE_DIR/Characters/_raw_downloads"
HIGHLIGHT_URL="https://www.instagram.com/stories/highlights/18259015006201085/"
COOKIES_PATH="chromium:/home/asardinha/snap/chromium/common/chromium/Default"

mkdir -p "$DEST_DIR"
cd "$DEST_DIR"

echo "=== Baixando highlights do Instagram ==="
echo "Conta: @adao.superacao"
echo "Destino: $DEST_DIR"
echo ""

echo "[1/1] Usando cookies do Chromium (snap WSL)..."
yt-dlp --cookies-from-browser "$COOKIES_PATH" "$HIGHLIGHT_URL" -o "%(title)s_%(id)s.%(ext)s"

echo ""
echo "=== Download concluido ==="
echo "Arquivos baixados:"
ls -la "$DEST_DIR/"

echo ""
echo "Agora copie manualmente para as pastas de referencia ou execute:"
echo "  python3 $SCRIPT_DIR/organize_references.py"
echo "para ver a lista de arquivos baixados."
