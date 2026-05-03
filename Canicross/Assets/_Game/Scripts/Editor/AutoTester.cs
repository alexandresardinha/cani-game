using UnityEngine;
using UnityEditor;
using System.IO;

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
                CaptureScreenshot();
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
            if (frameCount < 3) return;
            EditorApplication.update -= WaitAndCapture;
            CaptureScreenshot();
        }

        private static void CaptureScreenshot()
        {
            Debug.Log("[AutoTester] Capturando screenshot...");

            int width = 1280;
            int height = 720;

            var cam = GameObject.Find("ScreenshotCamera")?.GetComponent<Camera>();
            if (cam == null) cam = Camera.main;

            if (cam == null)
            {
                Debug.LogError("[AutoTester] Nenhuma camera encontrada!");
                if (Application.isBatchMode) EditorApplication.Exit(1);
                return;
            }

            var rt = new RenderTexture(width, height, 24);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string testDir = Path.Combine(projectRoot, "TestResults");
            Directory.CreateDirectory(testDir);

            string screenshotPath = Path.Combine(testDir, "dog_test_result.png");
            File.WriteAllBytes(screenshotPath, tex.EncodeToPNG());

            Debug.Log("[AutoTester] Screenshot salvo em: " + screenshotPath);

            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(tex);

            GenerateReport(testDir);

            Debug.Log("[AutoTester] Teste concluido!");
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }

        private static void GenerateReport(string testDir)
        {
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

                var renderer = dog.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    string matName = renderer.sharedMaterial != null ? renderer.sharedMaterial.name : "N/A";
                    sb.AppendLine("- Material: " + matName);
                    sb.AppendLine("- Bounds: " + renderer.bounds.ToString());
                }

                var animator = dog.GetComponentInChildren<Animator>();
                sb.AppendLine("- Animator: " + (animator != null ? "SIM" : "NAO"));

                var rb = dog.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    sb.AppendLine("- Massa: " + rb.mass.ToString());
                    sb.AppendLine("- Use Gravity: " + rb.useGravity.ToString());
                }

                var dogCtrl = dog.GetComponent<DogController>();
                if (dogCtrl != null)
                {
                    sb.AppendLine("- DogController: SIM");
                }

                var tether = dog.transform.Find("TetherAttachPoint");
                sb.AppendLine("- TetherAttachPoint: " + (tether != null ? "SIM" : "NAO"));
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

            var trackObjs = GameObject.FindObjectsOfType<Canicross.Environment.CheckpointTrigger>();
            sb.AppendLine();
            sb.AppendLine("## Cenario");
            sb.AppendLine("- Checkpoints: " + trackObjs.Length);

            string reportPath = Path.Combine(testDir, "test_report.md");
            File.WriteAllText(reportPath, sb.ToString());
            Debug.Log("[AutoTester] Relatorio salvo em: " + reportPath);
        }
    }
}