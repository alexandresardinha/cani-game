using UnityEngine;
using UnityEditor;
using System.IO;
using Canicross.Player;
using Canicross.Environment;

namespace Canicross.Editor
{
    public static class AutoTester
    {
        [MenuItem("Canicross/Run Auto Test")]
        public static void RunTest()
        {
            Debug.Log("[AutoTester] Iniciando teste automatico...");

            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Single);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.8f, 0.8f, 0.9f);

            var camGo = new GameObject("ScreenshotCamera");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.5f, 0.7f, 0.9f);
            cam.fieldOfView = 90;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 100f;

            Debug.Log("[AutoTester] Chamando GameSetupWizard...");
            GameSetupWizard.SetupGameScene();

            var dog = GameObject.Find("Dog");
            if (dog != null)
            {
                cam.transform.position = new Vector3(3f, 1.2f, 6f);
                cam.transform.LookAt(new Vector3(0f, 0.5f, 7f));
                Debug.Log("[AutoTester] Camera posicionada no cachorro: " + dog.transform.position);
            }
            else
            {
                cam.transform.position = new Vector3(3, 2, -5);
                cam.transform.LookAt(Vector3.zero);
                Debug.LogWarning("[AutoTester] Cachorro nao encontrado, camera na posicao padrao");
            }

            if (Application.isBatchMode)
            {
                GenerateReport();
                // In batch mode, wait briefly for render pipeline to initialize
                // Optimized: reduced from 2000ms to 500ms for faster batch testing
                System.Threading.Thread.Sleep(500);
                CaptureScreenshotRendered();
                Debug.Log("[AutoTester] Teste concluido!");
                EditorApplication.Exit(0);
            }
            else
            {
                EditorApplication.update += WaitAndCapture;
            }
        }

        private static int frameCount = 0;
        private static void WaitAndCapture()
        {
            frameCount++;
            // Optimized: reduced from 10 to 3 frames for faster editor testing
            if (frameCount < 3) return;
            EditorApplication.update -= WaitAndCapture;
            GenerateReport();
            CaptureScreenshotRendered();
            Debug.Log("[AutoTester] Teste concluido!");
        }

        private static void CaptureScreenshotSimple(Camera cam)
        {
        }

        private static void CaptureScreenshotRendered()
        {
            Debug.Log("[AutoTester] Capturando screenshot via render...");

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string testDir = Path.Combine(projectRoot, "TestResults");
            Directory.CreateDirectory(testDir);
            string screenshotPath = Path.Combine(testDir, "dog_test_result.png");

            try
            {
                Camera cam = null;
                var camObj = GameObject.Find("CameraRig");
                if (camObj != null) cam = camObj.GetComponent<Camera>();
                if (cam == null) cam = Camera.main;
                if (cam == null)
                {
                    var screenshotCam = GameObject.Find("ScreenshotCamera");
                    if (screenshotCam != null) cam = screenshotCam.GetComponent<Camera>();
                }

                if (cam == null)
                {
                    Debug.LogError("[AutoTester] Nenhuma camera encontrada para screenshot!");
                    return;
                }

                int width = 1280;
                int height = 720;

                RenderTexture rt = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
                rt.antiAliasing = 1;
                rt.Create();
                cam.targetTexture = rt;
                cam.Render();
                RenderTexture.active = rt;

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();

                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(screenshotPath, bytes);

                Debug.Log("[AutoTester] Screenshot salvo em: " + screenshotPath + " (" + bytes.Length + " bytes)");

                cam.targetTexture = null;
                RenderTexture.active = null;
                Object.DestroyImmediate(rt);
                Object.DestroyImmediate(tex);
            }
            catch (System.Exception e)
            {
                Debug.LogError("[AutoTester] Erro ao capturar screenshot: " + e.Message + "\n" + e.StackTrace);
            }
        }

        private static void GenerateReport()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string testDir = Path.Combine(projectRoot, "TestResults");
            Directory.CreateDirectory(testDir);

            var dog = GameObject.Find("Dog");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("# Relatorio de Teste - Canicross");
            sb.AppendLine("Data: " + System.DateTime.Now.ToString());
            sb.AppendLine();

            if (dog != null)
            {
                sb.AppendLine("## Cachorro (Dog)");
                sb.AppendLine("- Nome: " + dog.name);
                sb.AppendLine("- Posicao: " + dog.transform.position.ToString());
                sb.AppendLine("- Escala: " + dog.transform.localScale.ToString());
                sb.AppendLine("- Rotacao: " + dog.transform.eulerAngles.ToString());
                sb.AppendLine("- Filhos: " + dog.transform.childCount.ToString());

                var renderer = dog.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    sb.AppendLine("- Bounds: " + renderer.bounds.ToString());
                }

                var rb = dog.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    sb.AppendLine("- Massa: " + rb.mass.ToString());
                    sb.AppendLine("- Use Gravity: " + rb.useGravity.ToString());
                }

                var dogCtrl = dog.GetComponent<DogController>();
                sb.AppendLine("- DogController: " + (dogCtrl != null ? "SIM" : "NAO"));

                var tether = dog.transform.Find("TetherAttachPoint");
                sb.AppendLine("- TetherAttachPoint: " + (tether != null ? "SIM" : "NAO"));

                int partCount = 0;
                foreach (Transform child in dog.transform)
                {
                    partCount++;
                }
                sb.AppendLine("- Partes (filhos): " + partCount.ToString());
            }
            else
            {
                sb.AppendLine("## ERRO: Cachorro nao encontrado!");
            }

            var runner = GameObject.Find("Runner");
            if (runner != null)
            {
                sb.AppendLine();
                sb.AppendLine("## Corredor (Runner)");
                sb.AppendLine("- Posicao: " + runner.transform.position.ToString());
                sb.AppendLine("- Escala: " + runner.transform.localScale.ToString());
                sb.AppendLine("- Filhos: " + runner.transform.childCount.ToString());

                var cc = runner.GetComponent<CharacterController>();
                if (cc != null)
                {
                    sb.AppendLine("- CharacterController: SIM (height=" + cc.height + ", radius=" + cc.radius + ")");
                }

                var tether = runner.transform.Find("TetherAttachPoint");
                sb.AppendLine("- TetherAttachPoint: " + (tether != null ? "SIM" : "NAO"));
            }

            var tetherObj = GameObject.Find("Tether");
            if (tetherObj != null)
            {
                sb.AppendLine();
                sb.AppendLine("## Guia (Tether)");
                var tetherSys = tetherObj.GetComponent<Canicross.Player.TetherSystem>();
                sb.AppendLine("- TetherSystem: " + (tetherSys != null ? "SIM" : "NAO"));
                var lr = tetherObj.GetComponent<LineRenderer>();
                sb.AppendLine("- LineRenderer: " + (lr != null ? "SIM" : "NAO"));
            }

            var trackObjs = Object.FindObjectsByType<CheckpointTrigger>(FindObjectsSortMode.None);
            sb.AppendLine();
            sb.AppendLine("## Cenario");
            sb.AppendLine("- Checkpoints: " + (trackObjs != null ? trackObjs.Length.ToString() : "0"));

            var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            sb.AppendLine("- Total GameObjects: " + (allObjects != null ? allObjects.Length.ToString() : "0"));

            string reportPath = Path.Combine(testDir, "test_report.md");
            File.WriteAllText(reportPath, sb.ToString());
            Debug.Log("[AutoTester] Relatorio salvo em: " + reportPath);
        }
    }
}