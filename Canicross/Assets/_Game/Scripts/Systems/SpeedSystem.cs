using UnityEngine;
using Canicross.Player;

namespace Canicross.Systems
{
    public class SpeedSystem : MonoBehaviour
    {
        [SerializeField] private DogController dogController;

        public float CurrentSpeedKmh { get; private set; }
        public float SmoothedSpeedKmh { get; private set; }
        public float MaxSpeedKmh { get; private set; }

        private void Start()
        {
            if (dogController == null) dogController = FindFirstObjectByType<DogController>();
        }

        private void Update()
        {
            if (dogController == null) return;

            CurrentSpeedKmh = dogController.SpeedKmh;
            SmoothedSpeedKmh = Mathf.Lerp(SmoothedSpeedKmh, CurrentSpeedKmh, 8f * Time.deltaTime);

            if (CurrentSpeedKmh > MaxSpeedKmh)
            {
                MaxSpeedKmh = CurrentSpeedKmh;
            }
        }
    }
}
