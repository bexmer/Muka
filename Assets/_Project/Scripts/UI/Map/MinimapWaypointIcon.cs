using UnityEngine;

namespace Project.UI
{
    // Colócalo en el icono (Image) del waypoint dentro del RawImage del MINIMAPA.
    public class MinimapWaypointIcon : MonoBehaviour
    {
        [Header("Refs")]
        public Camera minimapCam;              // ortho top-down
        public RectTransform minimapRect;      // RectTransform del RawImage del minimapa
        public Transform player;               // para compensar rotación si hace falta
        public WaypointSystem system;          // si lo dejas null buscará el singleton

        [Header("UI Elements")]
        public RectTransform iconRect;         // punto amarillo (dentro del mapa)
        public RectTransform edgeArrowRect;    // flecha en el borde (cuando está fuera)

        [Header("Behaviour")]
        public bool minimapNorthUp = true;     // DEBE coincidir con tu MinimapFollow.northUp
        [Range(0f, 30f)] public float edgePadding = 8f;     // margen dentro del marco
        [Range(0f, 0.2f)] public float innerMargin = 0.0f;  // margen interior para “dentro”

        void Reset()
        {
            iconRect = GetComponent<RectTransform>();
        }

        void LateUpdate()
        {
            if (!minimapCam || !minimapRect) return;
            if (!system) system = WaypointSystem.I;
            Transform target = (system ? system.Active : null);
            if (!target)
            {
                SetIcon(false);
                SetArrow(false);
                return;
            }

            // 1) Proyección a viewport (0..1)
            Vector3 vp = minimapCam.WorldToViewportPoint(target.position);

            // Si por alguna razón aparece "detrás" de la cámara, trátalo como fuera
            bool isBehind = vp.z < 0f;

            // “Dentro” con margen configurable
            bool inside =
                !isBehind &&
                vp.x >= innerMargin && vp.x <= (1f - innerMargin) &&
                vp.y >= innerMargin && vp.y <= (1f - innerMargin);

            if (inside)
            {
                // --- Mostrar punto amarillo dentro del minimapa ---
                Vector2 size = minimapRect.rect.size;
                Vector2 anchored = new Vector2((vp.x - 0.5f) * size.x, (vp.y - 0.5f) * size.y);
                if (iconRect)
                {
                    iconRect.anchoredPosition = anchored;
                    SetIcon(true);
                }
                SetArrow(false); // asegúrate de esconder la flecha
            }
            else
            {
                // --- Mostrar flecha pegada al borde ---
                if (edgeArrowRect)
                {
                    Vector2 half = minimapRect.rect.size * 0.5f - Vector2.one * edgePadding;

                    // Dirección desde el centro del minimapa hacia el waypoint en espacio viewport
                    Vector2 dir = new Vector2(vp.x - 0.5f, vp.y - 0.5f);
                    if (dir.sqrMagnitude < 1e-6f) dir = Vector2.up;
                    dir.Normalize();

                    // Coloca en el borde del rect (clamp rectangular)
                    Vector2 pos = dir * half;
                    // Ajuste para que no se salga en diagonal (mantén ratio de borde)
                    pos.x = Mathf.Clamp(pos.x, -half.x, half.x);
                    pos.y = Mathf.Clamp(pos.y, -half.y, half.y);

                    edgeArrowRect.anchoredPosition = pos;

                    // Rotación de la flecha:
                    // - Si el minimapa es NorthUp (no rota), apunta según "dir".
                    // - Si el minimapa rota con el jugador, compensa el yaw del player/cámara.
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // 0=→, 90=↑
                    if (!minimapNorthUp && player)
                    {
                        // resta el yaw del jugador para que el indicador sea estable en pantalla
                        angle -= player.eulerAngles.y;
                    }
                    edgeArrowRect.localRotation = Quaternion.Euler(0, 0, angle - 90f); // asumiendo sprite hacia ↑

                    SetArrow(true);
                }
                SetIcon(false);
            }
        }

        // Helpers seguros para evitar que se “quede” activo
        void SetIcon(bool v) { if (iconRect) iconRect.gameObject.SetActive(v); }
        void SetArrow(bool v) { if (edgeArrowRect) edgeArrowRect.gameObject.SetActive(v); }
    }
}
