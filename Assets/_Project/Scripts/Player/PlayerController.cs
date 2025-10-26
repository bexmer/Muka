using UnityEngine;
using UnityEngine.InputSystem;
using Project.Core;

namespace Project.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Refs")]
        public Camera playerCamera;

        [Header("Move")]
        public float walkSpeed = 3.5f;
        public float runSpeed = 5.8f;
        public float crouchSpeed = 2.0f;

        [Header("Jump & Gravity")]
        public float gravity = -19.62f;   // NEGATIVA
        public float jumpHeight = 1.1f;

        [Header("Ground Check")]
        public LayerMask groundMask;      // capa del suelo
        public float groundTolerance = 0.07f;

        [Header("Crouch (collider & camera)")]
        public float standHeight = 1.8f;
        public float crouchHeight = 1.2f;
        public float standCenterY = 0.9f;
        public float crouchCenterY = 0.6f;
        public float standCamY = 1.65f;
        public float crouchCamY = 1.10f;
        public float crouchLerp = 12f;

        private CharacterController _cc;
        private PlayerInput _input;
        private Vector3 _vel;
        private bool _isCrouching;
        private bool _isGrounded;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInput>();

            _cc.height = standHeight;
            _cc.center = new Vector3(0f, standCenterY, 0f);

            if (playerCamera)
            {
                var cam = playerCamera.transform.localPosition;
                cam.y = standCamY;
                playerCamera.transform.localPosition = cam;
            }
        }

        void Update()
        {
            // Bloquea toda la lógica cuando no estamos en gameplay
            if (GameManager.I != null)
            {
                var s = GameManager.I.State;
                if (s == GameState.Paused || s == GameState.Defeat || s == GameState.End)
                    return;
            }

            var actions = _input.actions;
            Vector2 move = actions["Move"].ReadValue<Vector2>();
            bool runDown = actions["Run"].IsPressed();
            bool crouchPress = actions["Crouch"].WasPerformedThisFrame();
            bool jumpDown = actions["Jump"].WasPerformedThisFrame() ||
                            (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);

            if (crouchPress) _isCrouching = !_isCrouching;

            // Grounded robusto (OverlapBox + Raycast + SphereCast)
            _isGrounded = IsGroundedUltra();

            // Movimiento horizontal
            float speed = _isCrouching ? crouchSpeed : (runDown ? runSpeed : walkSpeed);
            Vector3 dir = (transform.right * move.x + transform.forward * move.y);
            _cc.Move(dir * (speed * Time.deltaTime));

            // Gravedad + Salto
            if (_isGrounded && _vel.y < 0f) _vel.y = -2f;
            if (_isGrounded && jumpDown && !_isCrouching)
                _vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            _vel.y += gravity * Time.deltaTime;
            _cc.Move(_vel * Time.deltaTime);

            // Crouch suave: collider y cámara
            float targetHeight = _isCrouching ? crouchHeight : standHeight;
            float targetCenter = _isCrouching ? crouchCenterY : standCenterY;
            _cc.height = Mathf.Lerp(_cc.height, targetHeight, Time.deltaTime * crouchLerp);
            _cc.center = Vector3.Lerp(_cc.center, new Vector3(0f, targetCenter, 0f), Time.deltaTime * crouchLerp);

            if (playerCamera)
            {
                Vector3 cam = playerCamera.transform.localPosition;
                float targetY = _isCrouching ? crouchCamY : standCamY;
                cam.y = Mathf.Lerp(cam.y, targetY, Time.deltaTime * crouchLerp);
                playerCamera.transform.localPosition = cam;
            }
        }

        bool IsGroundedUltra()
        {
            int mask = groundMask.value == 0 ? Physics.DefaultRaycastLayers : groundMask.value;
            var b = _cc.bounds;

            // Alfombra fina bajo los pies
            float pad = 0.02f;
            Vector3 boxCenter = new Vector3(b.center.x, b.min.y + pad, b.center.z);
            Vector3 halfExt = new Vector3(_cc.radius * 0.98f, pad, _cc.radius * 0.98f);
            if (Physics.CheckBox(boxCenter, halfExt, Quaternion.identity, mask, QueryTriggerInteraction.Ignore))
                return true;

            // Distancia exacta al suelo
            if (Physics.Raycast(b.center, Vector3.down, out var hit, b.extents.y + 0.5f, mask, QueryTriggerInteraction.Ignore))
            {
                float dist = b.min.y - hit.point.y;
                if (dist <= groundTolerance) return true;
            }

            // Fallback
            float radius = Mathf.Max(0.05f, _cc.radius * 0.95f);
            Vector3 origin = new Vector3(b.center.x, b.min.y + radius + 0.02f, b.center.z);
            return Physics.SphereCast(origin, radius, Vector3.down, out _, 0.25f, mask, QueryTriggerInteraction.Ignore);
        }
    }
}
