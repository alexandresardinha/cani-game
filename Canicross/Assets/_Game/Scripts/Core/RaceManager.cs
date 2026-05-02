using UnityEngine;
using Canicross.Environment;

namespace Canicross.Core
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private CheckpointTrigger[] checkpoints;

        public float FinalTime { get; private set; }
        public float BestLap { get; private set; }
        public int CurrentCheckpoint { get; private set; }

        private float raceStartTime;
        private bool isRacing;

        private void Awake()
        {
            BestLap = PlayerPrefs.GetFloat("BestLap", float.MaxValue);
        }

        public void StartRace()
        {
            raceStartTime = Time.time;
            isRacing = true;
            CurrentCheckpoint = 0;
        }

        public void StopRace()
        {
            if (!isRacing) return;
            isRacing = false;
            FinalTime = Time.time - raceStartTime;

            if (FinalTime < BestLap)
            {
                BestLap = FinalTime;
                PlayerPrefs.SetFloat("BestLap", BestLap);
                PlayerPrefs.Save();
            }
        }

        public float GetElapsedTime()
        {
            if (!isRacing) return FinalTime;
            return Time.time - raceStartTime;
        }

        public void OnCheckpointReached(int index)
        {
            if (index == CurrentCheckpoint + 1)
            {
                CurrentCheckpoint = index;
            }
        }

        public bool AllCheckpointsReached()
        {
            if (checkpoints == null || checkpoints.Length == 0) return false;
            return CurrentCheckpoint >= checkpoints.Length;
        }
    }
}
