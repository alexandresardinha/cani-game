using UnityEngine;
using Canicross.Player;
using Canicross.Cam;

namespace Canicross.Environment
{
    public class Obstacle : MonoBehaviour
    {
        public enum ObstacleType
        {
            Root,
            Mud,
            Water,
            Rock,
            LowBranch
        }

        [SerializeField] private ObstacleType type;
        [SerializeField] private float speedMultiplier = 0.7f;
        [SerializeField] private float penaltyDuration = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Dog")) return;

            DogController dog = other.GetComponent<DogController>();
            if (dog == null) return;

            switch (type)
            {
                case ObstacleType.Root:
                case ObstacleType.Rock:
                    dog.ApplyObstaclePenalty(speedMultiplier, penaltyDuration);
                    FindFirstObjectByType<POVCamera>()?.TriggerLandingShake();
                    break;

                case ObstacleType.Mud:
                    dog.ApplyObstaclePenalty(speedMultiplier, penaltyDuration);
                    break;

                case ObstacleType.Water:
                    ResetToLastCheckpoint();
                    break;

                case ObstacleType.LowBranch:
                    dog.ApplyObstaclePenalty(1f, 0.3f);
                    break;
            }
        }

        private void ResetToLastCheckpoint()
        {
            TrackManager track = FindFirstObjectByType<TrackManager>();
            if (track != null)
            {
                RunnerController runner = FindFirstObjectByType<RunnerController>();
                runner?.ResetPosition(track.GetStartPosition());
            }
        }
    }
}
