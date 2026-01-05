using UnityEngine;
using System.Collections;

namespace JS
{
    public class CutsceneManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameManager gameManager;

        [Header("Death Cutscenes")]
        [SerializeField] private GameObject enemyACutscene;
        [SerializeField] private GameObject fallCutscene;

        private bool isPlaying;
        #endregion

        #region Unity Event Method
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
            // 이미 컷신 재생 중이면 무시
            if (isPlaying)
                return;

            switch (cause)
            {
                case DeathCause.EnemyA:
                    StartCoroutine(Play(enemyACutscene, 2.5f));
                    break;

                case DeathCause.Fall:
                    StartCoroutine(Play(fallCutscene, 3.0f));
                    break;
            }
        }

        private IEnumerator Play(GameObject cutscene, float duration)
        {
            isPlaying = true;

            cutscene.SetActive(true);
            yield return new WaitForSecondsRealtime(duration);
            cutscene.SetActive(false);

            isPlaying = false;

            // 컷신 종료 알림
            gameManager.NotifyGameOverCutsceneFinished();
        }
        #endregion
    }
}
