using UnityEngine;
using UnityEngine.UI;
using System;
using Canicross.Systems;
using Canicross.Player;
using Canicross.Core;

namespace Canicross.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Stamina Bars")]
        [SerializeField] private StaminaBar dogStaminaBar;
        [SerializeField] private StaminaBar runnerStaminaBar;

        [Header("Timer")]
        [SerializeField] private TimerDisplay timerDisplay;

        [Header("Speed")]
        [SerializeField] private Text speedText;

        [Header("Bond")]
        [SerializeField] private Text bondText;
        [SerializeField] private Image bondIcon;

        [Header("Dog Name")]
        [SerializeField] private Text dogNameText;

        [Header("Check-in Prompt")]
        [SerializeField] private GameObject checkInPrompt;
        [SerializeField] private float promptFadeTime = 0.3f;

        [Header("Boost Indicator")]
        [SerializeField] private GameObject boostIndicator;

        [Header("References")]
        [SerializeField] private StaminaSystem staminaSystem;
        [SerializeField] private SpeedSystem speedSystem;
        [SerializeField] private BondSystem bondSystem;
        [SerializeField] private RaceManager raceManager;
        [SerializeField] private DogController dogController;

        private void Start()
        {
            FindReferences();
            SubscribeEvents();
        }

        private void FindReferences()
        {
            if (staminaSystem == null) staminaSystem = FindFirstObjectByType<StaminaSystem>();
            if (speedSystem == null) speedSystem = FindFirstObjectByType<SpeedSystem>();
            if (bondSystem == null) bondSystem = FindFirstObjectByType<BondSystem>();
            if (raceManager == null) raceManager = FindFirstObjectByType<RaceManager>();
            if (dogController == null) dogController = FindFirstObjectByType<DogController>();
        }

        private void SubscribeEvents()
        {
            if (staminaSystem != null)
            {
                staminaSystem.OnDogStaminaChanged += UpdateDogStamina;
                staminaSystem.OnRunnerStaminaChanged += UpdateRunnerStamina;
            }
            if (bondSystem != null)
            {
                bondSystem.OnBondChanged += UpdateBond;
                bondSystem.OnDogCheckIn += ShowCheckInPrompt;
            }
        }

        private void Update()
        {
            UpdateTimer();
            UpdateSpeed();
            UpdateBoostIndicator();
        }

        private void UpdateTimer()
        {
            if (timerDisplay != null && raceManager != null)
            {
                timerDisplay.SetTime(raceManager.GetElapsedTime());
            }
        }

        private void UpdateSpeed()
        {
            if (speedText != null && speedSystem != null)
            {
                speedText.text = $"▼ {speedSystem.SmoothedSpeedKmh:F0} km/h";
            }
        }

        private void UpdateDogStamina(float current, float max)
        {
            dogStaminaBar?.SetValue(current / max);
        }

        private void UpdateRunnerStamina(float current, float max)
        {
            runnerStaminaBar?.SetValue(current / max);
        }

        private void UpdateBond(int level)
        {
            if (bondText != null)
            {
                bondText.text = $"🔗 {level}";
            }
        }

        private void UpdateBoostIndicator()
        {
            if (boostIndicator != null)
            {
                boostIndicator.SetActive(dogController != null && dogController.IsBoosting);
            }
        }

        private void ShowCheckInPrompt()
        {
            if (checkInPrompt == null) return;
            StopAllCoroutines();
            StartCoroutine(FlashPrompt());
        }

        private System.Collections.IEnumerator FlashPrompt()
        {
            checkInPrompt.SetActive(true);
            float elapsed = 0f;
            CanvasGroup cg = checkInPrompt.GetComponent<CanvasGroup>();
            if (cg == null) cg = checkInPrompt.AddComponent<CanvasGroup>();

            while (elapsed < promptFadeTime)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / promptFadeTime);
                yield return null;
            }

            yield return new WaitForSeconds(1.5f);

            elapsed = 0f;
            while (elapsed < promptFadeTime)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, elapsed / promptFadeTime);
                yield return null;
            }

            checkInPrompt.SetActive(false);
        }

        public void ShowCountdown(Action onComplete)
        {
            StartCoroutine(CountdownRoutine(onComplete));
        }

        private System.Collections.IEnumerator CountdownRoutine(Action onComplete)
        {
            yield return new WaitForSeconds(0.5f);
            onComplete?.Invoke();
        }

        public void ShowResults(float finalTime, float bestLap, int bondLevel)
        {
            if (timerDisplay != null)
            {
                timerDisplay.gameObject.SetActive(false);
            }
        }
    }
}
