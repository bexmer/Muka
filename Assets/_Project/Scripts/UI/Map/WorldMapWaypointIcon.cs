using UnityEngine;

namespace Project.UI
{
    // Colócalo en el icono del waypoint dentro del RawImage del mapa grande.
    public class WorldMapWaypointIcon : MonoBehaviour
    {
        public Camera worldMapCam;           // WorldMapCam (ortho)
        public RectTransform mapImageRect;   // RectTransform del RawImage del mapa
        public WaypointSystem system;

        RectTransform _rt;

        void Awake()
        {
            _rt = GetComponent<RectTransform>();
            if (!mapImageRect) mapImageRect = _rt.parent as RectTransform;
        }

        void LateUpdate()
        {
            if (!worldMapCam || !mapImageRect) return;
            if (!system) system = WaypointSystem.I;
            Transform target = system ? system.Active : null;

            if (!target)
            {
                _rt.gameObject.SetActive(false);
                return;
            }

            Vector3 vp = worldMapCam.WorldToViewportPoint(target.position);
            Vector2 size = mapImageRect.rect.size;
            Vector2 anchored = new Vector2((vp.x - 0.5f) * size.x, (vp.y - 0.5f) * size.y);

            _rt.anchoredPosition = anchored;
            _rt.gameObject.SetActive(vp.z >= 0); // oculta si está "detrás" (raro en ortho)
        }
    }
}

