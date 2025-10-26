using UnityEngine;


namespace Project.Debugging
{
    public class DebugOverlay : MonoBehaviour
    {
        public bool show = true;
        void OnGUI()
        {
            if (!show) return;
            GUI.Label(new Rect(10, 10, 400, 25), $"FPS: {1f / Time.smoothDeltaTime:0}");
        }
    }
}