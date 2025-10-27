using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Project.UI; // HUDController
// Nota: WaypointSystem está en Project.UI también

namespace Project.Missions
{
    [Serializable]
    public class MissionStep
    {
        public string id = "consigue_llave";
        [TextArea] public string objectiveText = "Toma la llave";
        public int waypointIndex = -1;           // -1 = sin waypoint
        public bool autoAdvanceOnTrigger = true; // si un MissionTrigger con ese id avanza
    }

    public class MissionController : MonoBehaviour
    {
        [Header("Steps (en orden)")]
        public List<MissionStep> steps = new List<MissionStep>();

        [Header("Estado")]
        [SerializeField] private int currentIndex = -1;
        public bool IsRunning => currentIndex >= 0 && currentIndex < steps.Count;
        public MissionStep Current => IsRunning ? steps[currentIndex] : null;

        [Header("Refs")]
        public HUDController hud; // si no lo asignas, lo busco

        [Header("Eventos")]
        public UnityEvent<int> OnStepChanged;
        public UnityEvent OnMissionCompleted;

        // -------------------- Ciclo de vida --------------------

        private void Awake()
        {
            // No buscamos HUD aquí para evitar warnings en escenas sin HUD.
        }

        private void Start()
        {
            Debug.Log($"[Mission] Start | steps={steps.Count} | currentIndex={currentIndex}");

            // Fuerza sincronizar HUD/waypoint con el estado actual
            if (steps.Count > 0)
            {
                if (currentIndex < 0) SetStep(0);
                else SetStep(currentIndex);
            }
        }

        // -------------------- API pública --------------------

        /// <summary>Avanza al siguiente paso.</summary>
        public void NextStep()
        {
            if (!IsRunning) { SetStep(0); return; }
            SetStep(currentIndex + 1);
        }

        /// <summary>Completa el paso actual y pasa al siguiente.</summary>
        public void CompleteCurrent()
        {
            if (!IsRunning) return;
            Debug.Log($"[Mission] CompleteCurrent | idx={currentIndex} '{steps[currentIndex].id}'");
            SetStep(currentIndex + 1);
        }

        /// <summary>Completa el paso solo si su id coincide con el actual.</summary>
        public void CompleteById(string id)
        {
            string curId = IsRunning ? steps[currentIndex].id : "<none>";
            Debug.Log($"[Mission] CompleteById('{id}') | current='{curId}'");
            if (IsRunning && steps[currentIndex].id == id)
                CompleteCurrent();
        }

        /// <summary>Llamado por MissionTrigger al entrar el jugador en el trigger.</summary>
        public void OnTriggerForStep(string id)
        {
            string curId = IsRunning ? steps[currentIndex].id : "<none>";
            Debug.Log($"[Mission] OnTriggerForStep('{id}') | current='{curId}'");
            if (!IsRunning) return;

            var cur = steps[currentIndex];
            if (cur.id != id) return;
            if (cur.autoAdvanceOnTrigger) CompleteCurrent();
        }

        /// <summary>Salta directamente a un paso por su id.</summary>
        public void SetStepById(string id)
        {
            int idx = steps.FindIndex(s => s.id == id);
            if (idx >= 0) SetStep(idx);
            else Debug.LogWarning($"[Mission] No existe paso con id '{id}'.");
        }

        // -------------------- Internals --------------------

        private void SetStep(int idx)
        {
            Debug.Log($"[Mission] SetStep({idx})");

            // Misión completada
            if (idx >= steps.Count || steps.Count == 0)
            {
                Debug.Log("[Mission] COMPLETED. Limpio HUD y waypoint.");
                currentIndex = -1;
                UpdateHUD(null);
                WaypointSystem.I?.Clear();
                OnMissionCompleted?.Invoke();
                return;
            }

            // Clamp y aplica
            currentIndex = Mathf.Clamp(idx, 0, steps.Count - 1);
            var step = steps[currentIndex];
            Debug.Log($"[Mission] Now step {currentIndex} | id='{step.id}' | text='{step.objectiveText}' | waypointIndex={step.waypointIndex}");

            // HUD
            UpdateHUD(step.objectiveText);

            // Waypoint
            if (step.waypointIndex >= 0)
            {
                Debug.Log($"[Mission] Activando waypoint index={step.waypointIndex}");
                WaypointSystem.I?.SetActiveByIndex(step.waypointIndex);
            }
            else
            {
                Debug.Log("[Mission] Sin waypoint para este paso. Clear().");
                WaypointSystem.I?.Clear();
            }

            OnStepChanged?.Invoke(currentIndex);
        }

        private void UpdateHUD(string text)
        {
            if (!hud)
            {
#if UNITY_2023_1_OR_NEWER
                hud = FindFirstObjectByType<HUDController>();
#else
                hud = FindObjectOfType<HUDController>();
#endif
                Debug.Log($"[Mission] HUD ref={(hud ? hud.name : "NULL")}");
            }

            if (hud)
            {
                hud.SetObjective(text ?? "");
                Debug.Log($"[Mission] HUD objective='{(text ?? "")}'");
            }
            else
            {
                Debug.LogWarning("[Mission] No hay HUDController en escena.");
            }
        }
    }
}
