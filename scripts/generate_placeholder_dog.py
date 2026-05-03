#!/usr/bin/env python3
"""
Gera um modelo placeholder low-poly de cachorro (Malinois) em formato .obj.
Formas geometricas simples: corpo, cabeca, focinho, 4 patas, cauda, orelhas.
O Unity importa .obj nativamente.
"""

import os


def write_box(f, cx, cy, cz, sx, sy, sz, group_name, v_offset):
    """Escreve uma caixa centrada em (cx,cy,cz) com tamanho (sx,sy,sz).
    Retorna o novo offset de vertice."""
    hx, hy, hz = sx/2, sy/2, sz/2
    verts = [
        (cx-hx, cy-hy, cz-hz), (cx+hx, cy-hy, cz-hz), (cx+hx, cy+hy, cz-hz), (cx-hx, cy+hy, cz-hz),  # frente z-
        (cx-hx, cy-hy, cz+hz), (cx+hx, cy-hy, cz+hz), (cx+hx, cy+hy, cz+hz), (cx-hx, cy+hy, cz+hz),  # tras z+
    ]
    for v in verts:
        f.write(f"v {v[0]:.4f} {v[1]:.4f} {v[2]:.4f}\n")

    f.write(f"g {group_name}\n")
    # Faces (1-indexed, com offset)
    vo = v_offset
    # frente (z-)
    f.write(f"f {vo+1} {vo+2} {vo+3} {vo+4}\n")
    # tras (z+)
    f.write(f"f {vo+5} {vo+8} {vo+7} {vo+6}\n")
    # topo
    f.write(f"f {vo+4} {vo+3} {vo+7} {vo+8}\n")
    # base
    f.write(f"f {vo+1} {vo+5} {vo+6} {vo+2}\n")
    # esquerda
    f.write(f"f {vo+1} {vo+4} {vo+8} {vo+5}\n")
    # direita
    f.write(f"f {vo+2} {vo+6} {vo+7} {vo+3}\n")

    return v_offset + 8


def generate_dog_placeholder(out_path):
    with open(out_path, 'w') as f:
        f.write("# Placeholder Low-Poly Dog (Malinois)\n")
        f.write("# Generated for Canicross Unity project\n")
        f.write("o Adao_Dog_Placeholder\n")

        vo = 0

        # Corpo principal: alongado, levemente inclinado para cima na frente
        # centro x=0, y=0.35, z=0 (meio do corpo)
        # tamanho: largura=0.22, altura=0.30, comprimento=0.65
        vo = write_box(f, 0.00, 0.38, 0.00, 0.22, 0.28, 0.62, "Body", vo)

        # Peito (parte da frente do corpo, ligeiramente mais alto)
        vo = write_box(f, 0.00, 0.42, -0.22, 0.24, 0.32, 0.22, "Chest", vo)

        # Pescoco
        vo = write_box(f, 0.00, 0.58, -0.32, 0.13, 0.18, 0.14, "Neck", vo)

        # Cabeca
        vo = write_box(f, 0.00, 0.72, -0.38, 0.16, 0.18, 0.18, "Head", vo)

        # Focinho (preto)
        vo = write_box(f, 0.00, 0.68, -0.50, 0.09, 0.09, 0.14, "Muzzle", vo)

        # Orelha esquerda
        vo = write_box(f, -0.06, 0.83, -0.38, 0.04, 0.08, 0.06, "Ear_L", vo)

        # Orelha direita
        vo = write_box(f, 0.06, 0.83, -0.38, 0.04, 0.08, 0.06, "Ear_R", vo)

        # Perna dianteira esquerda
        vo = write_box(f, -0.08, 0.16, -0.20, 0.07, 0.30, 0.07, "Leg_FL", vo)

        # Perna dianteira direita
        vo = write_box(f, 0.08, 0.16, -0.20, 0.07, 0.30, 0.07, "Leg_FR", vo)

        # Perna traseira esquerda
        vo = write_box(f, -0.08, 0.16, 0.22, 0.08, 0.30, 0.08, "Leg_BL", vo)

        # Perna traseira direita
        vo = write_box(f, 0.08, 0.16, 0.22, 0.08, 0.30, 0.08, "Leg_BR", vo)

        # Cauda (subindo levemente)
        vo = write_box(f, 0.00, 0.50, 0.42, 0.05, 0.05, 0.28, "Tail", vo)

    print(f"Modelo placeholder gerado: {out_path}")
    print(f"Total de vertices: {vo}")
    print(f"Tamanho aproximado: 0.22m largura x 0.88m altura x 1.0m comprimento")


if __name__ == "__main__":
    out = "/mnt/c/Users/alexa/Projetos/cani-game/Canicross/Assets/Art/Models/Dog_Malinois_Placeholder.obj"
    os.makedirs(os.path.dirname(out), exist_ok=True)
    generate_dog_placeholder(out)
