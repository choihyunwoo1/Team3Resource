using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JS
{
    public class EyeBallAbility : MonoBehaviour, IEnemyAbility
    {
        #region Variables
        private Enemy_Main owner;
        private GameManager gameManager;
        [SerializeField] Animator animator;

        [Header("Visuals")]
        [SerializeField] private GameObject eyeVisual;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 18f;   // 목표 점까지의 속도
        [SerializeField] private float waitAtWall = 0.1f; // 벽에 닿았을 때 아주 잠시 멈춤(생략 가능)

        [Header("Interval")]
        [SerializeField] private float minWaitTime = 3f;
        [SerializeField] private float maxWaitTime = 6f;

        private bool isAbilityActive = false;
        private Camera mainCam;
        private Vector3 originalPosition;

        // 벽의 종류를 정의
        private enum WallType { Top, Bottom, Left, Right }
        #endregion

        public void Setup(Enemy_Main enemy)
        {
            owner = enemy;
            gameManager = Object.FindAnyObjectByType<GameManager>();
            mainCam = Camera.main;
            animator = eyeVisual.GetComponent<Animator>();
        }

        public void OnEnter()
        {
            StopAllCoroutines();
            isAbilityActive = false;
            if (eyeVisual != null) eyeVisual.SetActive(true);
            if (gameManager != null) gameManager.OnGameOver += HandleGameOver;
            StartCoroutine(EyeRoutine());
        }

        public void OnTick()
        {
            if (!isAbilityActive) return;
            // 이동 로직은 코루틴 내에서 처리하므로 OnTick에서는 위치 고정만 관리
            if (!isMoving)
                owner.transform.position = owner.transform.position;
        }

        private bool isMoving = false; // 코루틴 이동 제어용

        private IEnumerator EyeRoutine()
        {
            while (true)
            {
                // 1. 대기 상태
                isAbilityActive = false;
                owner.transform.rotation = Quaternion.identity;
                yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

                // 2. 준비 상태
                originalPosition = owner.transform.position;
                isAbilityActive = true;
                if (animator != null) animator.SetTrigger("IsJump");
                yield return new WaitForSeconds(1f);

                // 3. 순차 이동 시작 (애니메이션 On)
                if (animator != null) animator.SetBool("IsBounce", true);

                // 순서 큐 생성: Top은 무조건 첫 번째, 나머지는 섞기
                List<WallType> wallSequence = new List<WallType> { WallType.Left, WallType.Right, WallType.Bottom };
                ShuffleList(wallSequence);
                wallSequence.Insert(0, WallType.Top); // 천장을 맨 앞으로

                // 각 벽을 순서대로 타격
                foreach (WallType wall in wallSequence)
                {
                    Vector3 targetPos = GetRandomPointOnWall(wall);
                    yield return StartCoroutine(MoveToTarget(targetPos));
                    yield return new WaitForSeconds(waitAtWall);
                }

                // 4. 원래 위치로 복귀
                yield return StartCoroutine(MoveToTarget(originalPosition));

                // 5. 종료
                if (animator != null) animator.SetBool("IsBounce", false);
                owner.transform.rotation = Quaternion.identity;
                yield return new WaitForSeconds(0.5f);
                isAbilityActive = false;
            }
        }

        private IEnumerator MoveToTarget(Vector3 target)
        {
            isMoving = true;
            while (Vector3.Distance(owner.transform.position, target) > 0.1f)
            {
                // 방향을 바라보게 회전
                Vector3 dir = (target - owner.transform.position).normalized;
                if (dir != Vector3.zero)
                {
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    owner.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
                }

                // 이동
                owner.transform.position = Vector3.MoveTowards(owner.transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
            owner.transform.position = target;
            isMoving = false;
        }

        private Vector3 GetRandomPointOnWall(WallType wall)
        {
            // 뷰포트 좌표(0~1)를 월드 좌표로 변환하여 정확한 벽 위치 계산
            float x = 0, y = 0;
            switch (wall)
            {
                case WallType.Top: x = Random.Range(0.1f, 0.9f); y = 0.95f; break;
                case WallType.Bottom: x = Random.Range(0.1f, 0.9f); y = 0.05f; break;
                case WallType.Left: x = 0.05f; y = Random.Range(0.1f, 0.9f); break;
                case WallType.Right: x = 0.95f; y = Random.Range(0.1f, 0.9f); break;
            }
            Vector3 worldPos = mainCam.ViewportToWorldPoint(new Vector3(x, y, 10f));
            worldPos.z = 0f;
            return worldPos;
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        public void OnExit() { StopAllCoroutines(); isMoving = false; }
        public void OnGameOver() => OnExit();
        private void HandleGameOver(DeathCause cause) => OnGameOver();
    }
}
