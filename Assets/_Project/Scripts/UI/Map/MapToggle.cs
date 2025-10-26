using UnityEngine;
using UnityEngine.InputSystem;
using Project.Core;

namespace Project.UI
{
    public class MapToggle : MonoBehaviour
    {
        [Tooltip("Canvas/Panel raíz del mapa grande (desactivado al inicio).")]
        public GameObject largeMapRoot;

        [Tooltip("PlayerInput para leer la acción 'Map' (si se deja vacío lo buscamos).")]
        public PlayerInput playerInput;

        [Tooltip("Pausar el juego al abrir el mapa grande.")]
        public bool pauseOnMap = false;

        InputAction _mapAction;

        void Start()
        {
            if (largeMapRoot) largeMapRoot.SetActive(false);

            if (!playerInput) playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput && playerInput.actions != null)
            {
                _mapAction = playerInput.actions.FindAction("Map");
                if (_mapAction != null && !_mapAction.enabled) _mapAction.Enable();
            }
        }

        void Update()
        {
            bool pressed = false;

            if (_mapAction != null && _mapAction.WasPerformedThisFrame())
                pressed = true;

            if (!pressed && Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
                pressed = true;

            if (!pressed) return;

            bool opening = !(largeMapRoot && largeMapRoot.activeSelf);

            if (largeMapRoot) largeMapRoot.SetActive(opening);

            if (pauseOnMap && GameManager.I != null)
            {
                if (opening) GameManager.I.Pause();
                else GameManager.I.Resume();
            }
        }
    }
}
