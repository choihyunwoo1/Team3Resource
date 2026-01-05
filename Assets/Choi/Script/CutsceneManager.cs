using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Choi
{ 
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager Instance;

        #region Variables
        [SerializeField] private GameManager gameManager;

        [Header("Death Cutscenes")]
        [SerializeField] private GameObject enemyCutscene;
        [SerializeField] private GameObject fallCutscene;
        [SerializeField] private GameObject obstacleCutscene;

        [Header("Ending Cutscenes (총 8종)")]
        [SerializeField] private GameObject zeroItemEnding;
        [SerializeField] private GameObject oneToThreeEnding;
        [SerializeField] private GameObject allFiveEnding;
        [SerializeField] private GameObject missingAEnding;
        [SerializeField] private GameObject missingBEnding;
        [SerializeField] private GameObject missingCEnding;
        [SerializeField] private GameObject missingDEnding;
        [SerializeField] private GameObject missingEEnding;

        // 컷씬 종료 이벤트
        public UnityEvent<string> OnCutsceneEndEvent;

        private bool isPlaying = false;
        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (OnCutsceneEndEvent == null)
                OnCutsceneEndEvent = new UnityEvent<string>();
        }

        // ============================================================
        // 1) DEATH CUTSCENE
        // ============================================================
        public void PlayDeathCutscene(DeathCause cause)
        {
            if (isPlaying) return;

            GameObject target = null;

            switch (cause)
            {
                case DeathCause.Enemy:
                    target = enemyCutscene;
                    break;

                case DeathCause.Fall:
                    target = fallCutscene;
                    break;

                case DeathCause.Obstacle:
                    target = obstacleCutscene;
                    break;
            }

            StartCoroutine(PlayCutscene(target, cause.ToString(), GameState.GameOverCutscene));
        }


        // ============================================================
        // 2) ENDING CUTSCENE (8종)
        // ============================================================
        public void PlayEndingCutscene(EndingType type)
        {
            if (isPlaying) return;

            GameObject target = type switch
            {
                EndingType.ZeroItem => zeroItemEnding,
                EndingType.OneToThree => oneToThreeEnding,
                EndingType.AllFive => allFiveEnding,
                EndingType.MissingA => missingAEnding,
                EndingType.MissingB => missingBEnding,
                EndingType.MissingC => missingCEnding,
                EndingType.MissingD => missingDEnding,
                EndingType.MissingE => missingEEnding,
                _ => null
            };

            if (target == null)
            {
                Debug.LogWarning($"Ending Cutscene not found for type: {type}");
                return;
            }

            StartCoroutine(PlayCutscene(target, type.ToString(), GameState.StageClearCutscene));
        }

        // ============================================================
        // 공통 컷씬 플레이어
        // ============================================================
        private IEnumerator PlayCutscene(GameObject cutsceneObj, string cutsceneName, GameState duringState)
        {
            isPlaying = true;

            cutsceneObj.SetActive(true);
            gameManager.SetState(duringState);

            Animator anim = cutsceneObj.GetComponentInChildren<Animator>();
            float animLength = 0f;

            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);

                // 첫 번째 스테이트의 길이 가져오기
                AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
                animLength = info.length;

                anim.Play(0);
            }

            // 애니메이션 길이 기다림
            if (animLength > 0f)
                yield return new WaitForSecondsRealtime(animLength);
            else
                yield return new WaitForSecondsRealtime(2f); // fallback

            // 컷씬 강제 종료
            EndCutscene();

            // ---------------- 종료 처리 ----------------
            cutsceneObj.SetActive(false);

            if (duringState == GameState.GameOverCutscene)
                gameManager.SetState(GameState.GameOver);
            else if (duringState == GameState.StageClearCutscene)
                gameManager.SetState(GameState.StageClear);

            OnCutsceneEndEvent.Invoke(cutsceneName);
        }


        // 외부에서 호출되는 컷씬 종료 신호
        public void EndCutscene()
        {
            isPlaying = false;
        }
    }
}
