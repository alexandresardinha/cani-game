#!/usr/bin/env python3
"""
Corrige a rotação e escala do modelo de cachorro no Blender.
O modelo original está deitado/escalado errado. Este script:
1. Aplica escala e rotação corretas
2. Reposiciona para ficar de pé
3. Exporta FBX corrigido
"""

import bpy
import math

def fix_dog_model():
    blend_file = "/mnt/c/Users/alexa/Projetos/cani-game/Canicross/Assets/Art/Models/Dog_JackRussel_Rigged.blend"
    output_fbx = "/mnt/c/Users/alexa/Projetos/cani-game/Canicross/Assets/Art/Models/Dog_Run_Animated.fbx"
    
    # Abre o arquivo
    bpy.ops.wm.open_mainfile(filepath=blend_file)
    
    # Seleciona o mesh e o armature
    dog = bpy.data.objects.get("Dog")
    rig = bpy.data.objects.get("Dog RIG")
    
    if not dog or not rig:
        print("ERRO: Objetos nao encontrados")
        return False
    
    # Seleciona ambos
    bpy.ops.object.select_all(action='DESELECT')
    dog.select_set(True)
    rig.select_set(True)
    bpy.context.view_layer.objects.active = dog
    
    # Aplica escala e rotação
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
    
    # Rotaciona 90 graus no eixo X para ficar de pé (Blender Z-up vs Unity Y-up)
    dog.rotation_euler = (math.radians(90), 0, 0)
    rig.rotation_euler = (math.radians(90), 0, 0)
    
    # Aplica novamente
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
    
    # Escala para tamanho real (~0.3m de altura)
    # Dimensões atuais do Dog
    print(f"Dimensoes antes: {dog.dimensions.x:.3f} x {dog.dimensions.y:.3f} x {dog.dimensions.z:.3f}")
    
    # Escala para ~0.3m de altura
    target_height = 0.3
    current_height = dog.dimensions.z
    scale = target_height / current_height
    
    dog.scale = (scale, scale, scale)
    rig.scale = (scale, scale, scale)
    
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
    
    print(f"Dimensoes depois: {dog.dimensions.x:.3f} x {dog.dimensions.y:.3f} x {dog.dimensions.z:.3f}")
    
    # Move para origem
    dog.location = (0, 0, 0)
    rig.location = (0, 0, 0)
    
    # Cria animação de corrida
    bpy.ops.object.mode_set(mode='POSE')
    
    leg_bones = [
        "front humerus.L", "front humerus.R",
        "femur.L", "femur.R",
    ]
    
    fps = 30
    bpy.context.scene.frame_start = 1
    bpy.context.scene.frame_end = 30
    bpy.context.scene.render.fps = fps
    
    for frame in range(1, 31):
        bpy.context.scene.frame_set(frame)
        phase = (frame / 30.0) * 2 * math.pi
        
        for i, bone_name in enumerate(leg_bones):
            if bone_name in bpy.context.active_object.pose.bones:
                bone = bpy.context.active_object.pose.bones[bone_name]
                offset = 0 if (i == 0 or i == 3) else math.pi
                angle = math.sin(phase + offset) * 0.5
                bone.rotation_euler = (angle, 0, 0)
                bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    # Anima cauda
    tail_bones = ["sacrum.001", "sacrum.002", "sacrum.003", "sacrum.004"]
    for frame in range(1, 31):
        bpy.context.scene.frame_set(frame)
        phase = (frame / 30.0) * 2 * math.pi
        for i, bone_name in enumerate(tail_bones):
            if bone_name in bpy.context.active_object.pose.bones:
                bone = bpy.context.active_object.pose.bones[bone_name]
                angle = math.sin(phase * 2 + i * 0.5) * 0.2
                bone.rotation_euler = (0, 0, angle)
                bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    # Anima cabeça
    if "Head" in bpy.context.active_object.pose.bones:
        for frame in range(1, 31):
            bpy.context.scene.frame_set(frame)
            phase = (frame / 30.0) * 2 * math.pi
            bone = bpy.context.active_object.pose.bones["Head"]
            angle = math.sin(phase) * 0.1
            bone.rotation_euler = (angle, 0, 0)
            bone.keyframe_insert(data_path="rotation_euler", frame=frame)
    
    bpy.ops.object.mode_set(mode='OBJECT')
    
    # Exporta FBX
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
    
    print(f"FBX corrigido exportado: {output_fbx}")
    return True


if __name__ == "__main__":
    success = fix_dog_model()
    if success:
        print("SUCESSO: Modelo corrigido e animado!")
    else:
        print("FALHA")
