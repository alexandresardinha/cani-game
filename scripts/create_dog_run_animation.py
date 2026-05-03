#!/usr/bin/env python3
"""
Cria uma animação simples de cachorro correndo no Blender.
Anima as pernas do modelo Jack Russell.
"""

import bpy
import math

def create_dog_run_animation(blend_file, output_fbx):
    # Abre o arquivo
    bpy.ops.wm.open_mainfile(filepath=blend_file)
    
    # Seleciona o armature
    armature = bpy.data.objects.get("Dog RIG")
    if not armature:
        print("ERRO: Armature 'Dog RIG' nao encontrado!")
        return False
    
    # Entra no modo pose
    bpy.context.view_layer.objects.active = armature
    bpy.ops.object.mode_set(mode='POSE')
    
    # Bones das pernas para animar
    leg_bones = [
        "front humerus.L",   # Perna dianteira esquerda (ombro)
        "front humerus.R",   # Perna dianteira direita
        "femur.L",           # Perna traseira esquerda (coxa)
        "femur.R",           # Perna traseira direita
    ]
    
    # Verifica se todos os bones existem
    for bone_name in leg_bones:
        if bone_name not in armature.pose.bones:
            print(f"AVISO: Bone '{bone_name}' nao encontrado!")
    
    # Configura animação
    fps = 30
    bpy.context.scene.frame_start = 1
    bpy.context.scene.frame_end = 30
    bpy.context.scene.render.fps = fps
    
    # Animação de corrida — alternância de pernas
    for frame in range(1, 31):
        bpy.context.scene.frame_set(frame)
        
        # Fase da corrida (0 a 2*pi)
        phase = (frame / 30.0) * 2 * math.pi
        
        for i, bone_name in enumerate(leg_bones):
            if bone_name in armature.pose.bones:
                bone = armature.pose.bones[bone_name]
                
                # Diagonais em fase: L frente + R tras vs R frente + L tras
                if i == 0 or i == 3:  # L frente e R tras
                    offset = 0
                else:  # R frente e L tras
                    offset = math.pi
                
                # Rotação para simular passo (eixo X para frente/tras)
                angle = math.sin(phase + offset) * 0.6
                
                bone.rotation_euler = (angle, 0, 0)
                bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    # Adiciona movimento de cauda para dar vida
    tail_bones = ["sacrum.001", "sacrum.002", "sacrum.003", "sacrum.004"]
    for frame in range(1, 31):
        bpy.context.scene.frame_set(frame)
        phase = (frame / 30.0) * 2 * math.pi
        
        for i, bone_name in enumerate(tail_bones):
            if bone_name in armature.pose.bones:
                bone = armature.pose.bones[bone_name]
                angle = math.sin(phase * 2 + i * 0.5) * 0.2  # Abana rápido
                bone.rotation_euler = (0, 0, angle)
                bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    # Adiciona movimento de cabeça
    if "Head" in armature.pose.bones:
        for frame in range(1, 31):
            bpy.context.scene.frame_set(frame)
            phase = (frame / 30.0) * 2 * math.pi
            bone = armature.pose.bones["Head"]
            angle = math.sin(phase) * 0.1  # Balanço suave
            bone.rotation_euler = (angle, 0, 0)
            bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    # Volta para modo objeto
    bpy.ops.object.mode_set(mode='OBJECT')
    
    # Exporta como FBX com animação
    bpy.ops.export_scene.fbx(
        filepath=output_fbx,
        use_selection=False,
        add_leaf_bones=True,
        bake_anim=True,
        bake_anim_use_all_bones=True,
        bake_anim_use_nla_strips=False,
        bake_anim_use_all_actions=True,
        bake_anim_force_startend_keying=True,
        mesh_smooth_type='FACE',
        use_mesh_modifiers=True,
        use_armature_deform_only=True,
        apply_unit_scale=True,
        apply_scale_options='FBX_SCALE_UNITS',
        path_mode='AUTO',
        embed_textures=False,
        axis_forward='-Z',
        axis_up='Y'
    )
    
    print(f"Animacao exportada para: {output_fbx}")
    return True


if __name__ == "__main__":
    blend = "/mnt/c/Users/alexa/Projetos/cani-game/Canicross/Assets/Art/Models/Dog_JackRussel_Rigged.blend"
    fbx = "/mnt/c/Users/alexa/Projetos/cani-game/Canicross/Assets/Art/Models/Dog_Run_Animated.fbx"
    
    success = create_dog_run_animation(blend, fbx)
    if success:
        print("SUCESSO: Animacao de corrida criada!")
    else:
        print("FALHA: Nao foi possivel criar a animacao.")
