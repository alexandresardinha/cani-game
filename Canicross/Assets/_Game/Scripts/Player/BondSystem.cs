using UnityEngine;
using System.Collections;
using Canicross.Systems;

namespace Canicross.Player
{
    public class BondSystem : MonoBehaviour
    {
        [Header("Bond Settings")]
        [SerializeField] private float checkInInterval = 30f;
        [SerializeField] private float checkInWindow = 2f;
        [SerializeField] private float bondBonusStamina = 20f;
        [SerializeField] private int bondOnCheckIn = 10;
        [SerializeField] private int maxBondLevel = 100;

        [Header("References")]
        [SerializeField] private StaminaSystem staminaSystem;
        [SerializeField] private DogController dogController;

        public int BondLevel { get; private set; } = 50;
        public bool IsCheckingIn { get; private set; }

        private float nextCheckInTime;
        private float checkInStartTime;

        public event System.Action<int> OnBondChanged;
        public event System.Action OnDogCheckIn;

        private void Start()
        {
            nextCheckInTime = Random.Range(checkInInterval * 0.5f, checkInInterval);
            if (staminaSystem == null) staminaSystem = FindFirstObjectByType<StaminaSystem>();
            if (dogController == null) dogController = FindFirstObjectByType<DogController>();
        }

        private void Update()
        {
            if (Time.time >= nextCheckInTime && !IsCheckingIn)
            {
                StartCheckIn();
                nextCheckInTime = Time.time + checkInInterval + Random.Range(-5f, 5f);
            }
        }

        private void StartCheckIn()
        {
            IsCheckingIn = true;
            checkInStartTime = Time.time;
            OnDogCheckIn?.Invoke();
            StartCoroutine(CheckInWindowRoutine());
        }

        private IEnumerator CheckInWindowRoutine()
        {
            yield return new WaitForSeconds(checkInWindow);
            IsCheckingIn = false;
        }

        public void RespondToCheckIn()
        {
            if (!IsCheckingIn) return;

            IsCheckingIn = false;
            StopAllCoroutines();

            BondLevel = Mathf.Min(BondLevel + bondOnCheckIn, maxBondLevel);

            if (staminaSystem != null)
            {
                staminaSystem.AddDogStamina(bondBonusStamina);
            }

            OnBondChanged?.Invoke(BondLevel);
        }

        public void MissCheckIn()
        {
            BondLevel = Mathf.Max(BondLevel - 5, 0);
            OnBondChanged?.Invoke(BondLevel);
        }

        public float GetResponsivenessMultiplier()
        {
            return 0.7f + (BondLevel / 100f) * 0.3f;
        }
    }
}
