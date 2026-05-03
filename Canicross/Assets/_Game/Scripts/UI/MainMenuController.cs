using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Canicross.Core;

namespace Canicross.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color accentColor = new Color(0f, 0.75f, 1f);
        [SerializeField] private Color warmColor = new Color(0.85f, 0.64f, 0.13f);
        [SerializeField] private Color darkPanelColor = new Color(0.05f, 0.05f, 0.08f, 0.85f);

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 1.2f;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float titleBounceDuration = 1.0f;

        private CanvasGroup canvasGroup;
        private GameObject titleText;
        private GameObject playButton;
        private GameObject subtitleText;
        private GameObject creditsText;
        private GameObject panelBg;

        private void Start()
        {
            BuildMenu();
            StartCoroutine(FadeIn());
            StartCoroutine(TitleBounce());
        }

        private void BuildMenu()
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Dark semi-transparent panel behind menu elements
            panelBg = CreatePanel("MenuPanel", Vector2.zero, new Vector2(480, 360), darkPanelColor);

            // Title with larger size and warm color
            titleText = CreateText("Title", "CANICROSS", new Vector2(0, 110),
                80, font, TextAnchor.LowerCenter, warmColor, Vector2.up);

            // Subtitle
            subtitleText = CreateText("Subtitle", "Corra com seu cao pela trilha", new Vector2(0, 70),
                20, font, TextAnchor.MiddleCenter, Color.white, Vector2.up);

            // Play button with rounded feel (using Image)
            playButton = CreatePlayButton(font);

            // Credits at bottom
            creditsText = CreateText("Credits", "Feito com amor por humanos e caes",
                new Vector2(0, -140), 13, font, TextAnchor.LowerCenter,
                new Color(1f, 1f, 1f, 0.5f), Vector2.up);

            // Character names label
            GameObject dogLabel = CreateText("DogName", "ADAO  x  ALEXANDRE", new Vector2(0, 90),
                16, font, TextAnchor.MiddleCenter, accentColor, Vector2.up);

            // Decorative lines
            CreateDecorativeLine(new Vector2(-140, 95), 280, accentColor, 2.5f);
            CreateDecorativeLine(new Vector2(-140, 55), 280, accentColor, 1.5f);

            // Small instruction text
            GameObject instruction = CreateText("Instruction", "Use WASD ou Setas para mover | Espaco para pular",
                new Vector2(0, -110), 12, font, TextAnchor.MiddleCenter,
                new Color(1f, 1f, 1f, 0.6f), Vector2.up);
        }

        private GameObject CreatePanel(string name, Vector2 pos, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name, typeof(Image));
            go.transform.SetParent(transform, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.up;
            rt.anchorMax = Vector2.up;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = go.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = false;

            return go;
        }

        private GameObject CreateText(string name, string text, Vector2 pos, int size,
            Font font, TextAnchor align, Color color, Vector2 anchor)
        {
            GameObject go = new GameObject(name, typeof(Text));
            go.transform.SetParent(transform, false);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(700, size + 30);

            Text uiText = go.GetComponent<Text>();
            uiText.text = text;
            uiText.font = font;
            uiText.fontSize = size;
            uiText.alignment = align;
            uiText.color = color;
            uiText.fontStyle = FontStyle.Bold;

            // Add shadow effect via outline (simple approach)
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(0, 0, 0, 0.5f);
            outline.effectDistance = new Vector2(1, -1);

            go.AddComponent<CanvasGroup>();
            return go;
        }

        private GameObject CreatePlayButton(Font font)
        {
            GameObject buttonGo = new GameObject("PlayButton", typeof(Image), typeof(Button));
            buttonGo.transform.SetParent(transform, false);

            RectTransform rt = buttonGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.up;
            rt.anchorMax = Vector2.up;
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0, -10);
            rt.sizeDelta = new Vector2(260, 60);

            Image bg = buttonGo.GetComponent<Image>();
            bg.color = accentColor;
            bg.raycastTarget = true;

            // Add rounded corners feel with simple sprite (if available, otherwise solid)
            // Unity default sprite is a simple white quad which works fine

            GameObject label = new GameObject("Label", typeof(Text));
            label.transform.SetParent(buttonGo.transform, false);
            RectTransform labelRt = label.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.sizeDelta = Vector2.zero;

            Text labelText = label.GetComponent<Text>();
            labelText.text = "JOGAR";
            labelText.font = font;
            labelText.fontSize = 32;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.white;
            labelText.fontStyle = FontStyle.Bold;

            Button btn = buttonGo.GetComponent<Button>();
            btn.targetGraphic = bg;
            btn.onClick.AddListener(OnPlayClicked);

            // Add hover color transition
            ColorBlock colors = btn.colors;
            colors.highlightedColor = new Color(0.2f, 0.85f, 1f);
            colors.pressedColor = new Color(0f, 0.55f, 0.8f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            btn.colors = colors;

            buttonGo.AddComponent<CanvasGroup>();
            return buttonGo;
        }

        private void CreateDecorativeLine(Vector2 pos, float width, Color color, float height = 2f)
        {
            GameObject line = new GameObject("DecoLine", typeof(Image));
            line.transform.SetParent(transform, false);

            RectTransform rt = line.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.up;
            rt.anchorMax = Vector2.up;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(width, height);

            line.GetComponent<Image>().color = color;
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private IEnumerator TitleBounce()
        {
            if (titleText == null) yield break;

            RectTransform titleRt = titleText.GetComponent<RectTransform>();
            Vector2 basePos = titleRt.anchoredPosition;
            float elapsed = 0f;

            while (true)
            {
                elapsed += Time.unscaledDeltaTime;
                float bounce = Mathf.Sin(elapsed * 2f) * 3f;
                titleRt.anchoredPosition = basePos + new Vector2(0, bounce);
                yield return null;
            }
        }

        private void Update()
        {
            if (playButton == null) return;

            float pulse = Mathf.Sin(Time.unscaledTime * pulseSpeed) * 0.15f + 0.85f;
            Image btnImage = playButton.GetComponent<Image>();
            if (btnImage != null)
            {
                btnImage.color = Color.Lerp(accentColor, Color.white, pulse * 0.25f);
            }

            // Subtle scale pulse for play button
            RectTransform btnRt = playButton.GetComponent<RectTransform>();
            float scalePulse = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed * 0.7f) * 0.02f;
            btnRt.localScale = new Vector3(scalePulse, scalePulse, 1f);
        }

        private void OnPlayClicked()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            StartCoroutine(TransitionToGame());
        }

        private IEnumerator TransitionToGame()
        {
            float elapsed = 0f;
            float duration = 0.6f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("Track_01_Lago");
        }

        public void QuitGame()
        {
#if !UNITY_WEBGL
            Application.Quit();
#endif
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
