using UnityEngine;
using System;

namespace Canicross.Systems
{
    public class StaminaSystem : MonoBehaviour
    {
        [Header("Dog Stamina")]
        [SerializeField] private float maxDogStamina = 100f;
        [SerializeField] private float dogDrainRate = 15f;
        [SerializeField] private float dogRegenRate = 8f;
        [SerializeField] private float dogRegenDelay = 1f;

        [Header("Runner Stamina")]
        [SerializeField] private float maxRunnerStamina = 100f;
        [SerializeField] private float runnerDrainRate = 6f;
        [SerializeField] private float runnerRegenRate = 2f;
        [SerializeField] private float runnerRegenDelay = 2f;

        public float DogStamina { get; private set; }
        public float RunnerStamina { get; private set; }
        public float DogStaminaPercent => DogStamina / maxDogStamina;
        public float RunnerStaminaPercent => RunnerStamina / maxRunnerStamina;

        public event Action<float, float> OnDogStaminaChanged;
        public event Action<float, float> OnRunnerStaminaChanged;

        private float dogRegenTimer;
        private float runnerRegenTimer;

        private void Start()
        {
            DogStamina = maxDogStamina;
            RunnerStamina = maxRunnerStamina;
        }

        public bool CanDogBoost()
        {
            return DogStamina > 0f;
        }

        public void ConsumeDogStamina(float deltaTime)
        {
            dogRegenTimer = 0f;
            DogStamina = Mathf.Max(DogStamina - dogDrainRate * deltaTime, 0f);
            OnDogStaminaChanged?.Invoke(DogStamina, maxDogStamina);
        }

        public void RegenerateDogStamina(float deltaTime)
        {
            dogRegenTimer += deltaTime;
            if (dogRegenTimer >= dogRegenDelay)
            {
                DogStamina = Mathf.Min(DogStamina + dogRegenRate * deltaTime, maxDogStamina);
                OnDogStaminaChanged?.Invoke(DogStamina, maxDogStamina);
            }
        }

        public void AddDogStamina(float amount)
        {
            DogStamina = Mathf.Min(DogStamina + amount, maxDogStamina);
            OnDogStaminaChanged?.Invoke(DogStamina, maxDogStamina);
        }

        public void ConsumeRunnerStamina(float deltaTime)
        {
            runnerRegenTimer = 0f;
            RunnerStamina = Mathf.Max(RunnerStamina - runnerDrainRate * deltaTime, 0f);
            OnRunnerStaminaChanged?.Invoke(RunnerStamina, maxRunnerStamina);
        }

        public void RegenerateRunnerStamina(float deltaTime)
        {
            runnerRegenTimer += deltaTime;
            if (runnerRegenTimer >= runnerRegenDelay)
            {
                RunnerStamina = Mathf.Min(RunnerStamina + runnerRegenRate * deltaTime, maxRunnerStamina);
                OnRunnerStaminaChanged?.Invoke(RunnerStamina, maxRunnerStamina);
            }
        }
    }
}
