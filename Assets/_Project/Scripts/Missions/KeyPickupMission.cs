using UnityEngine;
using Project.Missions;        // este mismo namespace si quieres referenciar otros scripts de Missions
using Project.Interaction;     // InteractableBase
using Project.Player;          // Interactor, Inventory
using System;

namespace Project.Missions
{
    // Colócalo en el objeto "Llave". Requiere collider y que el Interactor le pegue.
    [RequireComponent(typeof(Collider))]
    public class KeyPickupMission : InteractableBase
    {
        [Header("Mission")]
        public string stepIdToComplete = "consigue_llave";  // ID del paso que se completa al tomar la llave

        [Header("Inventario (opcional)")]
        public string inventoryItemId = "Key_Ritual";       // si usas Inventory

        [Header("Visual")]
        public GameObject modelToHide;   // malla de la llave; si lo dejas vacío usa este GO

        private bool _taken;

        void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            // puedes usar collider trigger o no; tu Interactor hace raycast
            var c = GetComponent<Collider>();
            if (c) c.isTrigger = false;
        }

        public override string Prompt => _taken ? "" : "E: Tomar llave";

        // IMPORTANTE: el tipo de parámetro DEBE coincidir con tu InteractableBase (Interactor del namespace Project.Player)
        public override void Interact(Interactor by)
        {
            if (_taken) return;
            _taken = true;

            // ===== INVENTARIO (robusto a distintas firmas) =====
            var inv = by.GetComponent<Inventory>();
            if (inv != null && !string.IsNullOrEmpty(inventoryItemId))
            {
                bool added = false;
                var t = inv.GetType();

                // 1) Add(string, int)
                var m = t.GetMethod("Add", new Type[] { typeof(string), typeof(int) });
                if (m != null) { m.Invoke(inv, new object[] { inventoryItemId, 1 }); added = true; }

                // 2) Add(string)
                if (!added)
                {
                    m = t.GetMethod("Add", new Type[] { typeof(string) });
                    if (m != null) { m.Invoke(inv, new object[] { inventoryItemId }); added = true; }
                }

                // 3) AddItem(string) / Give(string) (por si tu API se llama distinto)
                if (!added)
                {
                    m = t.GetMethod("AddItem", new Type[] { typeof(string) }) ??
                        t.GetMethod("Give", new Type[] { typeof(string) });
                    if (m != null) { m.Invoke(inv, new object[] { inventoryItemId }); added = true; }
                }

                // (opcional) si nada coincidió, puedes loguear o ignorar silenciosamente
                // if (!added) Debug.LogWarning($"Inventory no tiene Add/AddItem/Give compatibles para '{inventoryItemId}'.");
            }
            // ===== FIN INVENTARIO =====

            // Oculta visual y desactiva colisión
            if (!modelToHide) modelToHide = gameObject;
            if (modelToHide) modelToHide.SetActive(false);
            var col = GetComponent<Collider>(); if (col) col.enabled = false;

            
            // Completar paso de misión
            MissionController mc = null;
#if UNITY_2023_1_OR_NEWER
            mc = FindFirstObjectByType<MissionController>();
#else
    mc = FindObjectOfType<MissionController>();
#endif
            mc?.CompleteById(stepIdToComplete);
        }
    }
}
