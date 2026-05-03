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
            Debug.Log("[Canicross] Setting up game scene...");

            GameObject dog = CreateDog();
            GameObject runner = CreateRunner();
            GameObject tether = CreateTether(dog, runner);
            GameObject systems = CreateSystems();
            GameObject camera = CreateCamera(dog);
            GameObject hud = CreateHUD();

            WireUpReferences(dog, runner, tether, systems, camera, hud);

            Environment.TrackGenerator.GenerateLakeParkTrack(null);

            Debug.Log("[Canicross] Game scene setup complete! Press Play to test.");
        }

        private static GameObject CreateDog()
        {
            GameObject dog = Player.CharacterBuilder.BuildDog();
            dog.transform.position = new Vector3(0, 0.3f, 8f);
            dog.transform.rotation = Quaternion.identity;
            return dog;
        }

        private static GameObject CreateRunner()
        {
            GameObject runner = Player.CharacterBuilder.BuildRunner();
            runner.transform.position = new Vector3(0, 0f, 0f);
            runner.transform.rotation = Quaternion.identity;
            return runner;
        }

        private static GameObject CreateTether(GameObject dog, GameObject runner)
        {
            GameObject tether = new GameObject("Tether");
            LineRenderer lr = tether.AddComponent<LineRenderer>();
            Player.TetherSystem tetherSys = tether.AddComponent<Player.TetherSystem>();

            Transform dogAttach = dog.transform.Find("TetherAttachPoint");
            Transform runnerAttach = runner.transform.Find("TetherAttachPoint");

            tetherSys.SetPoints(dogAttach, runnerAttach);

            return tether;
        }

        private static GameObject CreateSystems()
        {
            GameObject systems = new GameObject("Systems");
            systems.AddComponent<Systems.StaminaSystem>();
            systems.AddComponent<Systems.SpeedSystem>();
            systems.AddComponent<Player.BondSystem>();
            systems.AddComponent<Player.InputHandler>();
            return systems;
        }

        private static GameObject CreateCamera(GameObject dog)
        {
            GameObject cameraRig = new GameObject("CameraRig");
            cameraRig.AddComponent<UnityEngine.Camera>();
            Cam.POVCamera povCam = cameraRig.AddComponent<Cam.POVCamera>();
            cameraRig.transform.position = new Vector3(0, 2.5f, 4f);

            return cameraRig;
        }

        private static GameObject CreateHUD()
        {
            GameObject hud = new GameObject("HUD");
            Canvas canvas = hud.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            hud.AddComponent<UnityEngine.UI.CanvasScaler>();
            hud.AddComponent<UnityEngine.UI.GraphicRaycaster>();

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
                "ADAO", 22, arial, TextAnchor.UpperCenter, Vector2.up);

            HUDController hudCtrl = hud.AddComponent<HUDController>();

            WireUpHUDReferences(hudCtrl, dogBar, runnerBar, timerDisplay, speedGo, bondGo, dogNameGo, dogStaminaGo, runnerStaminaGo);

            return hud;
        }

        private static GameObject CreateGameManager(GameObject dog, GameObject runner, GameObject tether, GameObject systems, GameObject camera, GameObject hud)
        {
            GameObject gameManager = new GameObject("GameManager");
            Core.GameManager gm = gameManager.AddComponent<Core.GameManager>();
            Core.RaceManager rm = gameManager.AddComponent<Core.RaceManager>();
            return gameManager;
        }

        private static void WireUpReferences(GameObject dog, GameObject runner, GameObject tether,
            GameObject systems, GameObject camera, GameObject hud)
        {
            DogController dogCtrl = dog.GetComponent<DogController>();
            RunnerController runnerCtrl = runner.GetComponent<RunnerController>();
            TetherSystem tetherSys = tether.GetComponent<TetherSystem>();
            POVCamera povCam = camera.GetComponent<POVCamera>();
            HUDController hudCtrl = hud.GetComponent<HUDController>();
            StaminaSystem stamina = systems.GetComponent<StaminaSystem>();
            SpeedSystem speed = systems.GetComponent<SpeedSystem>();
            BondSystem bond = systems.GetComponent<BondSystem>();
            InputHandler input = systems.GetComponent<InputHandler>();

            GameObject gameManager = new GameObject("GameManager");
            Core.GameManager gm = gameManager.AddComponent<Core.GameManager>();
            Core.RaceManager raceMgr = gameManager.AddComponent<Core.RaceManager>();

            SerializedObject gmSo = new SerializedObject(gm);
            gmSo.FindProperty("dogController").objectReferenceValue = dogCtrl;
            gmSo.FindProperty("runnerController").objectReferenceValue = runnerCtrl;
            gmSo.FindProperty("staminaSystem").objectReferenceValue = stamina;
            gmSo.FindProperty("bondSystem").objectReferenceValue = bond;
            gmSo.FindProperty("raceManager").objectReferenceValue = raceMgr;
            gmSo.FindProperty("hudController").objectReferenceValue = hudCtrl;
            gmSo.ApplyModifiedProperties();

            SerializedObject dogSo = new SerializedObject(dogCtrl);
            dogSo.FindProperty("staminaSystem").objectReferenceValue = stamina;
            dogSo.FindProperty("rb").objectReferenceValue = dog.GetComponent<Rigidbody>();
            dogSo.FindProperty("tetherSystem").objectReferenceValue = tetherSys;
            dogSo.ApplyModifiedProperties();

            SerializedObject runnerSo = new SerializedObject(runnerCtrl);
            runnerSo.FindProperty("dogTransform").objectReferenceValue = dog.transform;
            runnerSo.FindProperty("staminaSystem").objectReferenceValue = stamina;
            runnerSo.FindProperty("charController").objectReferenceValue = runner.GetComponent<CharacterController>();
            runnerSo.FindProperty("tetherSystem").objectReferenceValue = tetherSys;
            runnerSo.ApplyModifiedProperties();

            SerializedObject povSo = new SerializedObject(povCam);
            povSo.FindProperty("dogController").objectReferenceValue = dogCtrl;
            povSo.FindProperty("target").objectReferenceValue = dog.transform;
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

            SerializedObject tmSo = new SerializedObject(raceMgr);
            tmSo.ApplyModifiedProperties();

            SerializedObject tetherSo = new SerializedObject(tetherSys);
            tetherSo.FindProperty("dogPoint").objectReferenceValue = dog.transform.Find("TetherAttachPoint");
            tetherSo.FindProperty("runnerPoint").objectReferenceValue = runner.transform.Find("TetherAttachPoint");
            tetherSo.ApplyModifiedProperties();

            Debug.Log("[Canicross] All references wired up automatically.");
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
    }
}