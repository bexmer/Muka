using UnityEngine;

namespace Project.UI
{
    // Coloca este script en el icono del jugador dentro del WorldMapImage.
    // Posiciona el icono seg�n la proyecci�n de WorldMapCam.
    public class MapIconSync : MonoBehaviour
    {
        public Transform player;
        public Camera worldMapCam;          // la WorldMapCam (ortogr�fica)
        public RectTransform mapImageRect;  // RectTransform del RawImage que muestra RT_WorldMap

        RectTransform _icon;

        void Awake()
        {
            _icon = GetComponent<RectTransform>();
            if (!mapImageRect) mapImageRect = _icon.parent as RectTransform;
        }

        void LateUpdate()
        {
            if (!player || !worldMapCam || !mapImageRect) return;

            Vector3 vp = worldMapCam.WorldToViewportPoint(player.position);
            // si est� fuera, puedes clamp:
            vp.x = Mathf.Clamp01(vp.x);
            vp.y = Mathf.Clamp01(vp.y);

            Vector2 size = mapImageRect.rect.size;
            Vector2 anchored = new Vector2((vp.x - 0.5f) * size.x, (vp.y - 0.5f) * size.y);
            _icon.anchoredPosition = anchored;

            // rota flecha seg�n yaw del jugador (norte arriba)
            _icon.localRotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
        }
    }
}
