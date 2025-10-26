using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Project.UI;    // WaypointSystem
using Project.Core; // GameManager (opcional)

namespace Project.Missions
{
    [Serializable]
    public class MissionStep
    {
        public string id = "ve_a_la_iglesia";      // ID único para este paso
        [TextArea] public string objectiveText;    // Texto para HUD: “Ve a la iglesia”
        public int waypointIndex = -1;             // Índice en WaypointSystem (–1 = sin waypoint)
        public bool autoAdvanceOnTrigger = true;   // Si lo completa un MissionTrigger
    }

    public class MissionController : MonoBehaviour
    {
        [Header("Steps (en orden)")]
        public List<MissionStep> steps = new();

        [Header("Estado")]
        [SerializeField] private int currentIndex = -1;

        [Header("Refs (opcional)")]
        public HUDController hud; // si no lo asignas, lo busca en escena

        [Header("Eventos")]
        public UnityEvent<int> OnStepChanged;  // int = índice nuevo
        public UnityEvent OnMissionCompleted;

        public bool IsRunning => currentIndex >= 0 && currentIndex < steps.Count;
        public MissionStep Current => (IsRunning ? steps[currentIndex] : null);

        void Awake()
        {
            if (!hud) hud = FindObjectOfType<HUDController>();
        }

        void Start()
        {
            // Arranca en el primer paso si hay alguno
            if (steps.Count > 0 && currentIndex < 0)
                SetStep(0);
        }

        // ---- API pública ----

        public void SetStepById(string id)
        {
            int idx = steps.FindIndex(s => s.id == id);
            if (idx >= 0) SetStep(idx);
            else Debug.LogWarning($"[Mission] No existe paso con id '{id}'.");
        }

        public void NextStep()
        {
            if (!IsRunning) { SetStep(0); return; }
            SetStep(currentIndex + 1);
        }

        public void CompleteCurrent()
        {
            if (!IsRunning) return;
            NextStep();
        }

        public void CompleteById(string id)
        {
            // Completa actual sólo si coincide el id (evita avanzar desde triggers de otro paso)
            if (IsRunning && steps[currentIndex].id == id)
                CompleteCurrent();
        }

        // Llamado por MissionTrigger cuando el player entra
        public void OnTriggerForStep(string id)
        {
            if (!IsRunning) return;
            var cur = steps[currentIndex];
            if (cur.id != id) return;
            if (cur.autoAdvanceOnTrigger) CompleteCurrent();
        }

        // ---- Internals ----

        void SetStep(int idx)
        {
            // Fin de misión
            if (idx >= steps.Count)
            {
                currentIndex = -1;
                UpdateHUD(null);
                WaypointSystem.I?.Clear();
                OnMissionCompleted?.Invoke();
                // (opcional) avisar al GameManager de estado
                return;
            }

            currentIndex = Mathf.Clamp(idx, 0, steps.Count - 1);
            var step = steps[currentIndex];

            // HUD
            UpdateHUD(step?.objectiveText);

            // Waypoint
            if (step != null && step.waypointIndex >= 0)
                WaypointSystem.I?.SetActiveByIndex(step.waypointIndex);
            else
                WaypointSystem.I?.Clear();

            OnStepChanged?.Invoke(currentIndex);
        }

        void UpdateHUD(string text)
        {
            if (!hud)
            {
                // Unity 6/2023+: usa FindFirstObjectByType; en versiones previas cae al viejo método
#if UNITY_2023_1_OR_NEWER
                hud = FindFirstObjectByType<Project.UI.HUDController>();
#else
        hud = FindObjectOfType<Project.UI.HUDController>();
#endif
            }

            if (hud) hud.SetObjective(text ?? "");
        }

    }
}
