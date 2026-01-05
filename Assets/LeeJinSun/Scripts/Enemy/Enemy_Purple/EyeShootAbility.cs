using UnityEngine;
using System.Collections;

namespace JS
{
    public class EyeShootAbility : MonoBehaviour, IEnemyAbility
    {
        #region Variables
        private Enemy_Main owner;
        private GameManager gameManager;
        [SerializeField] Animator animator;
        private bool isAttacking = false;

        [Header("Visuals")]
        [SerializeField] private GameObject eyeVisual;

        [Header("Settings")]
        [SerializeField] private float minPrepareTime = 1.0f; // 랜덤 대기 최소시간
        [SerializeField] private float maxPrepareTime = 2.5f; // 랜덤 대기 최대시간
        [SerializeField] private float dashSpeed = 15f;      // 돌진 속도
        [SerializeField] private float retreatDistance = 5f; // 공격 전 뒤로 살짝 물러나는 거리
        [SerializeField] private float yFollowSmooth = 5f;   // Y축 추적 부드러움
        [SerializeField] private float ceilingY = 10f; // 천장 높이
        #endregion

        #region Custom Method
        public void Setup(Enemy_Main enemy)
        {
            owner = enemy;
            gameManager = Object.FindAnyObjectByType<GameManager>();
            animator = eyeVisual.GetComponent<Animator>();
        }

        public void OnEnter()
        {
            StopAllCoroutines();
            isAttacking = false;
            if (eyeVisual != null) eyeVisual.SetActive(true);
            StartCoroutine(ShootRoutine());
        }

        public void OnExit()
        {
            StopAllCoroutines();
            if (eyeVisual != null) eyeVisual.SetActive(false);
        }

        public void OnTick()
        {
            // 공격 중이 아닐 때만 플레이어의 Y축을 따라감
            if (!isAttacking && owner.player != null)
            {
                float targetY = owner.player.position.y;
                float newY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * yFollowSmooth);

                // X축은 고정 (보통 플레이어 왼쪽 일정 거리)
                /*float fixedX = owner.player.position.x - 8f;
                transform.position = new Vector3(fixedX, newY, transform.position.z);*/
            }
        }

        private IEnumerator ShootRoutine()
        {
            while (true)
            {
                // 1. 대기 및 Y축 추적 (OnTick에서 수행됨)
                yield return new WaitForSeconds(Random.Range(minPrepareTime, maxPrepareTime));

                // 2. 공격 예고 (뒤로 물러나기 시작)
                isAttacking = true;
                if (animator != null) animator.SetTrigger("IsJump");

                // [중요] Lerp를 쓸 때는 시작 지점(startPos)을 반드시 고정해야 미끄러지지 않음
                Vector3 startPos = transform.position;
                Vector3 preparePos = startPos + Vector3.right * retreatDistance;

                float retreatDuration = 0.4f; // 물러나는 동작의 소요 시간
                float t = 0;

                while (t < 1.0f)
                {
                    t += Time.deltaTime / retreatDuration;

                    // Mathf.SmoothStep을 쓰면 시작과 끝이 훨씬 부드러워짐 (가속/감속 효과)
                    float smoothT = Mathf.SmoothStep(0, 1, t);
                    transform.position = Vector3.Lerp(startPos, preparePos, smoothT);
                    yield return null;
                }

                // 3. 기 모으는 시간 (점프 애니메이션의 정점이나 대기 동작에 맞춰 조절)
                // 0.8초는 플레이어가 반응하기에 너무 길 수 있어 0.4~0.5초 권장
                yield return new WaitForSeconds(0.4f);

                // 4. 돌진 공격 (X축으로 길게 이동)
                if (animator != null) animator.SetBool("IsBounce", true);
                Vector3 dashTarget = transform.position + Vector3.left * 25f;
                while (Vector3.Distance(transform.position, dashTarget) > 0.5f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);
                    yield return null;
                }

                // 4. 천장 찍고 복귀 (함수 호출)
                // 현재 위치(dashEndPos)를 인자로 넘겨줍니다.
                yield return StartCoroutine(BounceReturnStep(transform.position));

                // 5. 공격 종료
                animator.SetBool("IsBounce", false);
                yield return new WaitForSeconds(1f);
                isAttacking = false;
            }
        }

        // --- 복귀 로직만 따로 분리한 함수 ---
        private IEnumerator BounceReturnStep(Vector3 currentPos)
        {
            float returnX = owner.player.position.x - 8f;
            Vector3 ceilingPos = new Vector3((currentPos.x + returnX) * 0.5f, ceilingY, transform.position.z);
            Vector3 finalPos = new Vector3(returnX, owner.player.position.y, transform.position.z);

            // A. 천장으로 상승 (튕겨 나가는 느낌)
            float t = 0;
            while (t < 1.0f)
            {
                Vector3 previousPos = transform.position; // 이전 프레임 위치 저장
                t += Time.deltaTime * 2.5f;
                transform.position = Vector3.Lerp(currentPos, ceilingPos, t * t);

                // 이동 방향을 바라보도록 회전 함수 호출
                RotateToMovement(transform.position - previousPos);

                yield return null;
            }

            // B. 천장 찍고 하강 (부드럽게 안착)
            t = 0;
            while (t < 1.0f)
            {
                Vector3 previousPos = transform.position;
                t += Time.deltaTime * 2.0f;
                transform.position = Vector3.Lerp(ceilingPos, finalPos, Mathf.SmoothStep(0, 1, t));

                RotateToMovement(transform.position - previousPos);

                yield return null;
            }

            // 복귀 완료 후 회전 초기화 (원래 각도로 부드럽게 복구하거나 즉시 리셋)
            transform.rotation = Quaternion.identity;
        }

        // 이동 방향(Vector3 direction)을 입력받아 회전시키는 헬퍼 함수
        private void RotateToMovement(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0.001f) // 아주 작은 움직임이 아닐 때만 계산
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // [중요] 눈알 이미지의 정면이 어디냐에 따라 보정값이 달라집니다.
                // 눈알의 정면(동공)이 위를 향하고 있다면 -90, 오른쪽을 향하고 있다면 0을 넣으세요.
                float angleOffset = 0f;

                transform.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
            }
        }
        #endregion
    }
}
