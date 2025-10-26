using UnityEngine;

namespace Project.Missions
{
    public class MissionEvents : MonoBehaviour
    {
        public MissionController controller;

        void Awake()
        {
            if (!controller) controller = FindObjectOfType<MissionController>();
        }

        public void NextStep() => controller?.NextStep();
        public void CompleteCurrent() => controller?.CompleteCurrent();
        public void SetStepById(string id) => controller?.SetStepById(id);
        public void CompleteById(string id) => controller?.CompleteById(id);
    }
}
