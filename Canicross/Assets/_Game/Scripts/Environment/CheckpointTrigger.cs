using UnityEngine;
using Canicross.Core;

namespace Canicross.Environment
{
    public class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private int checkpointIndex;
        [SerializeField] private bool isFinishLine;

        public int Index => checkpointIndex;
        public bool IsFinishLine => isFinishLine;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Dog") && !other.CompareTag("Runner")) return;

            RaceManager raceManager = FindFirstObjectByType<RaceManager>();
            if (raceManager != null)
            {
                raceManager.OnCheckpointReached(checkpointIndex);

                if (isFinishLine && raceManager.AllCheckpointsReached())
                {
                    GameManager.Instance?.FinishRace();
                }
            }
        }
    }
}
