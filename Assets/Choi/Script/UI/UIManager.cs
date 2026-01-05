using UnityEngine;
using UnityEngine.SceneManagement;

namespace Choi
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [Header("UI")]
        [SerializeField] private GameObject readyUI;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject stageClearUI;

        private void OnEnable()
        {
            gameManager.OnStateChanged += HandleState;

            // 현재 상태 즉시 반영 (중요)
            HandleState(gameManager.State);
        }

        private void OnDisable()
        {
            gameManager.OnStateChanged -= HandleState;
        }

        private void HandleState(GameState state)
        {
            readyUI.SetActive(false);
            pauseUI.SetActive(false);
            gameOverUI.SetActive(false);
            stageClearUI.SetActive(false); 

            switch (state)
            {
                case GameState.Ready:
                    readyUI.SetActive(true);
                    break;

                case GameState.Paused:
                    pauseUI.SetActive(true);
                    break;

                case GameState.GameOver:
                    // 게임오버 직전에 마스크 UI 끄기
                    if (MaskUIManager.Instance != null)
                        MaskUIManager.Instance.HideAll();
                    gameOverUI.SetActive(true);
                    break;

                case GameState.StageClear:
                    // 게임오버 직전에 마스크 UI 끄기
                    if (MaskUIManager.Instance != null)
                        MaskUIManager.Instance.HideAll();
                    stageClearUI.SetActive(true);
                    break;
            }
        }

        public void Retry()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("PlayScene");
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        public void Continue()
        {
            gameManager.SetState(GameState.Playing);
        }

        public void Pause()
        {
            if (gameManager.State == GameState.Playing)
            {
                gameManager.SetState(GameState.Paused);
            }
            else if (gameManager.State == GameState.Paused)
            {
                gameManager.SetState(GameState.Playing);
            }
        }
    }
}
