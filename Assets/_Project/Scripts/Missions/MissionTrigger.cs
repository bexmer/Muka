using UnityEngine;

namespace Project.Missions
{
    // Pon este script en un GameObject con Collider marcado como isTrigger
    // y asigna el ID del paso que debe completar.
    [RequireComponent(typeof(Collider))]
    public class MissionTrigger : MonoBehaviour
    {
        public string stepId = "ve_a_la_iglesia";
        public MissionController controller; // si no lo asignas, lo buscamos

        void Reset()
        {
            var c = GetComponent<Collider>();
            if (c) c.isTrigger = true;
        }

        void Awake()
        {
            if (!controller) controller = FindObjectOfType<MissionController>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (!controller) controller = FindObjectOfType<MissionController>();
            controller?.OnTriggerForStep(stepId);
        }
    }
}
