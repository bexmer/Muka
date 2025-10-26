using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Core
{
    public class SceneFlow : MonoBehaviour
    {
        public static IEnumerator LoadSceneAsync(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!op.isDone) yield return null;
        }

        public void LoadMainMenu() => StartCoroutine(LoadSceneAsync("00_MainMenu"));
        public void LoadGame() => StartCoroutine(LoadSceneAsync("01_Town_Greybox"));
        public void LoadEnd() => StartCoroutine(LoadSceneAsync("03_End"));
        public void LoadDefeat() => StartCoroutine(LoadSceneAsync("99_Defeat"));
        public void QuitApp() => Application.Quit();
    }
}
