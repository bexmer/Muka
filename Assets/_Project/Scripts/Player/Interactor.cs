using UnityEngine;
using UnityEngine.InputSystem;
using Project.Interaction;

namespace Project.Player
{
    public class Interactor : MonoBehaviour
    {
        public Camera cam;
        public float distance = 3f;
        public LayerMask interactableMask;
        public UI.PromptWidget prompt;
        public UI.ReticleWidget reticle;   // <-- NUEVO

        PlayerInput _input;
        IInteractable _hover;

        void Awake() { _input = GetComponent<PlayerInput>(); }

        void Update()
        {
            bool hasHit = false;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);

            if (Physics.Raycast(ray, out var hit, distance, interactableMask))
            {
                // Busca IInteractable en el collider O en el padre
                var it = hit.collider.GetComponent<Project.Interaction.IInteractable>()
                         ?? hit.collider.GetComponentInParent<Project.Interaction.IInteractable>();

                if (it != null)
                {
                    hasHit = true;
                    if (_hover != it)
                    {
                        _hover = it;
                        prompt?.Show(it.Prompt);
                    }

                    if (_input.actions["Interact"].WasPerformedThisFrame())
                        it.Interact(this);
                }
            }

            if (!hasHit)
            {
                _hover = null;
                prompt?.Hide();
            }

            // retícula (si la usas)
            if (reticle) reticle.SetHover(hasHit);
        }
    }
}
