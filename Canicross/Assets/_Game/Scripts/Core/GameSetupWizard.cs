using UnityEngine;
using UnityEditor;
using Canicross.Player;
using Canicross.Systems;
using Canicross.Cam;
using Canicross.UI;
using Canicross.Environment;

namespace Canicross.Editor
{
    public static class GameSetupWizard
    {
        [MenuItem("Canicross/Setup Game Scene")]
        public static void SetupGameScene()
        {
            GameObject gameManager = CreateGameObject("GameManager", null);
            gameManager.AddComponent<Core.GameManager>();
            gameManager.AddComponent<Core.RaceManager>();

            GameObject dog = CreatePrimitiveObject("Dog", PrimitiveType.Capsule, "Dog");
            Rigidbody dogRb = dog.AddComponent<Rigidbody>();
            dogRb.linearDamping = 1f;
            dogRb.angularDamping = 1f;
            dogRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Player.DogController dogCtrl = dog.AddComponent<Player.DogController>();
            dog.transform.position = new Vector3(0, 0.5f, 8f);

            GameObject runner = CreatePrimitiveObject("Runner", PrimitiveType.Cylinder, "Runner");
            // Remove the primitive collider — CharacterController has its own
            CapsuleCollider runnerCol = runner.GetComponent<CapsuleCollider>();
            if (runnerCol != null) Object.DestroyImmediate(runnerCol);
            CharacterController runnerCC = runner.AddComponent<CharacterController>();
            runnerCC.height = 1.7f;
            runnerCC.radius = 0.3f;
            runnerCC.center = new Vector3(0, 0.85f, 0);
            Player.RunnerController runnerCtrl = runner.AddComponent<Player.RunnerController>();
            runner.transform.position = new Vector3(0, 0, 0);
            runner.transform.localScale = new Vector3(0.5f, 0.85f, 0.25f);

            GameObject tether = new GameObject("Tether");
            LineRenderer lr = tether.AddComponent<LineRenderer>();
            Player.TetherSystem tetherSys = tether.AddComponent<Player.TetherSystem>();

            Transform dogAttach = new GameObject("AttachPoint").transform;
            dogAttach.SetParent(dog.transform);
            dogAttach.localPosition = new Vector3(0, 0.3f, -0.5f);

            Transform runnerAttach = new GameObject("AttachPoint").transform;
            runnerAttach.SetParent(runner.transform);
            runnerAttach.localPosition = new Vector3(0, 0.8f, 0.2f);

            tetherSys.SetPoints(dogAttach, runnerAttach);

            GameObject systems = CreateGameObject("Systems", null);
            systems.AddComponent<Systems.StaminaSystem>();
            systems.AddComponent<Systems.SpeedSystem>();
            systems.AddComponent<Player.BondSystem>();
            systems.AddComponent<Player.InputHandler>();

            GameObject cameraRig = new GameObject("CameraRig");
            cameraRig.AddComponent<UnityEngine.Camera>();
            Cam.POVCamera povCam = cameraRig.AddComponent<Cam.POVCamera>();
            cameraRig.transform.position = new Vector3(0, 1.6f, -3f);

            GameObject hud = CreateCanvas("HUD");
            GameObject[] hudElements = SetupHUD(hud);

            HUDController hudCtrl = hud.GetComponent<HUDController>();

            WireUpReferences(gameManager, dogCtrl, runnerCtrl, tetherSys, povCam, hudCtrl, hudElements);

            // Create ground plane for jump/obstacle testing
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0, 0, 0);
            ground.transform.localScale = new Vector3(200, 1, 200);
            ground.GetComponent<Renderer>().material.color = new Color(0.35f, 0.25f, 0.15f);

            // Configure ground layer for jump detection (Default = bit 0 = value 1)
            SerializedObject dogCtrlSo = new SerializedObject(dogCtrl);
            dogCtrlSo.FindProperty("groundLayer").intValue = 1;
            dogCtrlSo.ApplyModifiedProperties();

            Debug.Log("[Canicross] Game scene setup complete! Press Play to test.");
        }

        private static GameObject CreateGameObject(string name, string tag)
        {
            GameObject go = new GameObject(name);
            if (!string.IsNullOrEmpty(tag))
            {
                try { go.tag = tag; }
                catch { Debug.LogWarning($"Tag '{tag}' not found. Add it manually."); }
            }
            return go;
        }

        private static GameObject CreatePrimitiveObject(string name, PrimitiveType type, string tag)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.GetComponent<Renderer>().material.color = new Color(0.11f, 0.11f, 0.11f);
            if (!string.IsNullOrEmpty(tag))
            {
                try { go.tag = tag; }
                catch { Debug.LogWarning($"Tag '{tag}' not found. Add it manually."); }
            }
            return go;
        }

        private static GameObject CreateCanvas(string name)
        {
            GameObject go = new GameObject(name);
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<UnityEngine.UI.CanvasScaler>();
            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return go;
        }

        private static GameObject[] SetupHUD(GameObject hud)
        {
            Font arial = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject dogStaminaGo = CreateHUDBar(hud, "DogStamina", new Vector2(20, -20),
                new Vector2(200, 12), "#00BFFF", arial, "⚡", TextAnchor.MiddleLeft, Vector2.up);
            StaminaBar dogBar = dogStaminaGo.AddComponent<StaminaBar>();

            GameObject runnerStaminaGo = CreateHUDBar(hud, "RunnerStamina", new Vector2(20, -40),
                new Vector2(200, 10), "#FFB347", arial, "💨", TextAnchor.MiddleLeft, Vector2.up);
            StaminaBar runnerBar = runnerStaminaGo.AddComponent<StaminaBar>();

            GameObject timerGo = CreateHUDText(hud, "Timer", new Vector2(0, 10),
                "00:00.00", 28, arial, TextAnchor.UpperCenter, Vector2.up);
            TimerDisplay timerDisplay = timerGo.AddComponent<TimerDisplay>();

            GameObject speedGo = CreateHUDText(hud, "Speed", new Vector2(20, 20),
                "▼ 0 km/h", 18, arial, TextAnchor.LowerLeft, Vector2.zero);
            GameObject bondGo = CreateHUDText(hud, "Bond", new Vector2(-20, 20),
                "🔗 50", 18, arial, TextAnchor.LowerRight, Vector2.right);
            GameObject dogNameGo = CreateHUDText(hud, "DogName", new Vector2(0, -5),
                "MIA", 22, arial, TextAnchor.UpperCenter, Vector2.up);

            HUDController hudCtrl = hud.AddComponent<HUDController>();

            WireUpHUDReferences(hudCtrl, dogBar, runnerBar, timerDisplay, speedGo, bondGo, dogNameGo, dogStaminaGo, runnerStaminaGo);

            return new GameObject[] { speedGo, bondGo };
        }

        private static void WireUpHUDReferences(HUDController hudCtrl, StaminaBar dogBar, StaminaBar runnerBar,
            TimerDisplay timerDisplay, GameObject speedGo, GameObject bondGo, GameObject dogNameGo,
            GameObject dogStaminaGo, GameObject runnerStaminaGo)
        {
            SerializedObject hudSo = new SerializedObject(hudCtrl);
            hudSo.FindProperty("dogStaminaBar").objectReferenceValue = dogBar;
            hudSo.FindProperty("runnerStaminaBar").objectReferenceValue = runnerBar;
            hudSo.FindProperty("timerDisplay").objectReferenceValue = timerDisplay;
            hudSo.FindProperty("speedText").objectReferenceValue = speedGo.GetComponent<UnityEngine.UI.Text>();
            hudSo.FindProperty("bondText").objectReferenceValue = bondGo.GetComponent<UnityEngine.UI.Text>();
            hudSo.FindProperty("dogNameText").objectReferenceValue = dogNameGo != null ? dogNameGo.GetComponent<UnityEngine.UI.Text>() : null;
            hudSo.ApplyModifiedProperties();

            // Wire up StaminaBar fill images
            if (dogStaminaGo != null)
            {
                Transform fillTr = dogStaminaGo.transform.Find("Fill");
                if (fillTr != null)
                {
                    SerializedObject dogBarSo = new SerializedObject(dogBar);
                    dogBarSo.FindProperty("fillImage").objectReferenceValue = fillTr.GetComponent<UnityEngine.UI.Image>();
                    dogBarSo.ApplyModifiedProperties();
                }
            }

            if (runnerStaminaGo != null)
            {
                Transform fillTr = runnerStaminaGo.transform.Find("Fill");
                if (fillTr != null)
                {
                    SerializedObject runnerBarSo = new SerializedObject(runnerBar);
                    runnerBarSo.FindProperty("fillImage").objectReferenceValue = fillTr.GetComponent<UnityEngine.UI.Image>();
                    runnerBarSo.ApplyModifiedProperties();
                }
            }
        }

        private static GameObject CreateHUDBar(GameObject parent, string name, Vector2 anchoredPos,
            Vector2 size, string hexColor, Font font, string label, TextAnchor align, Vector2 pivot)
        {
            GameObject go = new GameObject(name, typeof(UnityEngine.UI.Image));
            go.transform.SetParent(parent.transform, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = pivot;
            rt.anchorMax = pivot;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            ColorUtility.TryParseHtmlString(hexColor, out Color color);
            go.GetComponent<UnityEngine.UI.Image>().color = color;

            GameObject fill = new GameObject("Fill", typeof(UnityEngine.UI.Image));
            fill.transform.SetParent(go.transform, false);
            RectTransform fillRt = fill.GetComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.sizeDelta = Vector2.zero;
            fill.GetComponent<UnityEngine.UI.Image>().color = Color.white;

            if (!string.IsNullOrEmpty(label))
            {
                GameObject labelGo = new GameObject("Label", typeof(UnityEngine.UI.Text));
                labelGo.transform.SetParent(parent.transform, false);
                RectTransform labelRt = labelGo.GetComponent<RectTransform>();
                labelRt.anchorMin = pivot;
                labelRt.anchorMax = pivot;
                labelRt.pivot = Vector2.one * 0.5f;
                labelRt.anchoredPosition = anchoredPos + new Vector2(-30, 0);
                labelRt.sizeDelta = new Vector2(50, 20);
                UnityEngine.UI.Text text = labelGo.GetComponent<UnityEngine.UI.Text>();
                text.text = label;
                text.font = font;
                text.fontSize = 12;
                text.alignment = align;
                text.color = Color.white;
            }

            return go;
        }

        private static GameObject CreateHUDText(GameObject parent, string name, Vector2 anchoredPos,
            string defaultText, int fontSize, Font font, TextAnchor align, Vector2 pivot)
        {
            GameObject go = new GameObject(name, typeof(UnityEngine.UI.Text));
            go.transform.SetParent(parent.transform, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = pivot;
            rt.anchorMax = pivot;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(250, 30);

            UnityEngine.UI.Text text = go.GetComponent<UnityEngine.UI.Text>();
            text.text = defaultText;
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = align;
            text.color = Color.white;
            text.fontStyle = FontStyle.Bold;

            return go;
        }

        private static void WireUpReferences(GameObject gameManagerGo, DogController dogCtrl,
            RunnerController runnerCtrl, TetherSystem tetherSys, Cam.POVCamera povCam,
            HUDController hudCtrl, GameObject[] hudExtra)
        {
            StaminaSystem stamina = Object.FindFirstObjectByType<StaminaSystem>();
            BondSystem bond = Object.FindFirstObjectByType<BondSystem>();
            SpeedSystem speed = Object.FindFirstObjectByType<SpeedSystem>();
            Core.RaceManager raceMgr = Object.FindFirstObjectByType<Core.RaceManager>();
            InputHandler input = Object.FindFirstObjectByType<InputHandler>();

            SerializedObject gmSo = new SerializedObject(gameManagerGo.GetComponent<Core.GameManager>());
            gmSo.FindProperty("dogController").objectReferenceValue = dogCtrl;
            gmSo.FindProperty("runnerController").objectReferenceValue = runnerCtrl;
            gmSo.FindProperty("staminaSystem").objectReferenceValue = stamina;
            gmSo.FindProperty("bondSystem").objectReferenceValue = bond;
            gmSo.FindProperty("raceManager").objectReferenceValue = raceMgr;
            gmSo.FindProperty("hudController").objectReferenceValue = hudCtrl;
            gmSo.ApplyModifiedProperties();

            SerializedObject dogSo = new SerializedObject(dogCtrl);
            dogSo.FindProperty("staminaSystem").objectReferenceValue = stamina;
            dogSo.FindProperty("rb").objectReferenceValue = dogCtrl.GetComponent<Rigidbody>();
            dogSo.ApplyModifiedProperties();

            SerializedObject runnerSo = new SerializedObject(runnerCtrl);
            runnerSo.FindProperty("dogTransform").objectReferenceValue = dogCtrl.transform;
            runnerSo.FindProperty("staminaSystem").objectReferenceValue = stamina;
            runnerSo.FindProperty("charController").objectReferenceValue = runnerCtrl.GetComponent<CharacterController>();
            runnerSo.ApplyModifiedProperties();

            SerializedObject povSo = new SerializedObject(povCam);
            povSo.FindProperty("dogController").objectReferenceValue = dogCtrl;
            povSo.FindProperty("target").objectReferenceValue = dogCtrl.transform;
            povSo.ApplyModifiedProperties();

            SerializedObject speedSo = new SerializedObject(speed);
            speedSo.FindProperty("dogController").objectReferenceValue = dogCtrl;
            speedSo.ApplyModifiedProperties();

            if (bond != null)
            {
                SerializedObject bondSo = new SerializedObject(bond);
                bondSo.FindProperty("staminaSystem").objectReferenceValue = stamina;
                bondSo.FindProperty("dogController").objectReferenceValue = dogCtrl;
                bondSo.ApplyModifiedProperties();
            }

            if (input != null)
            {
                SerializedObject inputSo = new SerializedObject(input);
                inputSo.FindProperty("dogController").objectReferenceValue = dogCtrl;
                inputSo.FindProperty("bondSystem").objectReferenceValue = bond;
                inputSo.ApplyModifiedProperties();
            }

            if (hudCtrl != null)
            {
                SerializedObject hudSo = new SerializedObject(hudCtrl);
                hudSo.FindProperty("staminaSystem").objectReferenceValue = stamina;
                hudSo.FindProperty("speedSystem").objectReferenceValue = speed;
                hudSo.FindProperty("bondSystem").objectReferenceValue = bond;
                hudSo.FindProperty("raceManager").objectReferenceValue = raceMgr;
                hudSo.FindProperty("dogController").objectReferenceValue = dogCtrl;
                hudSo.ApplyModifiedProperties();
            }

            Debug.Log("[Canicross] All references wired up automatically.");
        }
    }
}
