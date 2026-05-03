using UnityEngine;
using UnityEditor;

namespace Canicross.Editor
{
    /// <summary>
    /// Constroi o prefab do Dog usando o modelo Jack Russell (OBJ).
    /// </summary>
    public static class DogPrefabBuilder
    {
        [MenuItem("Canicross/Build Dog Prefab (Adao Animated)")]
        public static void BuildDogPrefab()
        {
            // 1. Carrega o OBJ original (mesh estatico, sem bones)
            string objPath = "Assets/Art/Models/Dog_JackRussel_Rigged.obj";
            Mesh dogMesh = AssetDatabase.LoadAssetAtPath<Mesh>(objPath);

            if (dogMesh == null)
            {
                Debug.LogError($"[DogPrefabBuilder] Mesh nao encontrado em: {objPath}");
                return;
            }

            // 2. Cria GameObject com o mesh
            GameObject dogInstance = new GameObject("Dog_Adao");
            
            MeshFilter mf = dogInstance.AddComponent<MeshFilter>();
            mf.sharedMesh = dogMesh;
            
            MeshRenderer mr = dogInstance.AddComponent<MeshRenderer>();
            
            // 3. Escala para tamanho real (~0.3m de altura)
            // O modelo original tem dimensoes grandes do Blender
            dogInstance.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            
            // Rotaciona para ficar de pe (correcao de eixo)
            dogInstance.transform.rotation = Quaternion.Euler(-90, 0, 0);

            // 4. Configura materiais do OBJ
            if (mr.sharedMaterial == null || mr.sharedMaterial.name == "Default-Material")
            {
                Material brownMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Game/Materials/Dog_Brown.mat");
                if (brownMat != null)
                {
                    mr.sharedMaterial = brownMat;
                }
            }

            // 5. Adiciona Rigidbody
            Rigidbody rb = dogInstance.AddComponent<Rigidbody>();
            rb.linearDamping = 1f;
            rb.angularDamping = 1f;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.mass = 22f;
            rb.useGravity = true;

            // 6. DogController
            dogInstance.AddComponent<Player.DogController>();

            // 7. Ponto de ancoragem da corda (tronco)
            GameObject attachGo = new GameObject("TetherAttachPoint");
            attachGo.transform.SetParent(dogInstance.transform, false);
            attachGo.transform.localPosition = new Vector3(0f, 0.15f, -0.1f);

            // 8. Cria prefab
            string prefabPath = "Assets/_Game/Prefabs/Dog_Adao.prefab";
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existingPrefab != null)
            {
                AssetDatabase.DeleteAsset(prefabPath);
            }

            PrefabUtility.SaveAsPrefabAsset(dogInstance, prefabPath);
            Object.DestroyImmediate(dogInstance);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);

            Debug.Log("[DogPrefabBuilder] Dog criado com sucesso!");
        }
    }
}
