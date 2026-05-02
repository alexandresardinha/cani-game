using UnityEngine;
using Canicross.Systems;

namespace Canicross.Player
{
    public class RunnerController : MonoBehaviour
    {
        [Header("Tether")]
        [SerializeField] private Transform dogTransform;
        [SerializeField] private float tetherLength = 2.5f;
        [SerializeField] private float tetherSpring = 50f;
        [SerializeField] private float tetherDamper = 10f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 12f;
        [SerializeField] private float lateralSpeed = 5f;

        [Header("References")]
        [SerializeField] private StaminaSystem staminaSystem;
        [SerializeField] private CharacterController charController;

        public bool IsMoving { get; private set; }

        private void Start()
        {
            if (charController == null) charController = GetComponent<CharacterController>();
            if (staminaSystem == null) staminaSystem = FindFirstObjectByType<StaminaSystem>();
        }

        private void Update()
        {
            if (dogTransform == null) return;

            Vector3 tetherVector = transform.position - dogTransform.position;
            float distance = tetherVector.magnitude;

            Vector3 moveDirection = Vector3.zero;

            if (distance > tetherLength)
            {
                Vector3 pullDirection = (dogTransform.position - transform.position).normalized;
                float pullForce = Mathf.Clamp((distance - tetherLength) * tetherSpring, 0f, 20f) * Time.deltaTime;
                pullForce -= tetherDamper * Time.deltaTime;

                moveDirection = pullDirection * (moveSpeed + pullForce);
            }
            else
            {
                Vector3 forward = dogTransform.forward;
                moveDirection = forward * moveSpeed * 0.8f;
            }

            IsMoving = moveDirection.magnitude > 0.1f;

            if (staminaSystem != null)
            {
                if (IsMoving)
                {
                    staminaSystem.ConsumeRunnerStamina(Time.deltaTime);
                }
                else
                {
                    staminaSystem.RegenerateRunnerStamina(Time.deltaTime);
                }
            }

            if (charController != null)
            {
                moveDirection.y = Physics.gravity.y * Time.deltaTime;
                charController.Move(moveDirection * Time.deltaTime);
            }
            else
            {
                moveDirection.y = 0f;
                transform.position += moveDirection * Time.deltaTime;
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
