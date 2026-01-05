using UnityEngine;

namespace Choi
{
    /// <summary>
    /// 플레이어를 향해 공격하는 스크립트
    /// </summary>
    public class EnemyFire : MonoBehaviour
    {
        #region Variables
        //공격할 타겟 - 플레이어
        [Header("Target & Prefabs")]
        public GameObject targetPlayer;             // 인스펙터에서 플레이어 오브젝트 연결
        public GameObject dangerLinePrefab;         // 인스펙터에서 DangerLine 프리팹 연결
        public Transform firePoint;                 // 인스펙터에서 발사 위치 Transform 연결 (몬스터 위치)
        public GameObject firePrefab;               // (선택 사항) 발사할 실제 공격 프리팹

        [Header("Timing Settings")]
        public float fireTimer = 2f;                // 공격 사이의 간격 (경고선 발사 주기)
        public float trackingTime = 1f;           // 경고선이 플레이어를 추적하는 시간
        public float fixedTime = 1f;              // 고정된 후 실제 공격까지의 대기 시간

        private float countdown = 0f;

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            countdown = 0f;
            // 타겟이 없으면 에러 방지를 위해 종료
            if (targetPlayer == null)
            {
                Debug.LogError("targetPlayer가 EnemyFire 스크립트에 연결되지 않았습니다!");
                enabled = false;
            }
        }

        private void Update()
        {
            // 경고선 발사 타이머
            countdown += Time.deltaTime;

            if (countdown >= fireTimer)
            {
                DrawDangerLine();
                countdown = 0f;
            }
        }
        #endregion

        #region Custom Method
        void DrawDangerLine()
        {
            //경고선 프리팹을 생성(위치는 DangerLine 스크립트가 처리)
            GameObject effectGo = Instantiate(dangerLinePrefab, Vector3.zero, Quaternion.identity);
            DangerLine dangerLineComp = effectGo.GetComponent<DangerLine>();

            if (dangerLineComp != null)
            {
                //DangerLine 스크립트 초기화 및 추적 시작
                dangerLineComp.InitializeAndStart(
                    firePoint,                   // 시작점 Transform
                    targetPlayer.transform,      // 타겟 Transform
                    trackingTime,                // 추적 시간
                    fixedTime                    // 고정 유지 시간
                );

                // 경고선 완료 이벤트에 공격 실행 메서드 연결
                // DangerLine이 추적/고정 단계를 마치고 파괴되기 직전에 이 메서드가 호출
                dangerLineComp.OnWarningComplete.AddListener(() => ExecuteAttack(dangerLineComp.fixedEndPosition));
            }
        }

        /// <summary>
        /// 경고선이 고정한 위치를 목표로 실제 공격을 실행합니다.
        /// (이 메서드는 DangerLine의 OnWarningComplete 이벤트에 의해 호출됩니다.)
        /// </summary>
        /// <param name="targetPosition">경고선이 Raycast를 통해 최종 고정한 위치</param>
        void ExecuteAttack(Vector3 targetPosition)
        {
            //여기에 실제 공격 발사 로직을 구현

            // 1. 공격 방향 계산
            Vector3 direction = (targetPosition - firePoint.position).normalized;

            // 2. 발사 로직 예시 (firePrefab이 연결되어 있다고 가정)
            if (firePrefab != null)
            {
                // 발사체의 회전은 방향 벡터를 바라보도록 설정
                Quaternion rotation = Quaternion.LookRotation(direction);
                // 2D 게임이라면 Z축 회전만 필요할 수 있습니다. (direction을 이용해 각도 계산)

                // 2D 환경에서 Z축 회전 계산 예시 (선택적)
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation2D = Quaternion.Euler(0, 0, angle);

                GameObject projectile = Instantiate(firePrefab, firePoint.position, rotation2D);

                // 발사체 스크립트에 방향을 전달하여 앞으로 나아가게 할 수 있습니다.
                // projectile.GetComponent<ProjectileScript>().Initialize(direction);
            }

            Debug.Log($"공격 실행! 고정된 최종 목표 위치: {targetPosition}");
        }
        #endregion
    }
}