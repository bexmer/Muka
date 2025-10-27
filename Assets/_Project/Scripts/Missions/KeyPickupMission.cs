using UnityEngine;
using Project.Interaction;   // InteractableBase
using Project.Player;        // Interactor, Inventory

namespace Project.Missions
{
    [RequireComponent(typeof(Collider))]
    public class KeyPickupMission : InteractableBase
    {
        [Header("Mission")]
        public string stepIdToComplete = "consigue_llave";

        [Header("Inventario (visual y/o real)")]
        public string inventoryItemId = "Key_Ritual";
        public string displayName = "Llave del ritual";
        public Sprite icon;

        [Header("Fallback por trigger (por si tu Interactor no llama Interact)")]
        public bool enableTriggerFallback = true; // si es true, también funciona estando dentro del trigger + E
        public KeyCode fallbackKey = KeyCode.E;

        [Header("Visual")]
        public GameObject modelToHide;

        private bool _taken;

        public override string Prompt => _taken ? "" : "E: Tomar llave";

        void Reset()
        {
            var c = GetComponent<Collider>();
            c.isTrigger = true;                  // para el fallback
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        // ======== InteractableBase (Interactor llama esto) ========
        public override void Interact(Interactor by)
        {
            Debug.Log($"[KeyPickup] Interact() | taken={_taken} | by='{by?.name}'");
            if (_taken) return;
            DoPickup(by ? by.gameObject : null);
        }

        // ======== Fallback por trigger + tecla (por si Interactor no entra) ========
        void OnTriggerStay(Collider other)
        {
            if (!enableTriggerFallback || _taken) return;
            if (!other.CompareTag("Player")) return;

            bool pressed = false;
            // Input System
            if (UnityEngine.InputSystem.Keyboard.current != null)
                pressed = UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame;
            // Input clásico
            if (!pressed) pressed = Input.GetKeyDown(fallbackKey);

            if (pressed)
            {
                Debug.Log("[KeyPickup] Fallback trigger: E presionado dentro del trigger del jugador.");
                DoPickup(other.gameObject);
            }
        }

        // ======== Lógica común ========
        private void DoPickup(GameObject playerGO)
        {
            _taken = true;
            Debug.Log($"[KeyPickup] DoPickup START | stepId='{stepIdToComplete}' | item='{inventoryItemId}'");

            // 1) Inventario REAL (si lo tienes y conoces la firma exacta, descomenta UNA línea)
            var inv = playerGO ? playerGO.GetComponent<Inventory>() : null;
            if (inv && !string.IsNullOrEmpty(inventoryItemId))
            {
                Debug.Log("[KeyPickup] Inventory encontrado. (Agrega aquí tu llamada real: Add/AddItem/Give)");
                // inv.Add(inventoryItemId);
                // inv.AddItem(inventoryItemId);
                // inv.Give(inventoryItemId);
            }
            else
            {
                Debug.Log("[KeyPickup] NO Inventory o itemId vacío. Sigo con HUD visual.");
            }

            // 2) HUD de inventario (visual)
            Project.UI.InventoryHUD hud =
#if UNITY_2023_1_OR_NEWER
                FindFirstObjectByType<Project.UI.InventoryHUD>();
#else
                FindObjectOfType<Project.UI.InventoryHUD>();
#endif
            if (hud)
            {
                Debug.Log("[KeyPickup] Agrego al HUD de inventario.");
                hud.AddItem(inventoryItemId, 1, string.IsNullOrEmpty(displayName) ? inventoryItemId : displayName, icon);
            }
            else
            {
                Debug.LogWarning("[KeyPickup] NO encontré InventoryHUD en escena.");
            }

            // 3) Ocultar malla + colisión
            if (!modelToHide) modelToHide = gameObject;
            if (modelToHide) modelToHide.SetActive(false);
            var col = GetComponent<Collider>(); if (col) col.enabled = false;
            Debug.Log("[KeyPickup] Oculto modelo y desactivo collider.");

            // 4) Avanzar misión
            MissionController mc =
#if UNITY_2023_1_OR_NEWER
                FindFirstObjectByType<MissionController>();
#else
                FindObjectOfType<MissionController>();
#endif
            if (mc)
            {
                Debug.Log($"[KeyPickup] MissionController.CompleteById('{stepIdToComplete}')");
                mc.CompleteById(stepIdToComplete);
            }
            else
            {
                Debug.LogError("[KeyPickup] NO encontré MissionController. No puedo avanzar misión.");
            }

            Debug.Log("[KeyPickup] DoPickup END");
        }
    }
}
