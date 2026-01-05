using UnityEngine;
using System.Collections;

namespace Choi
{
    /// <summary>
    /// Enemy 외형: 손, 능력: 플레이어에게 주먹 공격
    /// </summary>
    public class PunchAbility : MonoBehaviour, IEnemyAbility
    {
        #region Variables
        private Enemy_Main owner; // Enemy 본체 참조
        private Animator animator; // 애니메이터 참조

        [Header("Visuals")]
        [SerializeField] private GameObject punchVisual; // 자식으로 넣은 '손 모양' 오브젝트

        [Header("Punch Settings")]
        [SerializeField] private GameObject punchPrefab;  // 떨어뜨릴 펀치 프리팹
        [SerializeField] private Transform punchPoint;    // 펀치 생성 위치(추적용)
        [SerializeField] private float punchFollowSmoothness = 10f;

        [Header("Cycle Settings")]
        [SerializeField] private float minWaitTime = 2f;
        [SerializeField] private float maxWaitTime = 4f;
        [SerializeField] private int minPunchCount = 2;
        [SerializeField] private int maxPunchCount = 5;
        [SerializeField] private float punchInterval = 1f;

        [Header("PunchPoint Settings")]
        [SerializeField] private float punchHeight = 6f;
        [SerializeField] private float offSet = -0.5f;

        private float punchTimer;

        #endregion

        #region Custom Method
        // 1. 초기 설정: Enemy 본체가 자신을 등록할 때 호출
        public void Setup(Enemy_Main enemy)
        {
            owner = enemy;
            // punchVisual이나 자식에 있는 애니메이터를 가져옵니다.
            animator = punchVisual.GetComponentInChildren<Animator>(true);
        }

        // 2. 능력 시작: 외형을 바꾸고 타이머 초기화
        public void OnEnter()
        {
            //혹시 진행되는 코루틴들 멈추게 하기
            StopAllCoroutines();

            //펀치 코루틴 시작
            StartCoroutine(PunchSequenceRoutine());

            owner.speed *= 0.5f;
        }

        // 3. 실행: Enemy의 Update에서 매 프레임 호출됨
        public void OnTick()
        {
            UpdatePunchPointPosition();
        }

        // 4. 능력 종료: 외형을 끄고 상태 정리
        public void OnExit()
        {
            if (punchVisual != null) punchVisual.SetActive(false);
            if (punchPoint != null) punchPoint.gameObject.SetActive(false);

            StopAllCoroutines();
            owner.speed *= 2f;
        }

        private IEnumerator PunchSequenceRoutine()
        {
            while (true)
            {
                // 1. [추격 단계] 애니메이션을 '손' 상태로
                if (punchVisual != null)
                {
                    punchVisual.SetActive(true);
                    animator.SetBool("IsAttack", false);
                }
                if (punchPoint != null)
                {
                    punchPoint.gameObject.SetActive(false);
                }

                float chasingTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(chasingTime);

                // 2. [공격 준비 단계] 애니메이션을 '주먹' 상태로
                if (animator != null)
                {
                    // 여기서 다시 한번 오브젝트가 켜져 있는지 확인
                    if (!punchVisual.activeSelf) punchVisual.SetActive(true);

                    animator.SetBool("IsAttack", true);
                }

                yield return new WaitForSeconds(1.75f); // 변신 후 딜레이


                // 3. [연속 발사 단계]
                int shootCount = Random.Range(minPunchCount, maxPunchCount + 1);
                for (int i = 0; i < shootCount; i++)
                {
                    if (punchVisual != null) punchVisual.SetActive(false);
                    if (punchPoint != null) punchPoint.gameObject.SetActive(true);

                    ExecutePunch();

                    float timer = punchInterval;
                    while (timer > 0)
                    {
                        timer -= Time.deltaTime;
                        yield return null;
                    }
                }
                yield return new WaitForSeconds(1f);

                if (punchVisual != null) punchVisual.SetActive(true);
                if (punchPoint != null) punchPoint.gameObject.SetActive(false);
            }
        }
        private void UpdatePunchPointPosition()
        {
            if (owner.player == null || punchPoint == null) return;

            float targetX = Mathf.Lerp(punchPoint.position.x, owner.player.position.x + offSet, Time.deltaTime * punchFollowSmoothness);
            punchPoint.position = new Vector3(targetX, punchHeight, 0f);
        }

        private void ExecutePunch()
        {
            if (punchPrefab != null && punchPoint != null)
            {
                Instantiate(punchPrefab, punchPoint.position, Quaternion.identity);
            }
        }
        #endregion
    }
}
