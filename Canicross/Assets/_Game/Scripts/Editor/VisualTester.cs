using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Canicross.Player;
using Canicross.Environment;

namespace Canicross.Editor
{
    public static class VisualTester
    {
        public struct CameraShot
        {
            public string Name;
            public Vector3 Position;
            public Vector3 LookAt;
            public string Description;
        }

        [MenuItem("Canicross/Run Visual Tests")]
        public static void RunVisualTests()
        {
            Debug.Log("[VisualTester] Starting visual test suite...");

            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Single);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.8f, 0.8f, 0.9f);

            // Setup the full game scene
            GameSetupWizard.SetupGameScene();

            // Wait a frame then capture
            EditorApplication.update += RunTestSequence;
        }

        private static int testPhase = 0;
        private static int frameDelay = 0;
        private static List<string> capturedImages = new List<string>();
        private static List<CameraShot> shots;
        private static int currentShot = 0;
        private static Camera testCam;

        private static void RunTestSequence()
        {
            frameDelay++;
            if (frameDelay < 5) return;

            if (testPhase == 0)
            {
                InitializeShots();
                testCam = SetupTestCamera();
                testPhase = 1;
                frameDelay = 0;
                return;
            }

            if (testPhase == 1)
            {
                if (currentShot < shots.Count)
                {
                    CaptureShot(shots[currentShot]);
                    currentShot++;
                    frameDelay = 0;
                }
                else
                {
                    testPhase = 2;
                    frameDelay = 0;
                }
                return;
            }

            if (testPhase == 2)
            {
                GenerateVisualReport();
                Cleanup();
                EditorApplication.update -= RunTestSequence;
                Debug.Log("[VisualTester] All visual tests completed!");
            }
        }

        private static void InitializeShots()
        {
            var dog = GameObject.Find("Dog");
            var runner = GameObject.Find("Runner");
            Vector3 dogPos = dog != null ? dog.transform.position : new Vector3(0, 0.6f, 8f);
            Vector3 runnerPos = runner != null ? runner.transform.position : new Vector3(0, 0, 5f);
            Vector3 center = (dogPos + runnerPos) * 0.5f;

            shots = new List<CameraShot>
            {
                new CameraShot {
                    Name = "01_SideView",
                    Position = new Vector3(5, 2, center.z),
                    LookAt = center,
                    Description = "Vista lateral - verifica se cachorro e humano sao reconheciveis e guia elastica esta visivel"
                },
                new CameraShot {
                    Name = "02_FrontView",
                    Position = new Vector3(center.x, 1.5f, center.z - 5),
                    LookAt = center,
                    Description = "Vista frontal - verifica proporcoes e posicionamento"
                },
                new CameraShot {
                    Name = "03_TopView",
                    Position = new Vector3(center.x, 8, center.z + 2),
                    LookAt = center,
                    Description = "Vista de cima - verifica alinhamento da trilha e posicionamento dos personagens"
                },
                new CameraShot {
                    Name = "04_CloseUp_Dog",
                    Position = dogPos + new Vector3(1.5f, 0.5f, 1.5f),
                    LookAt = dogPos + Vector3.up * 0.3f,
                    Description = "Close-up do cachorro - verifica se nao esta encaixado no chao"
                },
                new CameraShot {
                    Name = "05_CloseUp_Runner",
                    Position = runnerPos + new Vector3(1.2f, 0.8f, 1.2f),
                    LookAt = runnerPos + Vector3.up * 0.9f,
                    Description = "Close-up do corredor - verifica detalhes do personagem"
                },
                new CameraShot {
                    Name = "06_TetherDetail",
                    Position = center + new Vector3(0.5f, 1.5f, -2f),
                    LookAt = center + Vector3.up * 0.5f,
                    Description = "Detalhe da guia elastica - verifica conexao e curva"
                },
                new CameraShot {
                    Name = "07_Environment_Wide",
                    Position = new Vector3(-3, 4, 15),
                    LookAt = new Vector3(0, 0, 50),
                    Description = "Panoramica do cenario - verifica lago, arvores e trilha"
                },
                new CameraShot {
                    Name = "08_Menu_Preview",
                    Position = new Vector3(0, 1.5f, -2f),
                    LookAt = new Vector3(0, 0.2f, 8f),
                    Description = "Preview do menu - verifica silhuetas e ambiente 3D"
                }
            };
        }

        private static Camera SetupTestCamera()
        {
            GameObject camGo = new GameObject("VisualTestCamera");
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.5f, 0.7f, 0.9f);
            cam.fieldOfView = 60;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 200f;
            return cam;
        }

        private static void CaptureShot(CameraShot shot)
        {
            testCam.transform.position = shot.Position;
            testCam.transform.LookAt(shot.LookAt);

            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string testDir = Path.Combine(projectRoot, "TestResults", "Visual");
            Directory.CreateDirectory(testDir);
            string path = Path.Combine(testDir, shot.Name + ".png");

            int width = 1280;
            int height = 720;

            RenderTexture rt = new RenderTexture(width, height, 32, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 2;
            rt.Create();
            testCam.targetTexture = rt;
            testCam.Render();
            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            capturedImages.Add(shot.Name + "|" + shot.Description + "|" + path);

            testCam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(tex);

            Debug.Log($"[VisualTester] Captured: {shot.Name} -> {path}");
        }

        private static void GenerateVisualReport()
        {
            string projectRoot = Path.GetDirectoryName(Application.dataPath);
            string testDir = Path.Combine(projectRoot, "TestResults", "Visual");
            Directory.CreateDirectory(testDir);

            var reportPath = Path.Combine(testDir, "visual_report.html");
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html><head><meta charset='utf-8'><title>Canicross - Visual Test Report</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: sans-serif; background: #1a1a2e; color: #eee; padding: 20px; }");
            sb.AppendLine("h1 { color: #00BFFF; }");
            sb.AppendLine(".shot { margin: 20px 0; padding: 15px; background: #16213e; border-radius: 8px; }");
            sb.AppendLine(".shot img { max-width: 100%; border-radius: 4px; border: 2px solid #00BFFF; }");
            sb.AppendLine(".shot h3 { margin-top: 0; color: #DAA520; }");
            sb.AppendLine(".shot p { color: #aaa; }");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine("<h1>Canicross - Relatorio de Testes Visuais</h1>");
            sb.AppendLine("<p>Data: " + System.DateTime.Now.ToString() + "</p>");
            sb.AppendLine("<hr>");

            foreach (var img in capturedImages)
            {
                var parts = img.Split('|');
                string name = parts[0];
                string desc = parts[1];
                string path = parts[2];
                string relPath = System.Uri.EscapeUriString(Path.GetFileName(path));

                sb.AppendLine($"<div class='shot'>");
                sb.AppendLine($"<h3>{name}</h3>");
                sb.AppendLine($"<p>{desc}</p>");
                sb.AppendLine($"<img src='{relPath}' alt='{name}' />");
                sb.AppendLine($"</div>");
            }

            sb.AppendLine("</body></html>");
            File.WriteAllText(reportPath, sb.ToString());

            Debug.Log("[VisualTester] Report generated: " + reportPath);
        }

        private static void Cleanup()
        {
            if (testCam != null)
                Object.DestroyImmediate(testCam.gameObject);
        }
    }
}
