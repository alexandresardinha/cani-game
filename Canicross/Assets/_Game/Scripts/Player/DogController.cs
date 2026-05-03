using UnityEngine;
using Canicross.Systems;

namespace Canicross.Player
{
    public class DogController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float baseSpeed = 14f;
        [SerializeField] private float boostSpeed = 22f;
        [SerializeField] private float turnSpeed = 3f;
        [SerializeField] private float acceleration = 8f;
        [SerializeField] private float deceleration = 10f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 6f;
        [SerializeField] private float jumpCooldown = 0.5f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Steering")]
        [SerializeField] private float steerSmoothness = 5f;
        [SerializeField] private float maxLateralOffset = 1.5f;

        [Header("Tether Pull")]
        [SerializeField] private float tetherPullStrength = 5f;

        [Header("References")]
        [SerializeField] private StaminaSystem staminaSystem;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private TetherSystem tetherSystem;

        public bool IsGrounded { get; private set; } = true;
        public bool IsBoosting { get; private set; }
        public float CurrentSpeed { get; private set; }
        public Vector3 Velocity => rb != null ? rb.linearVelocity : Vector3.zero;
        public float SpeedKmh => CurrentSpeed * 3.6f;

        private float targetLateralPos;
        private float currentLateralPos;
        private float lastJumpTime;
        private float lateralInput;
        private float forwardBrakeInput;
        private bool boostInput;
        private bool jumpInput;
        private Vector3 startingForward;

        private void Start()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (staminaSystem == null) staminaSystem = FindFirstObjectByType<StaminaSystem>();
            if (tetherSystem == null) tetherSystem = FindFirstObjectByType<TetherSystem>();
            startingForward = transform.forward;
            CurrentSpeed = baseSpeed;
        }

        public void SetInput(float lateral, float brake, bool boost, bool jump)
        {
            lateralInput = lateral;
            forwardBrakeInput = brake;
            boostInput = boost;
            jumpInput = jump;
        }

        public void ApplyObstaclePenalty(float speedMultiplier, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(SpeedPenaltyRoutine(speedMultiplier, duration));
        }

        private System.Collections.IEnumerator SpeedPenaltyRoutine(float multiplier, float duration)
        {
            float originalBase = baseSpeed;
            CurrentSpeed *= multiplier;
            yield return new WaitForSeconds(duration);
            CurrentSpeed = originalBase;
        }

        private void Update()
        {
            HandleJump();
            UpdateTargetSpeed();
        }

        private void FixedUpdate()
        {
            UpdateSteering();
            ApplyMovement();
            ApplyTetherForce();
        }

        private void ApplyTetherForce()
        {
            if (rb == null || tetherSystem == null) return;

            if (tetherSystem.IsTaut && tetherSystem.CurrentTension > 0)
            {
                Vector3 pullDir = tetherSystem.GetPullDirection();
                float pullForce = tetherSystem.GetSpringForce() * tetherPullStrength;
                rb.AddForce(pullDir * pullForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }

        private void UpdateTargetSpeed()
        {
            float targetSpeed = baseSpeed;

            if (boostInput && staminaSystem != null && staminaSystem.CanDogBoost())
            {
                targetSpeed = boostSpeed;
                IsBoosting = true;
                staminaSystem.ConsumeDogStamina(Time.deltaTime);
            }
            else
            {
                IsBoosting = false;
                staminaSystem.RegenerateDogStamina(Time.deltaTime);
            }

            if (forwardBrakeInput > 0.1f)
            {
                targetSpeed = Mathf.Lerp(targetSpeed, baseSpeed * 0.4f, forwardBrakeInput);
            }

            CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed,
                (targetSpeed > CurrentSpeed ? acceleration : deceleration) * Time.deltaTime);
        }

        private void UpdateSteering()
        {
            targetLateralPos += lateralInput * turnSpeed * Time.deltaTime;
            targetLateralPos = Mathf.Clamp(targetLateralPos, -maxLateralOffset, maxLateralOffset);

            currentLateralPos = Mathf.Lerp(currentLateralPos, targetLateralPos, steerSmoothness * Time.deltaTime);
        }

        private void ApplyMovement()
        {
            if (rb == null) return;

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            Vector3 targetVelocity = forward * CurrentSpeed + right * 
                (currentLateralPos - transform.localPosition.x) * CurrentSpeed * 0.5f;

            Vector3 currentVel = rb.linearVelocity;
            Vector3 newVel = new Vector3(
                Mathf.Lerp(currentVel.x, targetVelocity.x, 10f * Time.fixedDeltaTime),
                currentVel.y,
                Mathf.Lerp(currentVel.z, targetVelocity.z, 10f * Time.fixedDeltaTime)
            );

            rb.linearVelocity = newVel;
        }

        private void HandleJump()
        {
            if (!jumpInput || Time.time < lastJumpTime + jumpCooldown) return;
            if (!IsGrounded) return;

            lastJumpTime = Time.time;
            IsGrounded = false;

            if (rb != null)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                IsGrounded = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            {
                IsGrounded = false;
            }
        }
    }
}
