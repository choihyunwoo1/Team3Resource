using UnityEngine;
using UnityEngine.Events;

namespace JS
{
    /// <summary>
    /// 플레이어에게 공격하기 전 경고선
    /// </summary>
    public class DangerLine : MonoBehaviour
    {
        #region Variables
        //경고선 - 참조
        private LineRenderer dangerLine;
        // 외부에서 설정할 타겟
        public Transform targetTransform;
        public Transform startTransform;

        // Raycast 관련 설정
        [Header("Raycast Settings")]
        [Tooltip("Raycast가 감지할 레이어 마스크")]
        public LayerMask hitLayerMask;
        [Tooltip("Raycast 최대 거리 (플레이어가 너무 멀리 있을 경우 대비)")]
        public float maxDistance = 50f;

        // 타이밍 설정
        [Header("Timing")]
        [Tooltip("경고선이 타겟을 추적하는 시간")]
        public float trackingDuration = 0.5f;
        [Tooltip("고정된 후 경고선이 사라지기까지의 유지 시간")]
        public float fixedDuration = 0.1f;

        private float startTime;
        private bool isTracking = true;

        //EnemyFire에서 공격 목표로 사용할 최종 고정 위치
        [HideInInspector]
        public Vector3 fixedEndPosition;        // 추적 후 고정될 라인의 끝점 위치

        // 외부 스크립트에 경고가 끝났음을 알리는 이벤트
        [HideInInspector]
        public UnityEvent OnWarningComplete = new UnityEvent();

        #endregion

        #region Unity Event Method
        void Awake()
        {
            dangerLine = GetComponent<LineRenderer>();

            // 렌더러 초기 설정 (프리팹에서 설정 가능)
            dangerLine.startColor = new Color(1, 0, 0, 0.7f);
            dangerLine.endColor = new Color(1, 0, 0, 0.7f);
            dangerLine.positionCount = 2;
            dangerLine.enabled = false;

            // 프리팹에서 이 값을 설정하지 않으면 기본값을 사용하도록
            if (hitLayerMask.value == 0)
            {
                // 플레이어(Player), 벽(Wall) 레이어를 지정하도록 유도하거나 기본값을 설정
                Debug.LogWarning("Line Renderer의 hitLayerMask를 설정해 주세요.");
            }
        }

        public void InitializeAndStart(Transform startPoint, Transform target, float trackTime, float fixedTime)
        {
            startTransform = startPoint;
            targetTransform = target;
            trackingDuration = trackTime;
            fixedDuration = fixedTime;

            dangerLine.enabled = true;
            isTracking = true;
            startTime = Time.time;
        }

        void Update()
        {
            if (targetTransform == null || startTransform == null) return;

            // 1. 추적 단계
            if (isTracking)
            {
                UpdateRaycastLine();            // Raycast를 사용해 라인 위치 업데이트

                if (Time.time >= startTime + trackingDuration)
                {
                    StopTrackingAndFixLine();
                }
            }
            // 2. 고정 단계
            else
            {
                // 고정된 위치를 사용하여 라인 그리기 (FixedEndPosition은 StopTrackingAndFixLine에서 이미 설정됨)
                dangerLine.SetPosition(0, startTransform.position);
                dangerLine.SetPosition(1, fixedEndPosition);

                // 고정 시간이 끝나면 이벤트 호출 후 오브젝트 제거
                if (Time.time >= startTime + trackingDuration + fixedDuration)
                {
                    OnWarningComplete.Invoke();
                    Destroy(gameObject);
                }
            }
        }
        #endregion

        #region Custom Method
        /// <summary>
        /// Raycast를 사용하여 라인의 끝 위치를 계산하고 그림
        /// </summary>
        void UpdateRaycastLine()
        {
            Vector3 startPos = startTransform.position;
            Vector3 targetPos = targetTransform.position;
            Vector3 direction = (targetPos - startPos).normalized;

            RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance, hitLayerMask);

            Vector3 endPoint;

            if (hit.collider != null)
            {
                // Raycast가 무언가에 맞았다면, 맞은 지점을 끝점으로 설정
                endPoint = hit.point;
            }
            else
            {
                // 아무것도 맞지 않았다면, 플레이어 위치나 최대 거리까지 라인을 뻗음
                endPoint = startPos + direction * maxDistance;
            }

            // 라인 렌더러 위치 업데이트
            dangerLine.SetPosition(0, startPos);
            dangerLine.SetPosition(1, endPoint);
        }

        void StopTrackingAndFixLine()
        {
            isTracking = false;

            // 추적을 멈추는 순간, 현재 그려진 라인의 끝점을 고정합니다.
            // 이 위치가 바로 EnemyFire에서 공격 발사 지점으로 사용될 것입니다.
            fixedEndPosition = dangerLine.GetPosition(1);

            // 라인 색상 변경 (경고가 고정되었음을 시각적으로 알림)
            dangerLine.startColor = new Color(1, 1, 0, 0.9f);
            dangerLine.endColor = new Color(1, 1, 0, 0.9f);
        }

        #endregion
    }
}
