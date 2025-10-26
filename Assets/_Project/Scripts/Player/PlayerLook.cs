using UnityEngine;
using UnityEngine.InputSystem;
using Project.Core;

namespace Project.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("Asigna la cámara hija")]
        public Transform cam;

        [Header("Settings")]
        public float sensitivity = 0.08f;
        public float minPitch = -75f;
        public float maxPitch = 80f;

        private PlayerInput _input;
        private float _yaw;
        private float _pitch;

        void Awake()
        {
            _input = GetComponent<PlayerInput>();
            if (!cam)
            {
                var c = GetComponentInChildren<Camera>();
                if (c) cam = c.transform;
            }
            _yaw = transform.localEulerAngles.y;
            if (cam) _pitch = NormalizePitch(cam.localEulerAngles.x);
        }

        void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void Update()
        {
            // No mirar cuando está pausado / derrota / fin
            if (GameManager.I != null)
            {
                var s = GameManager.I.State;
                if (s == GameState.Paused || s == GameState.Defeat || s == GameState.End)
                    return;
            }

            Vector2 look = _input.actions["Look"].ReadValue<Vector2>();
            _yaw += look.x * sensitivity;
            _pitch -= look.y * sensitivity;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
            if (cam) cam.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        static float NormalizePitch(float x)
        {
            if (x > 180f) x -= 360f;
            return x;
        }
    }
}
