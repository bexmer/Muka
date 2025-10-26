using UnityEngine;

namespace Project.Missions
{
    // Activa o desactiva objetos según el paso activo de la misión
    public class MissionStepGate : MonoBehaviour
    {
        [Header("Configura CUÁNDO debe estar activo este objeto")]
        public string stepId = "consigue_llave";  // ID del paso al que responde
        public bool activeWhenOnStep = true;      // true: activo SOLO en este paso; false: activo en todos MENOS este paso

        [Header("Objetos a togglear (si lo dejas vacío usa este GameObject)")]
        public GameObject[] targets;

        void Awake()
        {
            if (targets == null || targets.Length == 0) targets = new[] { gameObject };
        }

        // Llama este método desde On Step Changed (int)
        public void HandleStepChanged(int stepIndex)
        {
            var mc = FindObjectOfType<MissionController>();
            if (!mc || !mc.IsRunning) { SetActive(false); return; }

            bool isThis = mc.Current != null && mc.Current.id == stepId;
            SetActive(activeWhenOnStep ? isThis : !isThis);
        }

        void SetActive(bool v)
        {
            foreach (var t in targets)
                if (t) t.SetActive(v);
        }
    }
}
