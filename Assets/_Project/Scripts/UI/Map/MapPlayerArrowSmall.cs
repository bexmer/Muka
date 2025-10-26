using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class MapPlayerArrowSmall : MonoBehaviour
    {
        [Tooltip("Transform del jugador (usamos su yaw).")]
        public Transform player;
        [Tooltip("Si el minimapa es 'northUp', el icono rota; si rota con el jugador, deja false.")]
        public bool rotateIcon = true;

        RectTransform _rt;

        void Awake() { _rt = GetComponent<RectTransform>(); }

        void LateUpdate()
        {
            if (!player || !rotateIcon) return;
            Vector3 e = player.eulerAngles;
            _rt.localRotation = Quaternion.Euler(0, 0, -e.y); // z negativo para UI
        }
    }
}
