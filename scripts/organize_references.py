#!/usr/bin/env python3
"""
Organiza as fotos e vídeos baixados do Instagram nas pastas de referência.
Separa cão vs corredor manualmente (depois do download, o usuário classifica).
"""

import os
import shutil
from pathlib import Path

BASE = Path("/home/asardinha/workspace/cani-game")
RAW = BASE / "Characters" / "_raw_downloads"
DOG_PHOTOS = BASE / "Characters" / "Adao_Dog" / "Reference" / "Photos"
DOG_VIDEOS = BASE / "Characters" / "Adao_Dog" / "Reference" / "Videos"
RUNNER_PHOTOS = BASE / "Characters" / "Alexandre_Runner" / "Reference" / "Photos"
RUNNER_VIDEOS = BASE / "Characters" / "Alexandre_Runner" / "Reference" / "Videos"


def main():
    if not RAW.exists():
        print(
            f"Pasta {RAW} não existe. Execute download_instagram_highlights.sh primeiro."
        )
        return

    files = list(RAW.glob("*"))
    if not files:
        print("Nenhum arquivo encontrado em _raw_downloads/")
        return

    print(f"Encontrados {len(files)} arquivos:")
    for f in sorted(files):
        size_mb = f.stat().st_size / (1024 * 1024)
        ext = f.suffix.lower()
        file_type = (
            "📷 FOTO" if ext in (".jpg", ".jpeg", ".png", ".webp") else "🎬 VÍDEO"
        )
        print(f"  {file_type} | {size_mb:.1f} MB | {f.name}")

    print(f"\nClassifique manualmente:")
    print(f"  1. Fotos/vídeos do CÃO → {DOG_PHOTOS}")
    print(f"     ou {DOG_VIDEOS}")
    print(f"  2. Fotos/vídeos do CORREDOR → {RUNNER_PHOTOS}")
    print(f"     ou {RUNNER_VIDEOS}")
    print(f"\nDepois execute:")
    print(f"  python3 scripts/extract_frames.py")


if __name__ == "__main__":
    main()
