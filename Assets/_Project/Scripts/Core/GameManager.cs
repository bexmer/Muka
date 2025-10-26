using UnityEngine;
using UnityEngine.Events;

namespace Project.Core
{
    public enum GameState { MainMenu, Exploration, Cine, Paranormal, Chase, Escape, Paused, Defeat, End }

    public class GameManager : MonoBehaviour
    {
        public static GameManager I { get; private set; }

        [field: SerializeField] public GameState State { get; private set; } = GameState.MainMenu;

        public static event UnityAction<GameState> OnGameStateChanged;

        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetState(GameState next)
        {
            if (State == next) return;
            State = next;
            OnGameStateChanged?.Invoke(State);
        }

        // ---- Control de flujo: pausa/derrota/fin ----
        public void Pause()
        {
            if (State == GameState.Paused) return;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetState(GameState.Paused);
        }

        public void Resume()
        {
            if (State != GameState.Paused) return;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // vuelve al estado previo “jugable”; por simplicidad mandamos a Exploration si estabas jugando
            SetState(GameState.Exploration);
        }

        public void Defeat()
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetState(GameState.Defeat);
        }

        public void EndDemo()
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetState(GameState.End);
        }
    }
}
