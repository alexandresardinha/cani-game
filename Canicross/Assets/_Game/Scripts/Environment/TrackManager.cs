using UnityEngine;

namespace Canicross.Environment
{
    public class TrackManager : MonoBehaviour
    {
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 finishPosition;
        [SerializeField] private CheckpointTrigger[] checkpoints;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPosition, 1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(finishPosition, 1f);

            if (checkpoints != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var cp in checkpoints)
                {
                    if (cp != null)
                        Gizmos.DrawWireCube(cp.transform.position, Vector3.one * 0.5f);
                }
            }
        }

        public Vector3 GetStartPosition() => startPosition;
        public Vector3 GetFinishPosition() => finishPosition;
    }
}
