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
            runner.transform.position = new Vector3(0, 0f, 5f);
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

            if (dogAttach != null && runnerAttach != null)
            {
                tetherSys.SetPoints(dogAttach, runnerAttach);
            }
            else
            {
                Debug.LogWarning("[Canicross] TetherAttachPoint not found on dog or runner. Connecting to transforms directly.");
                tetherSys.SetPoints(dog.transform, runner.transform);
            }

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

            Rigidbody dogRb = dog.GetComponent<Rigidbody>();
            CharacterController runnerCC = runner.GetComponent<CharacterController>();

            GameObject gameManager = new GameObject("GameManager");
            Core.GameManager gm = gameManager.AddComponent<Core.GameManager>();
            Core.RaceManager raceMgr = gameManager.AddComponent<Core.RaceManager>();

            SafeSetProperty(gm, "dogController", dogCtrl);
            SafeSetProperty(gm, "runnerController", runnerCtrl);
            SafeSetProperty(gm, "staminaSystem", stamina);
            SafeSetProperty(gm, "bondSystem", bond);
            SafeSetProperty(gm, "raceManager", raceMgr);
            SafeSetProperty(gm, "hudController", hudCtrl);

            SafeSetProperty(dogCtrl, "staminaSystem", stamina);
            SafeSetProperty(dogCtrl, "rb", dogRb);
            SafeSetProperty(dogCtrl, "tetherSystem", tetherSys);

            SafeSetProperty(runnerCtrl, "dogTransform", dog.transform);
            SafeSetProperty(runnerCtrl, "staminaSystem", stamina);
            SafeSetProperty(runnerCtrl, "charController", runnerCC);
            SafeSetProperty(runnerCtrl, "tetherSystem", tetherSys);

            SafeSetProperty(povCam, "dogController", dogCtrl);
            SafeSetProperty(povCam, "target", dog.transform);

            SafeSetProperty(speed, "dogController", dogCtrl);

            if (bond != null)
            {
                SafeSetProperty(bond, "staminaSystem", stamina);
                SafeSetProperty(bond, "dogController", dogCtrl);
            }

            if (input != null)
            {
                SafeSetProperty(input, "dogController", dogCtrl);
                SafeSetProperty(input, "bondSystem", bond);
            }

            if (hudCtrl != null)
            {
                SafeSetProperty(hudCtrl, "staminaSystem", stamina);
                SafeSetProperty(hudCtrl, "speedSystem", speed);
                SafeSetProperty(hudCtrl, "bondSystem", bond);
                SafeSetProperty(hudCtrl, "raceManager", raceMgr);
                SafeSetProperty(hudCtrl, "dogController", dogCtrl);
            }

            Transform dogAttach = dog.transform.Find("TetherAttachPoint");
            Transform runnerAttach = runner.transform.Find("TetherAttachPoint");
            if (dogAttach != null && runnerAttach != null)
            {
                SafeSetProperty(tetherSys, "dogPoint", dogAttach);
                SafeSetProperty(tetherSys, "runnerPoint", runnerAttach);
            }

            Debug.Log("[Canicross] All references wired up.");
        }

        private static void SafeSetProperty(Object obj, string propertyName, Object value)
        {
            if (obj == null)
            {
                Debug.LogWarning($"[Canicross] Cannot set {propertyName}: target object is null");
                return;
            }
            if (value == null && propertyName != "rb")
            {
                Debug.LogWarning($"[Canicross] Setting {propertyName} to null on {obj.GetType().Name}");
            }

            SerializedObject so = new SerializedObject(obj);
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.objectReferenceValue = value;
                so.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning($"[Canicross] Property '{propertyName}' not found on {obj.GetType().Name}");
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

        private static void WireUpHUDReferences(HUDController hudCtrl, StaminaBar dogBar, StaminaBar runnerBar,
            TimerDisplay timerDisplay, GameObject speedGo, GameObject bondGo, GameObject dogNameGo,
            GameObject dogStaminaGo, GameObject runnerStaminaGo)
        {
            if (hudCtrl == null) return;

            SafeSetProperty(hudCtrl, "dogStaminaBar", dogBar);
            SafeSetProperty(hudCtrl, "runnerStaminaBar", runnerBar);
            SafeSetProperty(hudCtrl, "timerDisplay", timerDisplay);
            SafeSetProperty(hudCtrl, "speedText", speedGo != null ? speedGo.GetComponent<UnityEngine.UI.Text>() : null);
            SafeSetProperty(hudCtrl, "bondText", bondGo != null ? bondGo.GetComponent<UnityEngine.UI.Text>() : null);
            SafeSetProperty(hudCtrl, "dogNameText", dogNameGo != null ? dogNameGo.GetComponent<UnityEngine.UI.Text>() : null);

            if (dogStaminaGo != null)
            {
                Transform fillTr = dogStaminaGo.transform.Find("Fill");
                if (fillTr != null)
                {
                    SafeSetProperty(dogBar, "fillImage", fillTr.GetComponent<UnityEngine.UI.Image>());
                }
            }

            if (runnerStaminaGo != null)
            {
                Transform fillTr = runnerStaminaGo.transform.Find("Fill");
                if (fillTr != null)
                {
                    SafeSetProperty(runnerBar, "fillImage", fillTr.GetComponent<UnityEngine.UI.Image>());
                }
            }
        }
    }
}