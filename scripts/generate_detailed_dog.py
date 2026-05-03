#!/usr/bin/env python3
"""
Gera um modelo 3D low-poly detalhado do Adão (vira-lata caramelo)
Baseado na descrição visual: porte médio, pelagem curta caramelo, orelhas erguidas,
focinho alongado, cauda média, peito profundo, patas longas.

Usa formas paramétricas: elipsoides, cilindros, cones, pirâmides.
"""

import os
import math
from pathlib import Path
import numpy as np


class MeshBuilder:
    """Acumulador de mesh para exportar .obj"""
    def __init__(self):
        self.verts = []
        self.normals = []
        self.uvs = []
        self.faces = []
        self.v_offset = 1  # OBJ é 1-indexed

    def add_mesh(self, verts, faces, uvs=None, normals=None):
        """Adiciona uma sub-mesh. verts: list[(x,y,z)], faces: list[(i,j,k)]"""
        n = len(self.verts)
        self.verts.extend(verts)
        if uvs:
            self.uvs.extend(uvs)
        if normals:
            self.normals.extend(normals)
        for f in faces:
            self.faces.append(tuple(v + n + 1 for v in f))
        self.v_offset += len(verts)

    def write_obj(self, path):
        with open(path, 'w') as f:
            f.write("# Adao - Vira-lata Caramelo (Low-Poly)\n")
            f.write("# Gerado proceduralmente para Canicross\n\n")
            for v in self.verts:
                f.write(f"v {v[0]:.6f} {v[1]:.6f} {v[2]:.6f}\n")
            f.write(f"# {len(self.verts)} vertices\n\n")
            if self.normals:
                for n in self.normals:
                    f.write(f"vn {n[0]:.6f} {n[1]:.6f} {n[2]:.6f}\n")
            if self.uvs:
                for uv in self.uvs:
                    f.write(f"vt {uv[0]:.6f} {uv[1]:.6f}\n")
            for face in self.faces:
                if self.normals and self.uvs:
                    f.write(f"f {' '.join(f'{v}/{v}/{v}' for v in face)}\n")
                elif self.normals:
                    f.write(f"f {' '.join(f'{v}//{v}' for v in face)}\n")
                else:
                    f.write(f"f {' '.join(str(v) for v in face)}\n")


def normalize(v):
    v = np.array(v, dtype=float)
    n = np.linalg.norm(v)
    return v / n if n > 0 else v


def make_sphere(cx, cy, cz, rx, ry, rz, segments=12, rings=8):
    """Elipsoide centrado em (cx,cy,cz) com raios rx,ry,rz."""
    verts = []
    faces = []

    for r in range(rings + 1):
        theta = math.pi * r / rings
        sin_theta = math.sin(theta)
        cos_theta = math.cos(theta)
        for s in range(segments):
            phi = 2 * math.pi * s / segments
            sin_phi = math.sin(phi)
            cos_phi = math.cos(phi)

            x = cx + rx * sin_theta * cos_phi
            y = cy + ry * cos_theta
            z = cz + rz * sin_theta * sin_phi
            verts.append((x, y, z))

    # Faces
    for r in range(rings):
        for s in range(segments):
            v0 = r * segments + s
            v1 = r * segments + (s + 1) % segments
            v2 = (r + 1) * segments + (s + 1) % segments
            v3 = (r + 1) * segments + s
            faces.append((v0, v1, v2, v3))

    return verts, faces


def make_cylinder(cx, cy, cz, r_bottom, r_top, height, segments=10,
                  rot_axis=(0,1,0), rot_angle=0):
    """Cilindro/cone com base em y=cy e topo em y=cy+height.
    Pode ser rotacionado."""
    verts = []
    faces = []

    # Eixo de rotação
    axis = np.array(rot_axis, dtype=float)
    axis = axis / np.linalg.norm(axis)
    angle = math.radians(rot_angle)

    def rotate_point(p, center):
        p = np.array(p) - np.array(center)
        # Rodrigues rotation formula
        cos_a = math.cos(angle)
        sin_a = math.sin(angle)
        p_rot = p * cos_a + np.cross(axis, p) * sin_a + axis * np.dot(axis, p) * (1 - cos_a)
        return tuple(p_rot + np.array(center))

    # Base
    base_idx = len(verts)
    for s in range(segments):
        phi = 2 * math.pi * s / segments
        x = cx + r_bottom * math.cos(phi)
        z = cz + r_bottom * math.sin(phi)
        y = cy
        verts.append(rotate_point((x, y, z), (cx, cy, cz)))

    # Topo
    top_idx = len(verts)
    for s in range(segments):
        phi = 2 * math.pi * s / segments
        x = cx + r_top * math.cos(phi)
        z = cz + r_top * math.sin(phi)
        y = cy + height
        verts.append(rotate_point((x, y, z), (cx, cy, cz)))

    # Faces laterais
    for s in range(segments):
        s1 = (s + 1) % segments
        v0 = base_idx + s
        v1 = base_idx + s1
        v2 = top_idx + s1
        v3 = top_idx + s
        faces.append((v0, v1, v2, v3))

    # Tampas
    base_center = rotate_point((cx, cy, cz), (cx, cy, cz))
    top_center = rotate_point((cx, cy + height, cz), (cx, cy, cz))

    verts.append(base_center)
    bc_idx = len(verts) - 1
    verts.append(top_center)
    tc_idx = len(verts) - 1

    for s in range(segments):
        s1 = (s + 1) % segments
        faces.append((bc_idx, base_idx + s1, base_idx + s))
        faces.append((tc_idx, top_idx + s, top_idx + s1))

    return verts, faces


def make_pyramid(cx, cy, cz, width, height, depth, tip_offset=(0,0,0)):
    """Pirâmide de base quadrada com topo deslocado."""
    verts = []
    faces = []

    hw, hd = width/2, depth/2
    # Base
    verts.extend([
        (cx - hw, cy, cz - hd),
        (cx + hw, cy, cz - hd),
        (cx + hw, cy, cz + hd),
        (cx - hw, cy, cz + hd),
    ])
    # Topo
    tip = (cx + tip_offset[0], cy + height, cz + tip_offset[2])
    verts.append(tip)

    # Faces
    faces.extend([
        (0, 1, 2, 3),       # base
        (0, 4, 1),          # frente
        (1, 4, 2),          # direita
        (2, 4, 3),          # tras
        (3, 4, 0),          # esquerda
    ])

    return verts, faces


def make_torus(cx, cy, cz, major_r, minor_r, segments=16, rings=8):
    """Toro (anel) para coleira."""
    verts = []
    faces = []

    for s in range(segments):
        theta = 2 * math.pi * s / segments
        cos_t = math.cos(theta)
        sin_t = math.sin(theta)
        center_x = cx + major_r * cos_t
        center_z = cz + major_r * sin_t

        for r in range(rings):
            phi = 2 * math.pi * r / rings
            cos_p = math.cos(phi)
            sin_p = math.sin(phi)
            x = center_x + minor_r * cos_p * cos_t
            y = cy + minor_r * sin_p
            z = center_z + minor_r * cos_p * sin_t
            verts.append((x, y, z))

    for s in range(segments):
        for r in range(rings):
            v0 = s * rings + r
            v1 = s * rings + (r + 1) % rings
            v2 = ((s + 1) % segments) * rings + (r + 1) % rings
            v3 = ((s + 1) % segments) * rings + r
            faces.append((v0, v1, v2, v3))

    return verts, faces


def generate_adao_dog():
    """Gera o mesh completo do Adão."""
    mesh = MeshBuilder()

    # --- CORPO PRINCIPAL ---
    # Elipsoide alongado: torácico (peito profundo)
    v, f = make_sphere(0.00, 0.38, 0.00, 0.13, 0.16, 0.32, segments=14, rings=10)
    mesh.add_mesh(v, f)

    # Peito (parte frontal levemente mais larga)
    v, f = make_sphere(0.00, 0.36, -0.18, 0.14, 0.14, 0.15, segments=12, rings=8)
    mesh.add_mesh(v, f)

    # Abdômen (parte traseira mais fina)
    v, f = make_sphere(0.00, 0.35, 0.18, 0.11, 0.12, 0.18, segments=12, rings=8)
    mesh.add_mesh(v, f)

    # --- PESCOÇO ---
    # Inclinado para cima/frente
    v, f = make_cylinder(0.00, 0.50, -0.32, 0.065, 0.08, 0.14,
                         segments=10, rot_axis=(1,0,0), rot_angle=-25)
    mesh.add_mesh(v, f)

    # --- CABEÇA ---
    # Crânio
    v, f = make_sphere(0.00, 0.72, -0.42, 0.10, 0.11, 0.11, segments=12, rings=10)
    mesh.add_mesh(v, f)

    # --- FOCINHO (alongado, vira-lata) ---
    v, f = make_cylinder(0.00, 0.68, -0.54, 0.04, 0.02, 0.14,
                         segments=8, rot_axis=(1,0,0), rot_angle=-10)
    mesh.add_mesh(v, f)

    # Nariz
    v, f = make_sphere(0.00, 0.67, -0.62, 0.02, 0.015, 0.02, segments=8, rings=6)
    mesh.add_mesh(v, f)

    # --- ORELHAS (eretas, triangulares, vira-lata) ---
    # Esquerda
    v, f = make_pyramid(-0.08, 0.78, -0.42, 0.05, 0.08, 0.03, tip_offset=(0.01, 0, 0))
    mesh.add_mesh(v, f)
    # Direita
    v, f = make_pyramid(0.08, 0.78, -0.42, 0.05, 0.08, 0.03, tip_offset=(-0.01, 0, 0))
    mesh.add_mesh(v, f)

    # --- OLHOS (pequenas esferas salientes) ---
    v, f = make_sphere(-0.04, 0.73, -0.48, 0.018, 0.018, 0.015, segments=8, rings=6)
    mesh.add_mesh(v, f)
    v, f = make_sphere(0.04, 0.73, -0.48, 0.018, 0.018, 0.015, segments=8, rings=6)
    mesh.add_mesh(v, f)

    # --- PATAS DIANTEIRAS ---
    # Perna esquerda (ombro + antebraco + pata)
    # Ombro
    v, f = make_sphere(-0.08, 0.32, -0.18, 0.05, 0.06, 0.05, segments=8, rings=6)
    mesh.add_mesh(v, f)
    # Braço
    v, f = make_cylinder(-0.08, 0.10, -0.18, 0.04, 0.035, 0.22, segments=8)
    mesh.add_mesh(v, f)
    # Pata
    v, f = make_sphere(-0.08, 0.04, -0.18, 0.035, 0.025, 0.04, segments=8, rings=6)
    mesh.add_mesh(v, f)

    # Perna direita
    v, f = make_sphere(0.08, 0.32, -0.18, 0.05, 0.06, 0.05, segments=8, rings=6)
    mesh.add_mesh(v, f)
    v, f = make_cylinder(0.08, 0.10, -0.18, 0.04, 0.035, 0.22, segments=8)
    mesh.add_mesh(v, f)
    v, f = make_sphere(0.08, 0.04, -0.18, 0.035, 0.025, 0.04, segments=8, rings=6)
    mesh.add_mesh(v, f)

    # --- PATAS TRASEIRAS (mais musculosas) ---
    # Coxa esquerda
    v, f = make_sphere(-0.09, 0.35, 0.20, 0.06, 0.08, 0.07, segments=8, rings=6)
    mesh.add_mesh(v, f)
    # Perna
    v, f = make_cylinder(-0.09, 0.10, 0.22, 0.045, 0.04, 0.24, segments=8)
    mesh.add_mesh(v, f)
    # Pata
    v, f = make_sphere(-0.09, 0.04, 0.22, 0.035, 0.025, 0.04, segments=8, rings=6)
    mesh.add_mesh(v, f)

    # Coxa direita
    v, f = make_sphere(0.09, 0.35, 0.20, 0.06, 0.08, 0.07, segments=8, rings=6)
    mesh.add_mesh(v, f)
    v, f = make_cylinder(0.09, 0.10, 0.22, 0.045, 0.04, 0.24, segments=8)
    mesh.add_mesh(v, f)
    v, f = make_sphere(0.09, 0.04, 0.22, 0.035, 0.025, 0.04, segments=8, rings=6)
    mesh.add_mesh(v, f)

    # --- CAUDA (curva para cima) ---
    # Segmentos de esfera para curva natural
    tail_segments = [
        (0.00, 0.48, 0.42, 0.035),
        (0.00, 0.52, 0.48, 0.03),
        (0.00, 0.55, 0.53, 0.025),
        (0.00, 0.56, 0.57, 0.02),
    ]
    for tx, ty, tz, tr in tail_segments:
        v, f = make_sphere(tx, ty, tz, tr, tr, tr, segments=8, rings=6)
        mesh.add_mesh(v, f)

    # --- COLEIRA (anel azul) ---
    v, f = make_torus(0.00, 0.55, -0.30, 0.09, 0.012, segments=16, rings=6)
    mesh.add_mesh(v, f)

    # --- BOCHECHAS (leve saliência) ---
    v, f = make_sphere(-0.05, 0.69, -0.46, 0.025, 0.02, 0.02, segments=6, rings=4)
    mesh.add_mesh(v, f)
    v, f = make_sphere(0.05, 0.69, -0.46, 0.025, 0.02, 0.02, segments=6, rings=4)
    mesh.add_mesh(v, f)

    return mesh


def main():
    out_dir = Path("/mnt/c/Users/alexa/Projetos/cani-game/Canicross/Assets/Art/Models")
    out_dir.mkdir(parents=True, exist_ok=True)

    mesh = generate_adao_dog()

    out_path = out_dir / "Dog_Malinois_Detailed.obj"
    mesh.write_obj(str(out_path))

    print(f"=== Modelo do Adao gerado ===")
    print(f"Arquivo: {out_path}")
    print(f"Vertices: {len(mesh.verts)}")
    print(f"Faces: {len(mesh.faces)}")
    print(f"")
    print(f"Dimensoes aproximadas:")
    print(f"  Comprimento: ~1.2m (focinho a cauda)")
    print(f"  Altura: ~0.85m (solo a orelha)")
    print(f"  Largura: ~0.26m (ombro a ombro)")
    print(f"")
    print(f"Estrutura:")
    print(f"  - Corpo: elipsoide toracico + peito + abdomen")
    print(f"  - Pescoco: cilindro inclinado")
    print(f"  - Cabeca: esfera com focinho alongado (vira-lata)")
    print(f"  - Orelhas: piramides eretas")
    print(f"  - Olhos: esferas salientes")
    print(f"  - 4 Patas: articulacoes + cilindros")
    print(f"  - Cauda: curva com 4 segmentos")
    print(f"  - Coleira: toro azul")
    print(f"")
    print(f"Para usar no Unity:")
    print(f"1. Importe o .obj no projeto")
    print(f"2. Crie um material 'Dog_Caramel_Mat' com cor #D4A04A")
    print(f"3. Crie um prefab com Rigidbody, BoxCollider e DogController")


if __name__ == "__main__":
    main()
