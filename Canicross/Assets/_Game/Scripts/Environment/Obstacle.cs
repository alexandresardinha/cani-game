using UnityEngine;
using Canicross.Player;
using Canicross.Cam;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        public ObstacleType Type => type;

        public void SetType(ObstacleType newType)
        {
            type = newType;
            switch (newType)
            {
                case ObstacleType.Root:
                case ObstacleType.Rock:
                    speedMultiplier = 0.5f;
                    penaltyDuration = 0.5f;
                    break;
                case ObstacleType.Mud:
                    speedMultiplier = 0.7f;
                    penaltyDuration = 2f;
                    break;
                case ObstacleType.Water:
                    speedMultiplier = 0f;
                    penaltyDuration = 0f;
                    break;
                case ObstacleType.LowBranch:
                    speedMultiplier = 1f;
                    penaltyDuration = 0.3f;
                    break;
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

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
