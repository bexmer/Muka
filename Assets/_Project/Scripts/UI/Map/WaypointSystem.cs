using UnityEngine;
using System.Collections.Generic;

namespace Project.UI
{
    public class WaypointSystem : MonoBehaviour
    {
        public static WaypointSystem I { get; private set; }

        [Tooltip("Lista de posibles objetivos (Transforms en escena).")]
        public List<Transform> waypoints = new();

        [Tooltip("Índice del waypoint activo en la lista (–1 = ninguno).")]
        public int activeIndex = -1;

        public Transform Active => (activeIndex >= 0 && activeIndex < waypoints.Count) ? waypoints[activeIndex] : null;

        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetActiveByIndex(int index) => activeIndex = (index >= 0 && index < waypoints.Count) ? index : -1;
        public void SetActive(Transform t) => activeIndex = waypoints.IndexOf(t);
        public void Clear() => activeIndex = -1;

        // Utilidad: crea y registra un waypoint al vuelo
        public Transform Register(Transform t, bool setActive = false)
        {
            if (!waypoints.Contains(t)) waypoints.Add(t);
            if (setActive) SetActive(t);
            return t;
        }
    }
}
