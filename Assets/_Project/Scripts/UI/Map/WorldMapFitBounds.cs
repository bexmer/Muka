using UnityEngine;

namespace Project.UI
{
    // Auto-ajusta la WorldMapCam para encuadrar el nivel completo.
    public class WorldMapFitBounds : MonoBehaviour
    {
        public LayerMask includeLayers = ~0; // qu� capas incluir para calcular bounds
        public float padding = 5f;           // margen extra
        public float minSize = 20f;          // tama�o m�nimo por si la escena es peque�a
        public float fixedY = 100f;          // altura de la c�mara

        Camera _cam;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            if (_cam) _cam.orthographic = true;
        }

        void Start()
        {
            FitNow();
        }

        [ContextMenu("Fit Now")]
        public void FitNow()
        {
            var renderers = FindObjectsOfType<Renderer>();
            Bounds b = new Bounds(Vector3.zero, Vector3.zero);
            bool has = false;
            foreach (var r in renderers)
            {
                if (((1 << r.gameObject.layer) & includeLayers.value) == 0) continue;
                if (!has) { b = r.bounds; has = true; }
                else b.Encapsulate(r.bounds);
            }
            if (!has) return;

            // Posici�n al centro proyectado
            Vector3 center = b.center;
            transform.position = new Vector3(center.x, fixedY, center.z);
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            // Ajustar size ortogr�fico para cubrir extents X/Z
            float sizeX = b.extents.x + padding;
            float sizeZ = b.extents.z + padding;
            float target = Mathf.Max(sizeX, sizeZ, minSize);
            _cam.orthographicSize = target;
        }
    }
}
