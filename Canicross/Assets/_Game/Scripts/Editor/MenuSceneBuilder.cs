using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Canicross.UI;

namespace Canicross.Editor
{
    public static class MenuSceneBuilder
    {
        [MenuItem("Canicross/Create Menu Scene")]
        public static void CreateMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateMenuLighting();
            CreateMenuBackground();
            CreateMenuEnvironment();
            CreateMenuCamera();

            GameObject canvas = new GameObject("MenuCanvas");
            Canvas canvasComp = canvas.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            MainMenuController menuCtrl = canvas.AddComponent<MainMenuController>();

            EditorSceneManager.SaveScene(scene, "Assets/_Game/Scenes/MainMenu.unity");
            Debug.Log("[Canicross] Menu scene created at Assets/_Game/Scenes/MainMenu.unity");
        }

        private static void CreateMenuLighting()
        {
            GameObject sun = new GameObject("MenuLight");
            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.95f, 0.8f);
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(30f, -60f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.2f, 0.25f, 0.35f);
            RenderSettings.skybox = null;
        }

        private static void CreateMenuBackground()
        {
            // Sky-colored background plane far away
            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Plane);
            bg.name = "MenuBackground";
            bg.transform.position = new Vector3(0, -1, 20);
            bg.transform.localScale = new Vector3(80, 1, 60);

            Material bgMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            bgMat.color = new Color(0.02f, 0.06f, 0.12f);
            bg.GetComponent<Renderer>().material = bgMat;

            // Ground - grassy trail
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "MenuGround";
            ground.transform.position = new Vector3(0, -1.5f, 10);
            ground.transform.localScale = new Vector3(30, 1, 20);

            Material groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.15f, 0.35f, 0.12f);
            groundMat.SetFloat("_Glossiness", 0.05f);
            ground.GetComponent<Renderer>().material = groundMat;

            // Trail path
            GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Plane);
            trail.name = "MenuTrail";
            trail.transform.position = new Vector3(0, -1.48f, 10);
            trail.transform.localScale = new Vector3(6, 1, 20);

            Material trailMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            trailMat.color = new Color(0.36f, 0.25f, 0.15f);
            trailMat.SetFloat("_Glossiness", 0.05f);
            trail.GetComponent<Renderer>().material = trailMat;

            CreateDogAndRunnerScene(new Vector3(0, -1.0f, 8));
        }

        private static void CreateDogAndRunnerScene(Vector3 basePos)
        {
            Material dogMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            dogMat.color = new Color(0.83f, 0.63f, 0.3f);
            dogMat.SetFloat("_Glossiness", 0.1f);

            Material runnerMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            runnerMat.color = new Color(0.9f, 0.4f, 0.1f);
            runnerMat.SetFloat("_Glossiness", 0.15f);

            // Dog body
            GameObject dogBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dogBody.name = "MenuDogBody";
            dogBody.transform.position = basePos + new Vector3(1.5f, 0.35f, 0);
            dogBody.transform.localScale = new Vector3(0.25f, 0.2f, 0.4f);
            dogBody.transform.rotation = Quaternion.Euler(90, 0, 0);
            dogBody.GetComponent<Renderer>().material = dogMat;
            Object.DestroyImmediate(dogBody.GetComponent<Collider>());

            // Dog head
            GameObject dogHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dogHead.name = "MenuDogHead";
            dogHead.transform.position = basePos + new Vector3(1.5f, 0.55f, 0.3f);
            dogHead.transform.localScale = new Vector3(0.18f, 0.16f, 0.17f);
            dogHead.GetComponent<Renderer>().material = dogMat;
            Object.DestroyImmediate(dogHead.GetComponent<Collider>());

            // Dog ears
            GameObject dogEarL = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dogEarL.name = "MenuDogEarL";
            dogEarL.transform.position = basePos + new Vector3(1.4f, 0.7f, 0.2f);
            dogEarL.transform.localScale = new Vector3(0.05f, 0.08f, 0.03f);
            dogEarL.GetComponent<Renderer>().material = dogMat;
            Object.DestroyImmediate(dogEarL.GetComponent<Collider>());

            GameObject dogEarR = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dogEarR.name = "MenuDogEarR";
            dogEarR.transform.position = basePos + new Vector3(1.6f, 0.7f, 0.2f);
            dogEarR.transform.localScale = new Vector3(0.05f, 0.08f, 0.03f);
            dogEarR.GetComponent<Renderer>().material = dogMat;
            Object.DestroyImmediate(dogEarR.GetComponent<Collider>());

            // Dog legs (4)
            Vector3[] dogLegOffsets = new Vector3[]
            {
                new Vector3(1.4f, 0.1f, 0.15f),
                new Vector3(1.6f, 0.1f, 0.15f),
                new Vector3(1.4f, 0.1f, -0.15f),
                new Vector3(1.6f, 0.1f, -0.15f)
            };
            for (int i = 0; i < 4; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                leg.name = "MenuDogLeg" + i;
                leg.transform.position = basePos + dogLegOffsets[i];
                leg.transform.localScale = new Vector3(0.05f, 0.12f, 0.05f);
                leg.GetComponent<Renderer>().material = dogMat;
                Object.DestroyImmediate(leg.GetComponent<Collider>());
            }

            // Dog tail
            GameObject dogTail = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dogTail.name = "MenuDogTail";
            dogTail.transform.position = basePos + new Vector3(1.5f, 0.5f, -0.3f);
            dogTail.transform.localScale = new Vector3(0.03f, 0.12f, 0.03f);
            dogTail.transform.rotation = Quaternion.Euler(-30, 0, 0);
            dogTail.GetComponent<Renderer>().material = dogMat;
            Object.DestroyImmediate(dogTail.GetComponent<Collider>());

            // Runner body
            GameObject runnerBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            runnerBody.name = "MenuRunnerBody";
            runnerBody.transform.position = basePos + new Vector3(-1.0f, 0.5f, 0);
            runnerBody.transform.localScale = new Vector3(0.3f, 0.25f, 0.18f);
            runnerBody.GetComponent<Renderer>().material = runnerMat;
            Object.DestroyImmediate(runnerBody.GetComponent<Collider>());

            // Runner head
            GameObject runnerHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            runnerHead.name = "MenuRunnerHead";
            runnerHead.transform.position = basePos + new Vector3(-1.0f, 0.9f, 0);
            runnerHead.transform.localScale = new Vector3(0.15f, 0.17f, 0.15f);
            Material skinMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            skinMat.color = new Color(0.7f, 0.5f, 0.35f);
            runnerHead.GetComponent<Renderer>().material = skinMat;
            Object.DestroyImmediate(runnerHead.GetComponent<Collider>());

            // Runner arms
            GameObject runnerArmL = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            runnerArmL.name = "MenuRunnerArmL";
            runnerArmL.transform.position = basePos + new Vector3(-1.25f, 0.55f, 0.1f);
            runnerArmL.transform.localScale = new Vector3(0.06f, 0.2f, 0.06f);
            runnerArmL.transform.rotation = Quaternion.Euler(30, 0, -20);
            runnerArmL.GetComponent<Renderer>().material = skinMat;
            Object.DestroyImmediate(runnerArmL.GetComponent<Collider>());

            GameObject runnerArmR = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            runnerArmR.name = "MenuRunnerArmR";
            runnerArmR.transform.position = basePos + new Vector3(-0.75f, 0.55f, 0.1f);
            runnerArmR.transform.localScale = new Vector3(0.06f, 0.2f, 0.06f);
            runnerArmR.transform.rotation = Quaternion.Euler(30, 0, 20);
            runnerArmR.GetComponent<Renderer>().material = skinMat;
            Object.DestroyImmediate(runnerArmR.GetComponent<Collider>());

            // Runner legs
            GameObject runnerLegL = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            runnerLegL.name = "MenuRunnerLegL";
            runnerLegL.transform.position = basePos + new Vector3(-1.1f, 0.1f, 0);
            runnerLegL.transform.localScale = new Vector3(0.08f, 0.18f, 0.08f);
            runnerLegL.GetComponent<Renderer>().material = runnerMat;
            Object.DestroyImmediate(runnerLegL.GetComponent<Collider>());

            GameObject runnerLegR = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            runnerLegR.name = "MenuRunnerLegR";
            runnerLegR.transform.position = basePos + new Vector3(-0.9f, 0.1f, 0);
            runnerLegR.transform.localScale = new Vector3(0.08f, 0.18f, 0.08f);
            runnerLegR.GetComponent<Renderer>().material = runnerMat;
            Object.DestroyImmediate(runnerLegR.GetComponent<Collider>());

            // Tether (elastic leash) between runner and dog
            GameObject tether = new GameObject("MenuTether");
            LineRenderer lr = tether.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, basePos + new Vector3(-0.5f, 0.5f, 0));
            lr.SetPosition(1, basePos + new Vector3(1.0f, 0.4f, 0));
            lr.startWidth = 0.03f;
            lr.endWidth = 0.03f;
            lr.startColor = new Color(0f, 0.75f, 1f);
            lr.endColor = new Color(0f, 0.75f, 1f);
            lr.useWorldSpace = true;
            Material tetherMat = new Material(Shader.Find("Sprites/Default"));
            tetherMat.color = new Color(0f, 0.75f, 1f);
            lr.material = tetherMat;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private static void CreateMenuEnvironment()
        {
            // Trees on sides
            Color trunkColor = new Color(0.35f, 0.22f, 0.1f);
            Color foliageGreen = new Color(0.13f, 0.37f, 0.13f);

            Vector3[] treePositions = new Vector3[]
            {
                new Vector3(-5, -1.5f, 5),
                new Vector3(-7, -1.5f, 12),
                new Vector3(-4, -1.5f, 18),
                new Vector3(6, -1.5f, 6),
                new Vector3(8, -1.5f, 14),
                new Vector3(5, -1.5f, 20)
            };

            foreach (var pos in treePositions)
            {
                CreateMenuTree(pos, foliageGreen, trunkColor);
            }

            // Small rocks
            Vector3[] rockPositions = new Vector3[]
            {
                new Vector3(3, -1.3f, 4),
                new Vector3(-3, -1.35f, 10),
                new Vector3(2, -1.25f, 16)
            };

            foreach (var pos in rockPositions)
            {
                GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rock.name = "MenuRock";
                rock.transform.position = pos;
                rock.transform.localScale = new Vector3(0.4f, 0.2f, 0.5f);
                Material rockMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                rockMat.color = new Color(0.45f, 0.43f, 0.4f);
                rock.GetComponent<Renderer>().material = rockMat;
                Object.DestroyImmediate(rock.GetComponent<Collider>());
            }

            // Bench
            CreateMenuBench(new Vector3(-3.5f, -1.5f, 8));
        }

        private static void CreateMenuTree(Vector3 pos, Color foliageColor, Color trunkColor)
        {
            GameObject tree = new GameObject("MenuTree");
            tree.transform.position = pos;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0, 1.0f, 0);
            trunk.transform.localScale = new Vector3(0.2f, 1.0f, 0.2f);
            Material trunkMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            trunkMat.color = trunkColor;
            trunk.GetComponent<Renderer>().material = trunkMat;
            Object.DestroyImmediate(trunk.GetComponent<Collider>());

            GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage.name = "Foliage";
            foliage.transform.SetParent(tree.transform);
            foliage.transform.localPosition = new Vector3(0, 2.2f, 0);
            foliage.transform.localScale = new Vector3(1.5f, 1.3f, 1.5f);
            Material foliageMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            foliageMat.color = foliageColor;
            foliage.GetComponent<Renderer>().material = foliageMat;
            Object.DestroyImmediate(foliage.GetComponent<Collider>());
        }

        private static void CreateMenuBench(Vector3 pos)
        {
            GameObject bench = new GameObject("MenuBench");
            bench.transform.position = pos;

            Material woodMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            woodMat.color = new Color(0.55f, 0.35f, 0.15f);

            GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.name = "Seat";
            seat.transform.SetParent(bench.transform);
            seat.transform.localPosition = new Vector3(0, 0.35f, 0);
            seat.transform.localScale = new Vector3(1.2f, 0.08f, 0.4f);
            seat.GetComponent<Renderer>().material = woodMat;
            Object.DestroyImmediate(seat.GetComponent<Collider>());

            for (int i = 0; i < 2; i++)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.name = "Leg_" + i;
                leg.transform.SetParent(bench.transform);
                leg.transform.localPosition = new Vector3(i == 0 ? -0.5f : 0.5f, 0.175f, 0);
                leg.transform.localScale = new Vector3(0.08f, 0.35f, 0.35f);
                leg.GetComponent<Renderer>().material = woodMat;
                Object.DestroyImmediate(leg.GetComponent<Collider>());
            }
        }

        private static void CreateMenuCamera()
        {
            GameObject cam = new GameObject("MenuCamera");
            Camera menuCam = cam.AddComponent<Camera>();
            menuCam.clearFlags = CameraClearFlags.SolidColor;
            menuCam.backgroundColor = new Color(0.05f, 0.12f, 0.22f);
            menuCam.fieldOfView = 60;
            cam.transform.position = new Vector3(0, 1.5f, -2);
            cam.transform.LookAt(new Vector3(0, 0.2f, 8));
        }
    }
}
