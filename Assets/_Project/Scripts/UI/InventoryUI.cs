using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Project.UI
{
    /// <summary>
    /// Muestra una lista de items (icono + nombre + cantidad).
    /// No depende de tu Inventory. Llama AddItem/RemoveItem desde tu gameplay.
    /// </summary>
    public class InventoryHUD : MonoBehaviour
    {
        [Header("UI")]
        public RectTransform content;     // contenedor (Vertical/Horizontal/Grid)
        public GameObject slotPrefab;     // prefab con Image (icon), TMP_Text (name), TMP_Text (count)
        public Sprite defaultIcon;

        // Estado actual mostrado
        private readonly Dictionary<string, Slot> _slots = new Dictionary<string, Slot>();

        // ---- API pública ----
        public void AddItem(string id, int amount = 1, string displayName = null, Sprite icon = null)
        {
            if (string.IsNullOrEmpty(id) || amount <= 0) return;

            if (!_slots.TryGetValue(id, out var slot))
            {
                var go = Instantiate(slotPrefab, content);
                slot = new Slot(go);
                _slots[id] = slot;
                slot.SetIcon(icon ? icon : defaultIcon);
                slot.SetName(!string.IsNullOrEmpty(displayName) ? displayName : id);
                slot.SetCount(0);
            }

            slot.SetCount(slot.Count + amount);
        }

        public void SetCount(string id, int count, string displayName = null, Sprite icon = null)
        {
            if (string.IsNullOrEmpty(id) || count < 0) return;

            if (!_slots.TryGetValue(id, out var slot))
            {
                var go = Instantiate(slotPrefab, content);
                slot = new Slot(go);
                _slots[id] = slot;
                slot.SetIcon(icon ? icon : defaultIcon);
                slot.SetName(!string.IsNullOrEmpty(displayName) ? displayName : id);
            }

            slot.SetCount(count);

            if (slot.Count == 0)
            {
                Destroy(slot.Root);
                _slots.Remove(id);
            }
        }

        public void RemoveItem(string id, int amount = 1)
        {
            if (string.IsNullOrEmpty(id) || amount <= 0) return;
            if (!_slots.TryGetValue(id, out var slot)) return;

            slot.SetCount(Mathf.Max(0, slot.Count - amount));

            if (slot.Count == 0)
            {
                Destroy(slot.Root);
                _slots.Remove(id);
            }
        }

        // ---- Clase interna para manejar un row de UI ----
        private class Slot
        {
            public GameObject Root { get; }
            private readonly Image _icon;
            private readonly TMP_Text _name;
            private readonly TMP_Text _count;
            public int Count { get; private set; }

            public Slot(GameObject root)
            {
                Root = root;
                _icon = root.GetComponentInChildren<Image>(true);
                var tmps = root.GetComponentsInChildren<TMP_Text>(true);
                // asume el primer TMP es el nombre y el segundo (si hay) es la cantidad
                if (tmps.Length > 0) _name = tmps[0];
                if (tmps.Length > 1) _count = tmps[1];
            }

            public void SetIcon(Sprite s) { if (_icon) _icon.sprite = s; }
            public void SetName(string n) { if (_name) _name.text = n; }
            public void SetCount(int c)
            {
                Count = c;
                if (_count) _count.text = (c > 1) ? $"x{c}" : "";
            }
        }
    }
}
