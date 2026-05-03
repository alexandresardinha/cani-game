using UnityEngine;
using System.Collections.Generic;

namespace Canicross.Player
{
    public class TetherSystem : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private Transform dogPoint;
        [SerializeField] private Transform runnerPoint;
        [SerializeField] private Color ropeColor = new Color(0f, 0.75f, 1f, 1f);
        [SerializeField] private float ropeWidth = 0.04f;
        [SerializeField] private Material ropeMaterial;

        [Header("Elastic Physics")]
        [SerializeField] private float restLength = 1.2f;
        [SerializeField] private float maxDistance = 3f;
        [SerializeField] private float springStiffness = 80f;
        [SerializeField] private float damper = 12f;
        [SerializeField] private float bungeeStretchRatio = 0.6f;

        [Header("Visual Curve")]
        [SerializeField] private int segmentCount = 12;
        [SerializeField] private float slackAmount = 0.15f;

        private LineRenderer lineRenderer;
        private List<Vector3> segmentPoints = new List<Vector3>();
        private Rigidbody dogRb;
        private Rigidbody runnerRb;

        public float CurrentTension { get; private set; }
        public float CurrentDistance { get; private set; }
        public bool IsTaut { get; private set; }

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            ConfigureLineRenderer();

            if (dogPoint != null)
            {
                dogRb = dogPoint.GetComponentInParent<Rigidbody>();
            }
        }

        private void ConfigureLineRenderer()
        {
            lineRenderer.positionCount = segmentCount;
            lineRenderer.startWidth = ropeWidth;
            lineRenderer.endWidth = ropeWidth * 0.7f;
            lineRenderer.startColor = ropeColor;
            lineRenderer.endColor = ropeColor;
            lineRenderer.useWorldSpace = true;
            lineRenderer.textureMode = LineTextureMode.Tile;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;

            if (ropeMaterial != null)
            {
                lineRenderer.material = ropeMaterial;
            }
            else
            {
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = ropeColor;
                lineRenderer.material = mat;
            }
        }

        private void LateUpdate()
        {
            if (dogPoint == null || runnerPoint == null) return;

            CurrentDistance = Vector3.Distance(dogPoint.position, runnerPoint.position);

            UpdateElasticPhysics();
            UpdateVisualCurve();
        }

        private void UpdateElasticPhysics()
        {
            if (dogRb == null) return;

            Vector3 dir = (runnerPoint.position - dogPoint.position).normalized;
            float stretch = CurrentDistance - restLength;

            if (stretch > 0)
            {
                float tension = springStiffness * stretch;

                Vector3 dogVelDir = dogRb.linearVelocity.normalized;
                float dampingForce = -damper * Vector3.Dot(dogRb.linearVelocity, dir);
                tension += dampingForce;

                tension = Mathf.Max(tension, 0);

                if (CurrentDistance > maxDistance)
                {
                    IsTaut = true;
                    CurrentTension = tension + springStiffness * (CurrentDistance - maxDistance) * 3f;
                }
                else
                {
                    IsTaut = CurrentDistance > maxDistance * bungeeStretchRatio;
                    CurrentTension = tension;
                }
            }
            else
            {
                CurrentTension = 0f;
                IsTaut = false;
            }
        }

        private void UpdateVisualCurve()
        {
            Vector3 start = dogPoint.position;
            Vector3 end = runnerPoint.position;

            float slack = CalculateSlack();

            segmentPoints.Clear();
            for (int i = 0; i <= segmentCount; i++)
            {
                float t = (float)i / segmentCount;
                Vector3 point = CalculateBezierPoint(start, end, t, slack);
                segmentPoints.Add(point);
            }

            lineRenderer.positionCount = segmentPoints.Count;
            lineRenderer.SetPositions(segmentPoints.ToArray());

            float stretchRatio = Mathf.Clamp01(CurrentDistance / maxDistance);
            Color currentColor = Color.Lerp(ropeColor, Color.white, stretchRatio * 0.5f);
            lineRenderer.startColor = currentColor;
            lineRenderer.endColor = currentColor;

            float widthScale = Mathf.Lerp(1.2f, 0.8f, stretchRatio);
            lineRenderer.startWidth = ropeWidth * widthScale;
            lineRenderer.endWidth = ropeWidth * widthScale * 0.7f;
        }

        private float CalculateSlack()
        {
            float distanceRatio = Mathf.Clamp01(CurrentDistance / maxDistance);
            float slack = Mathf.Lerp(slackAmount, 0.02f, distanceRatio);
            return slack;
        }

        private Vector3 CalculateBezierPoint(Vector3 start, Vector3 end, float t, float slack)
        {
            Vector3 mid = (start + end) * 0.5f;
            Vector3 down = Vector3.down * slack * 3f;
            Vector3 forward = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 sideOffset = right * Mathf.Sin(t * Mathf.PI) * 0.02f;

            Vector3 controlPoint = mid + down;

            float oneMinusT = 1f - t;
            Vector3 point = oneMinusT * oneMinusT * start + 2f * oneMinusT * t * controlPoint + t * t * end;
            point += sideOffset;

            return point;
        }

        public float GetSpringForce()
        {
            return CurrentTension;
        }

        public Vector3 GetPullDirection()
        {
            if (dogPoint == null || runnerPoint == null) return Vector3.zero;
            return (runnerPoint.position - dogPoint.position).normalized;
        }

        public void SetPoints(Transform dog, Transform runner)
        {
            dogPoint = dog;
            runnerPoint = runner;
            if (dog != null)
            {
                dogRb = dog.GetComponentInParent<Rigidbody>();
            }
        }
    }
}