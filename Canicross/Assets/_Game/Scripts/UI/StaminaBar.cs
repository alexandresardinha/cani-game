using UnityEngine;
using UnityEngine.UI;

namespace Canicross.UI
{
    public class StaminaBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Color fullColor = new Color(0.12f, 0.75f, 1f);  // Dog: blue
        [SerializeField] private Color lowColor = new Color(1f, 0.3f, 0.3f);     // Low: red
        [SerializeField] private float lowThreshold = 0.25f;
        [SerializeField] private float smoothSpeed = 8f;

        private float targetFill;

        public void SetValue(float percent)
        {
            targetFill = Mathf.Clamp01(percent);
        }

        private void Update()
        {
            if (fillImage == null) return;

            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill,
                smoothSpeed * Time.deltaTime);

            fillImage.color = Color.Lerp(lowColor, fullColor,
                fillImage.fillAmount > lowThreshold ? 1f : fillImage.fillAmount / lowThreshold);
        }
    }
}
