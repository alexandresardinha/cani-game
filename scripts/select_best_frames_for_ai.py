#!/usr/bin/env python3
"""
Seleciona os melhores frames do Adão baseado em análise de cor.
Objetivo: encontrar frames com alta proporção de tons caramelo/dourado/marrom
(característicos do pelo do Adão) e baixa proporção de verde/azul (paisagem).
"""

import os
import shutil
from pathlib import Path
import cv2
import numpy as np

FRAMES_DIR = Path("/mnt/c/Users/alexa/Projetos/cani-game/Characters/Adao_Dog/Reference/Frames")
OUTPUT_DIR = Path("/mnt/c/Users/alexa/Projetos/cani-game/Characters/Adao_Dog/Reference/Selected_For_AI")


def analyze_frame_color(frame_path):
    """
    Analisa a proporção de cores em uma imagem.
    Retorna dict com scores.
    """
    img = cv2.imread(str(frame_path))
    if img is None:
        return None

    img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    h, w = img.shape[:2]
    total_pixels = h * w

    # Converte para HSV para análise de cor mais robusta
    hsv = cv2.cvtColor(img, cv2.COLOR_RGB2HSV)

    # Define faixas de cor no espaço HSV
    # Caramelo/Dourado/Marrom claro: H ~ 10-35, S moderado, V alto
    lower_caramel = np.array([10, 40, 80])
    upper_caramel = np.array([35, 255, 255])
    mask_caramel = cv2.inRange(hsv, lower_caramel, upper_caramel)
    caramel_ratio = np.sum(mask_caramel > 0) / total_pixels

    # Marrom escuro: H ~ 5-25, S moderado-alto, V médio
    lower_brown = np.array([5, 50, 30])
    upper_brown = np.array([25, 255, 120])
    mask_brown = cv2.inRange(hsv, lower_brown, upper_brown)
    brown_ratio = np.sum(mask_brown > 0) / total_pixels

    # Verde (paisagem): H ~ 35-85
    lower_green = np.array([35, 30, 30])
    upper_green = np.array([85, 255, 255])
    mask_green = cv2.inRange(hsv, lower_green, upper_green)
    green_ratio = np.sum(mask_green > 0) / total_pixels

    # Azul (céu/água): H ~ 90-130
    lower_blue = np.array([90, 30, 30])
    upper_blue = np.array([130, 255, 255])
    mask_blue = cv2.inRange(hsv, lower_blue, upper_blue)
    blue_ratio = np.sum(mask_blue > 0) / total_pixels

    # Cinza/asfalto: baixa saturação
    gray_mask = hsv[:, :, 1] < 30
    gray_ratio = np.sum(gray_mask) / total_pixels

    # Score composto: queremos alto caramelo/marrom e baixo verde/azul/cinza
    dog_color_score = caramel_ratio + brown_ratio * 0.5
    landscape_penalty = green_ratio + blue_ratio + gray_ratio * 0.5

    # Bônus para imagens com foco (menos borradas = maior variância de Laplacian)
    gray = cv2.cvtColor(img, cv2.COLOR_RGB2GRAY)
    laplacian_var = cv2.Laplacian(gray, cv2.CV_64F).var()
    focus_score = min(laplacian_var / 500, 1.0)  # normaliza

    final_score = dog_color_score * 2.0 - landscape_penalty * 0.8 + focus_score * 0.3

    return {
        "path": str(frame_path),
        "name": frame_path.name,
        "caramel_ratio": caramel_ratio,
        "brown_ratio": brown_ratio,
        "green_ratio": green_ratio,
        "blue_ratio": blue_ratio,
        "gray_ratio": gray_ratio,
        "focus_score": focus_score,
        "dog_color_score": dog_color_score,
        "landscape_penalty": landscape_penalty,
        "final_score": final_score,
        "height": h,
        "width": w,
    }


def main():
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

    frames = sorted(FRAMES_DIR.glob("*.jpg"))
    print(f"Analisando {len(frames)} frames...")

    results = []
    for i, frame_path in enumerate(frames):
        if i % 50 == 0:
            print(f"  {i}/{len(frames)}...")
        result = analyze_frame_color(frame_path)
        if result:
            results.append(result)

    # Ordena pelo score final (maior = mais provável de ter o Adão)
    results.sort(key=lambda x: x["final_score"], reverse=True)

    print("\n=== TOP 20 FRAMES (maior score de caramelo + menor paisagem) ===")
    for i, r in enumerate(results[:20]):
        print(f"{i+1:2d}. {r['name']}")
        print(f"    Caramelo: {r['caramel_ratio']:.1%} | Marrom: {r['brown_ratio']:.1%} | "
              f"Verde: {r['green_ratio']:.1%} | Azul: {r['blue_ratio']:.1%} | "
              f"Foco: {r['focus_score']:.2f} | Score: {r['final_score']:.3f}")

    # Seleciona os top N frames, tentando diversificar por vídeo
    selected = []
    video_sources = set()

    for r in results:
        video_name = r["name"].split("_f")[0]
        # Pega no máximo 2 frames por vídeo para diversidade
        video_count = sum(1 for s in selected if s["name"].split("_f")[0] == video_name)

        if video_count < 2:
            selected.append(r)

        if len(selected) >= 12:
            break

    # Agora vamos separar os melhores por ângulo estimado (baseado na proporção de cor)
    # Frames com mais marrom + menos verde = mais próximo/frente
    # Frames com mais caramelo + mais verde = mais distância/lado

    # Simplesmente pega os top 8 mais diversos
    final_selection = selected[:8]

    print(f"\n=== SELECIONADOS {len(final_selection)} FRAMES PARA AI ===")
    for i, r in enumerate(final_selection):
        print(f"{i+1}. {r['name']} (score: {r['final_score']:.3f})")
        src = Path(r["path"])
        dst = OUTPUT_DIR / f"{i+1:02d}_{r['name']}"
        shutil.copy2(src, dst)
        print(f"   -> Copiado para: {dst.name}")

    print(f"\nTodos os frames selecionados estão em: {OUTPUT_DIR}")
    print("Próximo passo: suba essas fotos no Meshy.ai ou Tripo3D")


if __name__ == "__main__":
    main()
