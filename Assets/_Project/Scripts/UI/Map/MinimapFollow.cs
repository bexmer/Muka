using UnityEngine;

namespace Project.UI
{
    public class MinimapFollow : MonoBehaviour
    {
        [Header("Follow target (Player)")]
        public Transform target;

        [Header("Height & framing")]
        public float height = 40f;
        public Vector3 offset = Vector3.zero;

        [Header("Orientation")]
        public bool northUp = true; // si false, rota con el jugador

        Camera _cam;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            if (_cam) _cam.orthographic = true;
        }

        void LateUpdate()
        {
            if (!target) return;

            Vector3 p = target.position + offset;
            transform.position = new Vector3(p.x, height, p.z);

            if (northUp)
                transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            else
                transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
    }
}
