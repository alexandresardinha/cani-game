using UnityEngine;
using UnityEngine.UI;

namespace Canicross.UI
{
    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField] private Text timerText;
        [SerializeField] private string format = "mm\\:ss\\.ff";

        private float currentTime;

        public void SetTime(float timeInSeconds)
        {
            currentTime = timeInSeconds;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (timerText == null) return;

            System.TimeSpan time = System.TimeSpan.FromSeconds(currentTime);
            timerText.text = string.Format("{0:00}:{1:00}.{2:00}",
                time.Minutes, time.Seconds, time.Milliseconds / 10);
        }
    }
}
