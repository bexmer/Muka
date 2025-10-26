using UnityEngine;
using UnityEngine.InputSystem;
using Project.Core;

namespace Project.UI
{
    public class PauseToggle : MonoBehaviour
    {
        [Tooltip("Canvas o panel del menú de pausa (debe iniciar desactivado).")]
        public GameObject pauseRoot;

        [Tooltip("PlayerInput del jugador (si lo dejas vacío lo busca).")]
        public PlayerInput playerInput;

        private GameManager _gm;
        private InputAction _pauseAction;

        private void Awake()
        {
            _gm = GameManager.I ?? FindObjectOfType<GameManager>();
            if (!playerInput) playerInput = FindObjectOfType<PlayerInput>();

            if (playerInput && playerInput.actions != null)
            {
                _pauseAction = playerInput.actions.FindAction("Pause");
            }
        }

        private void Start()
        {
            if (pauseRoot) pauseRoot.SetActive(false);
            if (_pauseAction != null && !_pauseAction.enabled) _pauseAction.Enable();
        }

        private void Update()
        {
            bool pressed = false;

            // Acción "Pause"
            if (_pauseAction != null && _pauseAction.WasPerformedThisFrame())
                pressed = true;

            // Fallback por teclado (por si no configuraste la acción)
            if (!pressed && Keyboard.current != null)
            {
                if (Keyboard.current.pKey.wasPressedThisFrame ||
                    Keyboard.current.escapeKey.wasPressedThisFrame)
                    pressed = true;
            }

            if (!pressed) return;

            if (_gm == null) _gm = GameManager.I ?? FindObjectOfType<GameManager>();
            if (_gm == null) return;

            if (_gm.State != GameState.Paused)
            {
                _gm.Pause();                 // TimeScale=0, cursor visible
                if (pauseRoot) pauseRoot.SetActive(true);
            }
            else
            {
                if (pauseRoot) pauseRoot.SetActive(false);
                _gm.Resume();                // TimeScale=1, cursor lock
            }
        }
    }
}
