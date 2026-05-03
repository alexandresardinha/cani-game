using UnityEngine;
using UnityEditor;

namespace Canicross.Environment
{
    public static class TrackGenerator
    {
        public static GameObject GenerateLakeParkTrack(Transform parent)
        {
            GameObject trackRoot = new GameObject("Track_01_Lago");
            trackRoot.transform.SetParent(parent, false);
            trackRoot.transform.position = Vector3.zero;

            CreateGround(trackRoot.transform);
            CreateLake(trackRoot.transform);
            CreateTrees(trackRoot.transform);
            CreatePath(trackRoot.transform);
            CreateGoldenField(trackRoot.transform);
            CreateForestTunnel(trackRoot.transform);
            CreateProps(trackRoot.transform);
            CreateLighting(trackRoot.transform);
            CreateCheckpoints(trackRoot.transform);
            CreateObstacles(trackRoot.transform);
            CreateAtmosphere(trackRoot.transform);

            return trackRoot;
        }

        private static void CreateGround(Transform parent)
        {
            // Main forest ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground_Forest";
            ground.transform.SetParent(parent);
            ground.transform.position = new Vector3(0, 0, 50);
            ground.transform.localScale = new Vector3(25, 1, 12);

            Material groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.22f, 0.38f, 0.14f);
            groundMat.SetFloat("_Glossiness", 0.05f);
            ground.GetComponent<Renderer>().material = groundMat;

            ground.layer = LayerMask.NameToLayer("Default");
        }

        private static void CreateLake(Transform parent)
        {
            // Main lake body
            GameObject lake = GameObject.CreatePrimitive(PrimitiveType.Plane);
            lake.name = "Lake";
            lake.transform.SetParent(parent);
            lake.transform.position = new Vector3(9, -0.08f, 50);
            lake.transform.localScale = new Vector3(8, 1, 12);

            Material waterMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            waterMat.color = new Color(0.1f, 0.5f, 0.72f);
            waterMat.SetFloat("_Glossiness", 0.95f);
            waterMat.SetFloat("_Metallic", 0.2f);
            lake.GetComponent<Renderer>().material = waterMat;

            Collider lakeCol = lake.GetComponent<Collider>();
            if (lakeCol != null)
            {
                lakeCol.isTrigger = true;
                lake.AddComponent<Obstacle>().SetType(Obstacle.ObstacleType.Water);
            }

            // Lake shore detail (small pebbles/edge)
            GameObject shore = GameObject.CreatePrimitive(PrimitiveType.Plane);
            shore.name = "LakeShore";
            shore.transform.SetParent(parent);
            shore.transform.position = new Vector3(5.5f, 0.01f, 50);
            shore.transform.localScale = new Vector3(1.5f, 1, 12);

            Material shoreMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            shoreMat.color = new Color(0.5f, 0.45f, 0.3f);
            shoreMat.SetFloat("_Glossiness", 0.1f);
            shore.GetComponent<Renderer>().material = shoreMat;
        }

        private static void CreateTrees(Transform parent)
        {
            Color trunkColor = new Color(0.35f, 0.22f, 0.1f);
            Color foliageGreen = new Color(0.13f, 0.37f, 0.13f);
            Color foliageDark = new Color(0.06f, 0.22f, 0.06f);
            Color foliageGolden = new Color(0.5f, 0.45f, 0.15f);

            float[,] treePositions = new float[,]
            {
                // Left side forest
                {-5f, 3f}, {-6.5f, 8f}, {-4f, 15f}, {-7f, 22f},
                {-5.5f, 30f}, {-6f, 38f}, {-4.5f, 45f}, {-7f, 52f},
                {-5f, 60f}, {-6.5f, 68f}, {-4f, 75f}, {-7f, 82f},
                {-5.5f, 88f}, {-6f, 95f},
                // Right side (away from lake)
                {-4f, 10f}, {-5.5f, 18f}, {-3.5f, 28f}, {-6f, 42f},
                {-4f, 55f}, {-5f, 65f}, {-3.5f, 72f}, {-6.5f, 85f},
                // Far right (past lake area)
                {12f, 5f}, {13.5f, 12f}, {12.5f, 20f}, {14f, 28f},
                {12f, 36f}, {13f, 42f}, {12.5f, 50f}, {14f, 58f}
            };

            for (int i = 0; i < treePositions.GetLength(0); i++)
            {
                float x = treePositions[i, 0];
                float z = treePositions[i, 1];
                float rand = Random.value;
                Color foliageColor;
                if (rand > 0.7f) foliageColor = foliageGolden;
                else if (rand > 0.4f) foliageColor = foliageDark;
                else foliageColor = foliageGreen;

                float scale = Random.Range(0.8f, 1.3f);
                CreateTree(parent, x, z, foliageColor, trunkColor, scale);
            }
        }

        private static void CreateTree(Transform parent, float x, float z, Color foliageColor, Color trunkColor, float scale = 1f)
        {
            float height = Random.Range(2.5f, 4.5f) * scale;
            float trunkHeight = height * 0.5f;
            float foliageRadius = Random.Range(1.2f, 2.0f) * scale;

            GameObject tree = new GameObject("Tree");
            tree.transform.SetParent(parent);
            tree.transform.position = new Vector3(x, 0, z);

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, trunkHeight * 0.5f, 0);
            trunk.transform.localScale = new Vector3(0.2f * scale, trunkHeight * 0.5f, 0.2f * scale);

            Material trunkMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            trunkMat.color = trunkColor;
            trunkMat.SetFloat("_Glossiness", 0.05f);
            trunk.GetComponent<Renderer>().material = trunkMat;
            DestroyColliderIfPrimitive(trunk);

            GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage.name = "Foliage";
            foliage.transform.SetParent(tree.transform);
            foliage.transform.localPosition = new Vector3(0, trunkHeight + foliageRadius * 0.5f, 0);
            foliage.transform.localScale = new Vector3(foliageRadius, foliageRadius * 0.9f, foliageRadius);

            Material foliageMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            foliageMat.color = foliageColor;
            foliageMat.SetFloat("_Glossiness", 0.05f);
            foliage.GetComponent<Renderer>().material = foliageMat;
            DestroyColliderIfPrimitive(foliage);
        }

        private static void CreatePath(Transform parent)
        {
            // Main dirt trail
            GameObject path = GameObject.CreatePrimitive(PrimitiveType.Plane);
            path.name = "Trail_Dirt";
            path.transform.SetParent(parent);
            path.transform.position = new Vector3(0, 0.01f, 50);
            path.transform.localScale = new Vector3(3.5f, 1, 12);

            Material pathMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            pathMat.color = new Color(0.4f, 0.28f, 0.15f);
            pathMat.SetFloat("_Glossiness", 0.05f);
            path.GetComponent<Renderer>().material = pathMat;

            // Trail edges - slightly lighter for worn look
            GameObject pathEdgeL = GameObject.CreatePrimitive(PrimitiveType.Plane);
            pathEdgeL.name = "TrailEdgeL";
            pathEdgeL.transform.SetParent(parent);
            pathEdgeL.transform.position = new Vector3(-1.8f, 0.005f, 50);
            pathEdgeL.transform.localScale = new Vector3(0.6f, 1, 12);

            Material edgeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            edgeMat.color = new Color(0.35f, 0.32f, 0.18f);
            edgeMat.SetFloat("_Glossiness", 0.05f);
            pathEdgeL.GetComponent<Renderer>().material = edgeMat;

            GameObject pathEdgeR = GameObject.CreatePrimitive(PrimitiveType.Plane);
            pathEdgeR.name = "TrailEdgeR";
            pathEdgeR.transform.SetParent(parent);
            pathEdgeR.transform.position = new Vector3(1.8f, 0.005f, 50);
            pathEdgeR.transform.localScale = new Vector3(0.6f, 1, 12);
            pathEdgeR.GetComponent<Renderer>().material = edgeMat;
        }

        private static void CreateGoldenField(Transform parent)
        {
            // Create a golden field area around z=35-45 (clearing)
            GameObject field = GameObject.CreatePrimitive(PrimitiveType.Plane);
            field.name = "GoldenField";
            field.transform.SetParent(parent);
            field.transform.position = new Vector3(-6, 0, 40);
            field.transform.localScale = new Vector3(8, 1, 4);

            Material fieldMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            fieldMat.color = new Color(0.75f, 0.65f, 0.2f);
            fieldMat.SetFloat("_Glossiness", 0.1f);
            field.GetComponent<Renderer>().material = fieldMat;

            // Add a solitary tree in the clearing
            Color trunkColor = new Color(0.35f, 0.22f, 0.1f);
            Color foliageColor = new Color(0.5f, 0.55f, 0.2f);
            CreateTree(parent, -6, 40, foliageColor, trunkColor, 1.4f);
        }

        private static void CreateForestTunnel(Transform parent)
        {
            // Dense trees forming a tunnel around z=55-70
            Color trunkColor = new Color(0.35f, 0.22f, 0.1f);
            Color foliageDark = new Color(0.05f, 0.18f, 0.05f);

            float[,] tunnelPositions = new float[,]
            {
                {-3f, 55f}, {-3.5f, 58f}, {-3f, 62f}, {-3.5f, 66f}, {-3f, 70f},
                {3f, 55f}, {3.5f, 58f}, {3f, 62f}, {3.5f, 66f}, {3f, 70f},
                {-2.5f, 52f}, {2.5f, 52f}, {-2.5f, 73f}, {2.5f, 73f}
            };

            for (int i = 0; i < tunnelPositions.GetLength(0); i++)
            {
                float x = tunnelPositions[i, 0];
                float z = tunnelPositions[i, 1];
                CreateTree(parent, x, z, foliageDark, trunkColor, 1.1f);
            }
        }

        private static void CreateProps(Transform parent)
        {
            CreateBench(parent, new Vector3(3.5f, 0, 10f));
            CreateBench(parent, new Vector3(-3.5f, 0f, 55f));
            CreateRock(parent, new Vector3(2.2f, 0.15f, 25f), 0.4f);
            CreateRock(parent, new Vector3(-1.8f, 0.12f, 40f), 0.3f);
            CreateRock(parent, new Vector3(1.5f, 0.18f, 70f), 0.5f);
            CreateRock(parent, new Vector3(-2.5f, 0.1f, 85f), 0.35f);

            // Small bushes
            CreateBush(parent, new Vector3(-2.5f, 0, 18f), 0.6f);
            CreateBush(parent, new Vector3(2.8f, 0, 32f), 0.5f);
            CreateBush(parent, new Vector3(-2.2f, 0, 48f), 0.7f);
            CreateBush(parent, new Vector3(2.5f, 0, 65f), 0.55f);
        }

        private static void CreateBench(Transform parent, Vector3 pos)
        {
            GameObject bench = new GameObject("Bench");
            bench.transform.SetParent(parent);
            bench.transform.position = pos;

            Material woodMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            woodMat.color = new Color(0.55f, 0.35f, 0.15f);
            woodMat.SetFloat("_Glossiness", 0.05f);

            GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.name = "Seat";
            seat.transform.SetParent(bench.transform);
            seat.transform.localPosition = new Vector3(0, 0.35f, 0);
            seat.transform.localScale = new Vector3(1.2f, 0.08f, 0.4f);
            seat.GetComponent<Renderer>().material = woodMat;
            DestroyColliderIfPrimitive(seat);

            for (int i = 0; i < 2; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.name = "Leg_" + i;
                leg.transform.SetParent(bench.transform);
                leg.transform.localPosition = new Vector3(i == 0 ? -0.5f : 0.5f, 0.175f, 0);
                leg.transform.localScale = new Vector3(0.08f, 0.35f, 0.35f);
                leg.GetComponent<Renderer>().material = woodMat;
                DestroyColliderIfPrimitive(leg);
            }
        }

        private static void CreateRock(Transform parent, Vector3 pos, float scale)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "Rock";
            rock.transform.SetParent(parent);
            rock.transform.position = pos;
            rock.transform.localScale = new Vector3(scale, scale * 0.5f, scale * 0.8f);

            Material rockMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            rockMat.color = new Color(0.45f, 0.43f, 0.4f);
            rockMat.SetFloat("_Glossiness", 0.05f);
            rock.GetComponent<Renderer>().material = rockMat;
        }

        private static void CreateBush(Transform parent, Vector3 pos, float scale)
        {
            GameObject bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bush.name = "Bush";
            bush.transform.SetParent(parent);
            bush.transform.position = pos + Vector3.up * scale * 0.4f;
            bush.transform.localScale = new Vector3(scale, scale * 0.7f, scale);

            Material bushMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            bushMat.color = new Color(0.2f, 0.5f, 0.15f);
            bushMat.SetFloat("_Glossiness", 0.1f);
            bush.GetComponent<Renderer>().material = bushMat;
            DestroyColliderIfPrimitive(bush);
        }

        private static void CreateLighting(Transform parent)
        {
            GameObject sun = new GameObject("Directional Light");
            sun.transform.SetParent(parent);
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.9f, 0.75f);
            light.intensity = 1.3f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.9f;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.6f, 0.65f, 0.75f);
        }

        private static void CreateAtmosphere(Transform parent)
        {
            // Add some god ray-like vertical light pillars (simple visual effect)
            for (int i = 0; i < 5; i++)
            {
                float z = 20f + i * 18f;
                float x = Random.Range(-2f, 2f);

                GameObject lightPillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                lightPillar.name = "GodRay_" + i;
                lightPillar.transform.SetParent(parent);
                lightPillar.transform.position = new Vector3(x, 4f, z);
                lightPillar.transform.localScale = new Vector3(0.8f, 4f, 0.8f);
                lightPillar.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

                Material rayMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                rayMat.color = new Color(0.9f, 0.85f, 0.5f, 0.15f);
                rayMat.SetFloat("_Glossiness", 0f);
                rayMat.SetFloat("_Metallic", 0f);
                rayMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                rayMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                rayMat.SetInt("_ZWrite", 0);
                rayMat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                rayMat.renderQueue = 3000;
                lightPillar.GetComponent<Renderer>().material = rayMat;
                DestroyColliderIfPrimitive(lightPillar);
            }
        }

        private static void CreateCheckpoints(Transform parent)
        {
            CreateCheckpoint(parent, "Start", new Vector3(0, 0, 0), 0, false);
            CreateCheckpoint(parent, "CP1_Clearing", new Vector3(0, 0, 25), 1, false);
            CreateCheckpoint(parent, "CP2_Forest", new Vector3(0, 0, 50), 2, false);
            CreateCheckpoint(parent, "CP3_Lake", new Vector3(0, 0, 75), 3, false);
            CreateCheckpoint(parent, "Finish", new Vector3(0, 0, 100), 4, true);
        }

        private static void CreateCheckpoint(Transform parent, string name, Vector3 pos, int index, bool isFinish)
        {
            GameObject cp = new GameObject(name);
            cp.transform.SetParent(parent);
            cp.transform.position = pos;

            BoxCollider trigger = cp.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(6, 4, 2);

            CheckpointTrigger cpTrigger = cp.AddComponent<CheckpointTrigger>();
            SetCheckpointIndex(cpTrigger, index, isFinish);

            GameObject poleL = CreatePole(parent, pos + new Vector3(-2.5f, 0, 0));
            GameObject poleR = CreatePole(parent, pos + new Vector3(2.5f, 0, 0));

            if (isFinish)
            {
                CreateBanner(parent, pos, new Color(1f, 0.85f, 0f));
            }
        }

        private static void SetCheckpointIndex(CheckpointTrigger trigger, int index, bool isFinish)
        {
            var serializedObj = new SerializedObject(trigger);
            serializedObj.FindProperty("checkpointIndex").intValue = index;
            serializedObj.FindProperty("isFinishLine").boolValue = isFinish;
            serializedObj.ApplyModifiedProperties();
        }

        private static GameObject CreatePole(Transform parent, Vector3 pos)
        {
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.transform.SetParent(parent);
            pole.transform.position = pos + Vector3.up * 1.5f;
            pole.transform.localScale = new Vector3(0.1f, 1.5f, 0.1f);

            Material poleMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            poleMat.color = Color.white;
            pole.GetComponent<Renderer>().material = poleMat;

            return pole;
        }

        private static void CreateBanner(Transform parent, Vector3 pos, Color color)
        {
            GameObject banner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            banner.name = "FinishBanner";
            banner.transform.SetParent(parent);
            banner.transform.position = pos + Vector3.up * 3f;
            banner.transform.localScale = new Vector3(5f, 0.3f, 0.1f);

            Material bannerMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            bannerMat.color = color;
            banner.GetComponent<Renderer>().material = bannerMat;
            DestroyColliderIfPrimitive(banner);
        }

        private static void CreateObstacles(Transform parent)
        {
            CreateRootObstacle(parent, new Vector3(0.8f, 0.1f, 20f));
            CreateRootObstacle(parent, new Vector3(-0.5f, 0.1f, 35f));
            CreateRootObstacle(parent, new Vector3(0.3f, 0.1f, 60f));
            CreateRootObstacle(parent, new Vector3(-0.7f, 0.1f, 80f));

            CreateMudPatch(parent, new Vector3(0, 0.01f, 15f), new Vector3(2, 0.05f, 3));
            CreateMudPatch(parent, new Vector3(0, 0.01f, 45f), new Vector3(1.5f, 0.05f, 2));
            CreateMudPatch(parent, new Vector3(0, 0.01f, 65f), new Vector3(2, 0.05f, 2.5f));
        }

        private static void CreateRootObstacle(Transform parent, Vector3 pos)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cube);
            root.name = "Root_Obstacle";
            root.transform.SetParent(parent);
            root.transform.position = pos;
            root.transform.localScale = new Vector3(0.8f, 0.2f, 0.3f);
            root.transform.rotation = Quaternion.Euler(0, Random.Range(-20f, 20f), 0);

            Material rootMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            rootMat.color = new Color(0.3f, 0.18f, 0.08f);
            root.GetComponent<Renderer>().material = rootMat;

            BoxCollider trigger = root.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1.5f, 3f, 1.5f);

            Obstacle obstacle = root.AddComponent<Obstacle>();
            SetObstacleType(obstacle, Obstacle.ObstacleType.Root);
        }

        private static void CreateMudPatch(Transform parent, Vector3 pos, Vector3 scale)
        {
            GameObject mud = GameObject.CreatePrimitive(PrimitiveType.Plane);
            mud.name = "Mud_Patch";
            mud.transform.SetParent(parent);
            mud.transform.position = pos;
            mud.transform.localScale = new Vector3(scale.x, 1, scale.z);

            Material mudMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mudMat.color = new Color(0.3f, 0.2f, 0.1f);
            mudMat.SetFloat("_Glossiness", 0.4f);
            mud.GetComponent<Renderer>().material = mudMat;

            BoxCollider trigger = mud.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1, 0.5f, 1);

            Obstacle obstacle = mud.AddComponent<Obstacle>();
            SetObstacleType(obstacle, Obstacle.ObstacleType.Mud);
        }

        private static void SetObstacleType(Obstacle obstacle, Obstacle.ObstacleType type)
        {
            obstacle.SetType(type);
        }

        private static void DestroyColliderIfPrimitive(GameObject go)
        {
            Collider col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
        }
    }
}
