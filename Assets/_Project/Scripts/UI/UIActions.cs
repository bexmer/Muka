using UnityEngine;
using UnityEngine.SceneManagement;
using Project.Core;

namespace Project.UI
{
    public class UIActions : MonoBehaviour
    {
        [Header("Opcional")]
        public GameObject pauseRoot;  // asigna el panel/canvas de pausa si usarás Resume()

        // --- MAIN MENU ---
        public void PlayGame()
        {
            Time.timeScale = 1f;
            if (GameManager.I) GameManager.I.SetState(GameState.Exploration);
            StartCoroutine(SceneFlow.LoadSceneAsync("00_Boot"));
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        // --- PAUSA ---
        public void ResumeGame()
        {
            if (pauseRoot) pauseRoot.SetActive(false);
            if (GameManager.I) GameManager.I.Resume();
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            var active = SceneManager.GetActiveScene().name;
            StartCoroutine(SceneFlow.LoadSceneAsync(active));
        }

        public void BackToMenu()
        {
            Time.timeScale = 1f;
            StartCoroutine(SceneFlow.LoadSceneAsync("00_MainMenu"));
        }

        // --- DERROTA / FIN ---
        public void RetryFromDefeat()
        {
            Time.timeScale = 1f;
            // reintenta la última escena jugable (la activa)
            var active = SceneManager.GetActiveScene().name;
            StartCoroutine(SceneFlow.LoadSceneAsync(active));
        }

        public void EndToMenu()
        {
            Time.timeScale = 1f;
            StartCoroutine(SceneFlow.LoadSceneAsync("00_MainMenu"));
        }
    }
}
