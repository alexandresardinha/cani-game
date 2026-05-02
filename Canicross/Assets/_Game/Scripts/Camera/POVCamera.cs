using UnityEngine;
using Canicross.Player;

namespace Canicross.Cam
{
    public class POVCamera : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Transform lookAtTarget;

        [Header("Positioning")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 1.6f, -3f);
        [SerializeField] private float positionSmoothTime = 0.1f;

        [Header("FOV")]
        [SerializeField] private float baseFOV = 90f;
        [SerializeField] private float boostFOV = 100f;
        [SerializeField] private float fovChangeSpeed = 5f;

        [Header("Sway")]
        [SerializeField] private float swayAmplitude = 0.3f;
        [SerializeField] private float swayFrequency = 2f;
        [SerializeField] private float swaySmoothTime = 0.05f;

        [Header("Tilt")]
        [SerializeField] private float maxTiltAngle = 5f;
        [SerializeField] private float tiltSmoothTime = 0.1f;

        [Header("Shake")]
        [SerializeField] private float landingShakeAmount = 0.1f;
        [SerializeField] private float landingShakeDuration = 0.2f;

        [Header("References")]
        [SerializeField] private DogController dogController;

        private UnityEngine.Camera cam;
        private Vector3 currentVelocity;
        private float currentSwayOffset;
        private float swayVelocity;
        private float currentTilt;
        private float tiltVelocity;
        private float shakeTimer;
        private float shakeAmount;

        private void Start()
        {
            cam = GetComponent<UnityEngine.Camera>();
            if (cam == null) cam = gameObject.AddComponent<UnityEngine.Camera>();

            if (dogController == null) dogController = FindFirstObjectByType<DogController>();
            if (target == null && dogController != null) target = dogController.transform;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            UpdateFOV();
            UpdatePosition();
            UpdateSway();
            UpdateTilt();
            UpdateShake();
        }

        private void UpdateFOV()
        {
            if (cam == null) return;

            float targetFOV = baseFOV;
            if (dogController != null && dogController.IsBoosting)
            {
                targetFOV = boostFOV;
            }

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        }

        private void UpdatePosition()
        {
            Vector3 targetPos = target.TransformPoint(offset);
            Vector3 shakeOffset = Vector3.zero;

            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                shakeOffset = Random.insideUnitSphere * shakeAmount;
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos + shakeOffset,
                ref currentVelocity,
                positionSmoothTime
            );

            Vector3 lookTarget = lookAtTarget != null ? lookAtTarget.position :
                target.position + target.forward * 2f + Vector3.up * 0.5f;

            transform.LookAt(lookTarget);
        }

        private void UpdateSway()
        {
            if (dogController == null) return;

            float speed = dogController.SpeedKmh;

            float targetSway = Mathf.Sin(Time.time * swayFrequency) * swayAmplitude *
                Mathf.Clamp01(speed / 15f);

            currentSwayOffset = Mathf.SmoothDamp(currentSwayOffset, targetSway,
                ref swayVelocity, swaySmoothTime);

            transform.RotateAround(target.position, Vector3.up, currentSwayOffset);
        }

        private void UpdateTilt()
        {
            if (dogController == null) return;

            float lateralInput = Input.GetAxis("Horizontal");
            float targetTilt = -lateralInput * maxTiltAngle;

            currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt,
                ref tiltVelocity, tiltSmoothTime);

            Vector3 currentEuler = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(currentEuler.x, currentEuler.y, currentTilt);
        }

        private void UpdateShake()
        {
        }

        public void TriggerLandingShake()
        {
            shakeTimer = landingShakeDuration;
            shakeAmount = landingShakeAmount;
        }
    }
}
