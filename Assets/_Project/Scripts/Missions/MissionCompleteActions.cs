using UnityEngine;
using Project.Core;

namespace Project.Missions
{
    public class MissionCompleteActions : MonoBehaviour
    {
        public void EndDemo()
        {
            if (GameManager.I != null) GameManager.I.EndDemo();
        }

        public void LoadEndScene()
        {
            StartCoroutine(Project.Core.SceneFlow.LoadSceneAsync("03_End"));
        }

        public void BackToMenu()
        {
            Time.timeScale = 1f;
            StartCoroutine(Project.Core.SceneFlow.LoadSceneAsync("00_MainMenu"));
        }
    }
}
