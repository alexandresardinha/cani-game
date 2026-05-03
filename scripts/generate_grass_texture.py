#!/usr/bin/env python3
"""
Gera uma textura de grama procedural simples.
Usa ruído para criar variação de cor verde/marrom.
"""

import numpy as np
from PIL import Image
import os

def generate_grass_texture(size=512):
    """Gera textura de grama com variação de cor."""
    img = np.zeros((size, size, 3), dtype=np.uint8)
    
    # Cores base da grama
    base_green = np.array([34, 139, 34])  # Forest green
    dark_green = np.array([20, 80, 20])   # Verde escuro
    light_green = np.array([60, 180, 60]) # Verde claro
    brown = np.array([139, 119, 101])     # Marrom seco
    
    # Gera ruído simples
    np.random.seed(42)
    
    for y in range(size):
        for x in range(size):
            # Ruído para variação
            noise = np.random.random()
            
            # Mistura de cores baseada no ruído
            if noise < 0.3:
                color = dark_green
            elif noise < 0.6:
                color = base_green
            elif noise < 0.85:
                color = light_green
            else:
                color = brown
            
            # Adiciona variação fina
            variation = np.random.randint(-15, 15, 3)
            pixel = np.clip(color + variation, 0, 255)
            
            img[y, x] = pixel
    
    return Image.fromarray(img)


def main():
    out_dir = "/mnt/c/Users/alexa/Projetos/cani-game/Canicross/Assets/Art/Textures"
    os.makedirs(out_dir, exist_ok=True)
    
    img = generate_grass_texture(512)
    out_path = os.path.join(out_dir, "Grass_Procedural.png")
    img.save(out_path)
    
    print(f"Textura de grama gerada: {out_path}")
    print(f"Tamanho: 512x512")
    print("Use como textura do chao no Unity")


if __name__ == "__main__":
    main()
