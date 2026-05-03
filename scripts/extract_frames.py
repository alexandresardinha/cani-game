#!/usr/bin/env python3
"""
Extrai frames de vídeos de referência para criar fotos de modelagem.
Útil para capturar poses específicas do cão e corredor.
"""

import os
import cv2
from pathlib import Path

BASE = Path("/mnt/c/Users/alexa/Projetos/cani-game")


def extract_frames(video_dir, frames_dir, every_n_seconds=2):
    """Extrai frames de todos os vídeos em video_dir para frames_dir"""
    video_dir = Path(video_dir)
    frames_dir = Path(frames_dir)
    frames_dir.mkdir(parents=True, exist_ok=True)

    for video_path in video_dir.glob("*"):
        if video_path.suffix.lower() not in (".mp4", ".webm", ".mov"):
            continue

        cap = cv2.VideoCapture(str(video_path))
        fps = cap.get(cv2.CAP_PROP_FPS)
        frame_interval = int(fps * every_n_seconds)
        total = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))
        name = video_path.stem

        count = 0
        for i in range(0, total, frame_interval):
            cap.set(cv2.CAP_PROP_POS_FRAMES, i)
            ret, frame = cap.read()
            if ret:
                out = frames_dir / f"{name}_f{i:05d}.jpg"
                cv2.imwrite(str(out), frame, [cv2.IMWRITE_JPEG_QUALITY, 90])
                count += 1

        cap.release()
        print(f"  {name}: {count} frames extraídos")


def main():
    print("=== Extraindo frames do Cão Adão ===")
    extract_frames(
        BASE / "Characters/Adao_Dog/Reference/Videos",
        BASE / "Characters/Adao_Dog/Reference/Frames",
        every_n_seconds=1,  # Mais frames para capturar poses
    )

    print("=== Extraindo frames do Corredor Alexandre ===")
    extract_frames(
        BASE / "Characters/Alexandre_Runner/Reference/Videos",
        BASE / "Characters/Alexandre_Runner/Reference/Frames",
        every_n_seconds=1,
    )

    print("\nFrames extraídos! Use como referência para modelagem 3D.")


if __name__ == "__main__":
    main()
