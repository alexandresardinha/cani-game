using UnityEngine;
using Canicross.Core;

namespace Canicross.Player
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private DogController dogController;
        [SerializeField] private BondSystem bondSystem;

        private void Awake()
        {
            if (dogController == null) dogController = FindFirstObjectByType<DogController>();
            if (bondSystem == null) bondSystem = FindFirstObjectByType<BondSystem>();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != Core.GameState.Racing)
                return;

            float lateral = Input.GetAxis("Horizontal");
            float brake = Input.GetAxis("Vertical") < 0 ? Mathf.Abs(Input.GetAxis("Vertical")) : 0f;
            bool boost = Input.GetAxis("Vertical") > 0.1f;
            bool jump = Input.GetKeyDown(KeyCode.Space);

            dogController?.SetInput(lateral, brake, boost, jump);

            if (Input.GetKeyDown(KeyCode.E))
            {
                bondSystem?.RespondToCheckIn();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.CurrentState == Core.GameState.Racing)
                        GameManager.Instance.SetState(Core.GameState.Paused);
                    else if (GameManager.Instance.CurrentState == Core.GameState.Paused)
                        GameManager.Instance.SetState(Core.GameState.Racing);
                }
            }
        }
    }
}
