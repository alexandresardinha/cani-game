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
            light.intensity = 0.5f;
            sun.transform.rotation = Quaternion.Euler(30f, -60f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.12f, 0.15f, 0.2f);
        }

        private static void CreateMenuBackground()
        {
            GameObject bg = GameObject.CreatePrimitive(PrimitiveType.Plane);
            bg.name = "MenuBackground";
            bg.transform.position = new Vector3(0, -2, 10);
            bg.transform.localScale = new Vector3(50, 1, 50);
            bg.transform.rotation = Quaternion.Euler(0, 0, 0);

            Material bgMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            bgMat.color = new Color(0.05f, 0.08f, 0.12f);
            bg.GetComponent<Renderer>().material = bgMat;

            CreateDogSilhouette(new Vector3(2, -1, 8));
            CreateRunnerSilhouette(new Vector3(-1, -1, 8));
        }

        private static void CreateDogSilhouette(Vector3 pos)
        {
            Material silhouetteMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            silhouetteMat.color = new Color(0.02f, 0.04f, 0.06f);

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "MenuDogBody";
            body.transform.position = pos;
            body.transform.localScale = new Vector3(0.3f, 0.18f, 0.35f);
            body.GetComponent<Renderer>().material = silhouetteMat;

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "MenuDogHead";
            head.transform.position = pos + new Vector3(0, 0.15f, 0.25f);
            head.transform.localScale = new Vector3(0.15f, 0.13f, 0.14f);
            head.GetComponent<Renderer>().material = silhouetteMat;
        }

        private static void CreateRunnerSilhouette(Vector3 pos)
        {
            Material silhouetteMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            silhouetteMat.color = new Color(0.02f, 0.04f, 0.06f);

            GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            torso.name = "MenuRunnerBody";
            torso.transform.position = pos;
            torso.transform.localScale = new Vector3(0.22f, 0.25f, 0.15f);
            torso.GetComponent<Renderer>().material = silhouetteMat;

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "MenuRunnerHead";
            head.transform.position = pos + new Vector3(0, 0.55f, 0);
            head.transform.localScale = new Vector3(0.12f, 0.14f, 0.12f);
            head.GetComponent<Renderer>().material = silhouetteMat;
        }

        private static void CreateMenuCamera()
        {
            GameObject cam = new GameObject("MenuCamera");
            Camera menuCam = cam.AddComponent<Camera>();
            menuCam.clearFlags = CameraClearFlags.SolidColor;
            menuCam.backgroundColor = new Color(0.05f, 0.08f, 0.12f);
            menuCam.fieldOfView = 60;
            cam.transform.position = new Vector3(0, 1, 0);
            cam.transform.LookAt(new Vector3(0, 0, 10));
        }
    }
}