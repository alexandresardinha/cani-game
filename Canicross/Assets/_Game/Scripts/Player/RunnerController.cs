using UnityEngine;
using Canicross.Systems;
using Canicross.Player;

namespace Canicross.Player
{
    public class RunnerController : MonoBehaviour
    {
        [Header("Tether")]
        [SerializeField] private Transform dogTransform;
        [SerializeField] private float tetherRestLength = 1.5f;
        [SerializeField] private float tetherMaxDistance = 3f;
        [SerializeField] private float tetherSpring = 60f;
        [SerializeField] private float tetherDamper = 8f;

        [Header("Movement")]
        [SerializeField] private float baseFollowSpeed = 10f;
        [SerializeField] private float lateralFollowDamping = 3f;
        [SerializeField] private float gravityForce = -20f;

        [Header("References")]
        [SerializeField] private StaminaSystem staminaSystem;
        [SerializeField] private CharacterController charController;
        [SerializeField] private TetherSystem tetherSystem;

        public bool IsMoving { get; private set; }
        public float CurrentSpeedKmh { get; private set; }

        private Vector3 previousPosition;

        private void Start()
        {
            if (charController == null) charController = GetComponent<CharacterController>();
            if (staminaSystem == null) staminaSystem = FindFirstObjectByType<StaminaSystem>();
            if (tetherSystem == null) tetherSystem = FindFirstObjectByType<TetherSystem>();
            previousPosition = transform.position;
        }

        private void Update()
        {
            if (dogTransform == null) return;

            Vector3 moveDirection = CalculateMovement();

            IsMoving = moveDirection.magnitude > 0.5f;
            UpdateSpeedDisplay();
            UpdateStamina();

            if (charController != null)
            {
                moveDirection.y += gravityForce * Time.deltaTime;
                charController.Move(moveDirection * Time.deltaTime);
            }
            else
            {
                moveDirection.y = 0f;
                transform.position += moveDirection * Time.deltaTime;
            }
        }

        private Vector3 CalculateMovement()
        {
            Vector3 toDog = dogTransform.position - transform.position;
            float distance = toDog.magnitude;
            Vector3 dirToDog = toDog.normalized;

            Vector3 moveDirection = Vector3.zero;

            float forwardComponent = Vector3.Dot(toDog, transform.forward);

            if (distance > tetherRestLength)
            {
                float pullForce = Mathf.Clamp((distance - tetherRestLength) * tetherSpring * 0.02f, 0f, 25f);

                moveDirection += dirToDog * (baseFollowSpeed + pullForce);

                if (distance > tetherMaxDistance)
                {
                    moveDirection += dirToDog * ((distance - tetherMaxDistance) * tetherSpring * 0.1f);
                }

                float lateralOffset = Vector3.Dot(toDog, transform.right);
                moveDirection += transform.right * (lateralOffset * lateralFollowDamping);
            }
            else
            {
                moveDirection = dirToDog * baseFollowSpeed * 0.7f;
            }

            return moveDirection;
        }

        private void UpdateSpeedDisplay()
        {
            float distanceMoved = Vector3.Distance(transform.position, previousPosition);
            CurrentSpeedKmh = (distanceMoved / Time.deltaTime) * 3.6f;
            previousPosition = transform.position;
        }

        private void UpdateStamina()
        {
            if (staminaSystem == null) return;

            if (IsMoving)
            {
                staminaSystem.ConsumeRunnerStamina(Time.deltaTime);
            }
            else
            {
                staminaSystem.RegenerateRunnerStamina(Time.deltaTime);
            }
        }

        public void ResetPosition(Vector3 position)
        {
            if (charController != null)
            {
                charController.enabled = false;
                transform.position = position;
                charController.enabled = true;
            }
            else
            {
                transform.position = position;
            }
        }
    }
}