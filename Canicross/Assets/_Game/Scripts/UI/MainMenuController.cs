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

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 1.5f;
        [SerializeField] private float pulseSpeed = 2f;

        private CanvasGroup canvasGroup;
        private GameObject titleText;
        private GameObject playButton;
        private GameObject subtitleText;
        private GameObject creditsText;

        private void Start()
        {
            BuildMenu();
            StartCoroutine(FadeIn());
        }

        private void BuildMenu()
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject title = CreateText("Title", "CANICROSS", new Vector2(0, 80),
                72, font, TextAnchor.LowerCenter, warmColor, Vector2.up);
            titleText = title;

            GameObject subtitle = CreateText("Subtitle", "Corra com seu cao pela trilha", new Vector2(0, 40),
                18, font, TextAnchor.MiddleCenter, Color.white, Vector2.up);
            subtitleText = subtitle;

            playButton = CreatePlayButton(font);

            GameObject credits = CreateText("Credits", "Feito com amor por humanos e caes",
                new Vector2(0, 30), 12, font, TextAnchor.LowerCenter,
                new Color(1f, 1f, 1f, 0.5f), Vector2.zero);
            creditsText = credits;

            GameObject dogLabel = CreateText("DogName", "ADAO  x  ALEXANDRE", new Vector2(0, 60),
                14, font, TextAnchor.MiddleCenter, accentColor, Vector2.up);

            CreateDecorativeLine(new Vector2(-120, 65), 240, accentColor);
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
            rt.sizeDelta = new Vector2(600, size + 20);

            Text uiText = go.GetComponent<Text>();
            uiText.text = text;
            uiText.font = font;
            uiText.fontSize = size;
            uiText.alignment = align;
            uiText.color = color;
            uiText.fontStyle = FontStyle.Bold;

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
            rt.sizeDelta = new Vector2(240, 55);

            Image bg = buttonGo.GetComponent<Image>();
            bg.color = accentColor;
            bg.raycastTarget = true;

            GameObject label = new GameObject("Label", typeof(Text));
            label.transform.SetParent(buttonGo.transform, false);
            RectTransform labelRt = label.GetComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.sizeDelta = Vector2.zero;

            Text labelText = label.GetComponent<Text>();
            labelText.text = "JOGAR";
            labelText.font = font;
            labelText.fontSize = 28;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.white;
            labelText.fontStyle = FontStyle.Bold;

            Button btn = buttonGo.GetComponent<Button>();
            btn.targetGraphic = bg;
            btn.onClick.AddListener(OnPlayClicked);

            buttonGo.AddComponent<CanvasGroup>();
            return buttonGo;
        }

        private void CreateDecorativeLine(Vector2 pos, float width, Color color)
        {
            GameObject line = new GameObject("DecoLine", typeof(Image));
            line.transform.SetParent(transform, false);

            RectTransform rt = line.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.up;
            rt.anchorMax = Vector2.up;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(width, 2);

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

        private void Update()
        {
            if (playButton == null) return;

            float pulse = Mathf.Sin(Time.unscaledTime * pulseSpeed) * 0.15f + 0.85f;
            Image btnImage = playButton.GetComponent<Image>();
            if (btnImage != null)
            {
                btnImage.color = Color.Lerp(accentColor, Color.white, pulse * 0.2f);
            }
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
            float duration = 0.8f;
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