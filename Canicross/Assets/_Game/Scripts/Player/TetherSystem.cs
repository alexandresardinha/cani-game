using UnityEngine;

namespace Canicross.Player
{
    public class TetherSystem : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private Transform dogPoint;
        [SerializeField] private Transform runnerPoint;
        [SerializeField] private Color ropeColor = new Color(0f, 0.75f, 1f, 1f); // #00BFFF
        [SerializeField] private float ropeWidth = 0.03f;
        [SerializeField] private Material ropeMaterial;

        [Header("Physics")]
        [SerializeField] private float maxDistance = 3f;
        [SerializeField] private float visualSlack = 0.05f;

        private LineRenderer lineRenderer;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            ConfigureLineRenderer();
        }

        private void ConfigureLineRenderer()
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = ropeWidth;
            lineRenderer.endWidth = ropeWidth;
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
        }

        private void LateUpdate()
        {
            if (dogPoint == null || runnerPoint == null) return;

            Vector3 dogPos = dogPoint.position;
            Vector3 runnerPos = runnerPoint.position;

            float distance = Vector3.Distance(dogPos, runnerPos);

            if (distance > maxDistance)
            {
                Vector3 direction = (dogPos - runnerPos).normalized;
                runnerPos = dogPos - direction * (maxDistance - 0.1f);
            }

            lineRenderer.SetPosition(0, dogPos);
            lineRenderer.SetPosition(1, runnerPos);

            float stretch = Mathf.Clamp01(distance / maxDistance);
            Color currentColor = Color.Lerp(ropeColor, Color.white, stretch);
            lineRenderer.startColor = currentColor;
            lineRenderer.endColor = currentColor;
        }

        public void SetPoints(Transform dog, Transform runner)
        {
            dogPoint = dog;
            runnerPoint = runner;
        }
    }
}
