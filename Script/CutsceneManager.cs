using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Choi
{
    public class CutsceneManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameManager gameManager;

        [Header("Death Cutscenes")]
        [SerializeField] private GameObject enemyACutscene;
        [SerializeField] private GameObject fallCutscene;

        // 컷씬 종료 신호 → DiarySystem에서 수집
        public UnityEvent<DeathCause> OnCutsceneFinished;

        private bool isPlaying;
        #endregion

        #region Property
        public static CutsceneManager Instance { get; private set; }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        void Start()
        {
            Player player = FindObjectOfType<Player>();
            player.OnPlayerDied += PlayDeathCutscene;
        }

        private void OnEnable()
        {
            gameManager.OnGameOver += PlayDeathCutscene;
        }

        private void OnDisable()
        {
            gameManager.OnGameOver -= PlayDeathCutscene;
        }
        #endregion

        #region Custom Method
        public void PlayDeathCutscene(DeathCause cause)
        {
            if (isPlaying)
                return;

            switch (cause)
            {
                case DeathCause.EnemyA:
                    StartCoroutine(Play(enemyACutscene, 2.5f, cause));
                    break;

                case DeathCause.Fall:
                    StartCoroutine(Play(fallCutscene, 3.0f, cause));
                    break;
            }
        }

        private IEnumerator Play(GameObject cutscene, float duration, DeathCause cause)
        {
            isPlaying = true;

            cutscene.SetActive(true);
            yield return new WaitForSecondsRealtime(duration);
            cutscene.SetActive(false);

            isPlaying = false;

            // 컷씬 종료 → 게임 상태 변경
            gameManager.NotifyGameOverCutsceneFinished();

            // 컷씬 종료 → Diary 기록 신호
            OnCutsceneFinished?.Invoke(cause);
        }
        #endregion
    }
}
