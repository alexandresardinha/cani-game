#!/bin/bash
# Script para baixar os highlights do Instagram
# Requer: yt-dlp instalado + estar logado no Instagram no Chrome/Firefox

set -e

DEST_DIR="/home/asardinha/workspace/cani-game/Characters/_raw_downloads"
HIGHLIGHT_URL="https://www.instagram.com/stories/highlights/18259015006201085/"

mkdir -p "$DEST_DIR"
cd "$DEST_DIR"

echo "=== Baixando highlights do Instagram ==="
echo "Conta: @adao.superacao"
echo "Destino: $DEST_DIR"
echo ""

# Tenta usar cookies do navegador (Windows - WSL)
echo "[1/3] Tentando Chrome..."
yt-dlp --cookies-from-browser chrome "$HIGHLIGHT_URL" -o "%(title)s_%(id)s.%(ext)s" 2>&1 || true

echo "[2/3] Tentando Firefox..."
yt-dlp --cookies-from-browser firefox "$HIGHLIGHT_URL" -o "%(title)s_%(id)s.%(ext)s" 2>&1 || true

echo ""
echo "=== Download concluído ==="
echo "Arquivos baixados:"
ls -la "$DEST_DIR/"

echo ""
echo "Agora execute:"
echo "  python3 /home/asardinha/workspace/cani-game/scripts/organize_references.py"
echo "para organizar as fotos/vídeos nas pastas corretas"
