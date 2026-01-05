using UnityEngine;

namespace JS
{
    
    public class Enemy : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float speed = 3f;
        [SerializeField] private float maxScale = 4f;

        private Transform player;

        [SerializeField] private float floatAmplitude = 0.3f;
        [SerializeField] private float floatFrequency = 3f;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private CutsceneManager cutsceneManager;

        private EnemyMoveState moveState = EnemyMoveState.Chasing;
        private Transform waypointTarget;

        [Header("보스 원래 상태")]
        private float baseSpeed;
        private Vector3 baseScale;
        private EnemyBuffType currentBuff = EnemyBuffType.None;

        [Header("Laser Beam")]
        [SerializeField] private LaserBeam laserBeam;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float laserDuration = 3f;

        private float laserTimer;
        private bool isFiringLaser;

        [Header("Punch Settings")]
        public GameObject punchPrefab;     // 떨어뜨릴 펀치 프리팹
        public Transform punchPoint;       // 보스의 자식 또는 별도 오브젝트
        [SerializeField] private float punchFollowSmoothness = 10f;
        [SerializeField] private float minWaitTime = 1.5f;
        [SerializeField] private float maxWaitTime = 3f;
        [SerializeField] private float punchHeight = 6f; // 펀치가 생성될 높이

        private float punchTimer;
        private bool isPunchingMode = true; // 현재 펀치 버프 중인지
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //초기화
            baseSpeed = speed;
            baseScale = transform.localScale;

            // 테스트용 치트키
            //ApplyBuff(EnemyBuffType.Punch, 0);
        }
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            //펀치 랜덤 타이머 세팅
            SetRandomPunchTimer();
        }

        private void Update()
        {
            if (gameManager.State != GameState.Playing)
                return;

            switch (moveState)
            {
                case EnemyMoveState.MovingToWaypoint:
                    MoveToWaypoint();
                    break;

                case EnemyMoveState.Chasing:                    
                    FollowPlayerGhostStyle();
                    CatchUpIfTooFar();
                    break;

            }

            //펀치 타이머 처리
            if (isPunchingMode)
            {
                UpdatePunchPointPosition();
                HandlePunchCycle();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 1. 충돌한 오브젝트에서 Player 컴포넌트 가져오기
            Player playerComponent = other.GetComponent<Player>();

            if (playerComponent == null)
                return;

            // 2. Player 인스턴스에서 Die 메서드 호출 및 DeathCause 전달
            // 적(Enemy)에 의한 사망이므로 DeathCause.EnemyA를 사용합니다.
            playerComponent.Die(DeathCause.EnemyA); // <--- 이 부분이 핵심!

            Debug.Log("Enemy caught the Player!");

            // (선택 사항: Enemy도 파괴 또는 비활성화 처리)
            gameObject.SetActive(false);
        }
        #endregion

        #region Custom Method
        private void CatchUpIfTooFar()
        {
            if (player == null)
                return;

            if (player.position.x - transform.position.x > 10f)
            {
                transform.position = new Vector3(
                    player.position.x - 8f,
                    transform.position.y,
                    transform.position.z
                );
            }
        }

        private void FollowPlayerGhostStyle()
        {
            if (player == null)
                return;

            float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

            float yTarget = Mathf.Lerp(
                transform.position.y,
                player.position.y,
                0.15f
            ) + offsetY;

            float xTarget = Mathf.Lerp(
                transform.position.x,
                player.position.x,
                speed * Time.deltaTime
            );

            Vector3 target = new Vector3(xTarget, yTarget, transform.position.z);

            transform.position = Vector3.Lerp(
             transform.position,
             target,
             speed * Time.deltaTime
            );
        }
        public void ApplyBuff(EnemyBuffType type, float value)
        {
            if (currentBuff == type)
                return;

            RemoveCurrentBuff();

            switch (type)
            {
                case EnemyBuffType.SpeedUp:
                    speed *= value;
                    break;

                case EnemyBuffType.ScaleUp:
                    IncreaseScale(value);
                    break;

                case EnemyBuffType.LaserBeam:
                    StartLaser();
                    break;
                /*case EnemyBuffType.Punch:
                    StartPunch();
                    break;*/
            }

            currentBuff = type;
        }
        private void RemoveCurrentBuff()
        {
            switch (currentBuff)
            {
                case EnemyBuffType.SpeedUp:
                    speed = baseSpeed;
                    break;

                case EnemyBuffType.ScaleUp:
                    transform.localScale = baseScale;
                    break;

                case EnemyBuffType.LaserBeam:
                    StopLaser();
                    break;
                /*case EnemyBuffType.Punch:
                    StopPunch();
                    break;*/
            }

            currentBuff = EnemyBuffType.None;
        }
        public void IncreaseScale(float scaleMultiplier)
        {
            Vector3 newScale = transform.localScale * scaleMultiplier;

            if (newScale.x > maxScale)
                newScale = Vector3.one * maxScale;

            transform.localScale = newScale;
        }
     
        private void HandleLaserDuration()
        {
            laserTimer += Time.deltaTime;

            if (laserTimer >= laserDuration)
            {
                StopLaser();
            }
        }
        private void StartLaser()
        {
            if (laserBeam == null || firePoint == null)
                return;

            isFiringLaser = true;
            laserTimer = 0f;

            laserBeam.transform.position = firePoint.position;
            laserBeam.transform.rotation = firePoint.rotation;
            laserBeam.gameObject.SetActive(true);
        }
        
        private void StopLaser()
        {
            isFiringLaser = false;
            laserBeam.gameObject.SetActive(false);
        }
        public void GoToWaypoint(Transform waypoint)
        {
            if (waypoint == null)
                return;

            waypointTarget = waypoint;
            moveState = EnemyMoveState.MovingToWaypoint;
        }
        private void MoveToWaypoint()
        {
            if (waypointTarget == null)
            {
                moveState = EnemyMoveState.Chasing;
                return;
            }

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(
                transform.position,
                waypointTarget.position,
                step
            );

            float distance = Vector3.Distance(transform.position, waypointTarget.position);
            if (distance < 0.1f)
            {
                waypointTarget = null;
                moveState = EnemyMoveState.Chasing;
            }
        }

        //펀치 발사
        private void StartPunch()
        {
            isPunchingMode = true;
            SetRandomPunchTimer(); // 첫 타이머 설정

        }

        private void StopPunch()
        {
            isPunchingMode = false;
            punchPoint.gameObject.SetActive(false);
        }

        // 매 프레임 타이머 계산 및 발사
        private void UpdatePunchPointPosition()
        {
            if (player == null || punchPoint == null) return;

            // PunchPoint는 Enemy의 위치와 상관없이 플레이어의 X값만 부드럽게 추적
            float targetX = Mathf.Lerp(punchPoint.position.x, player.position.x, Time.deltaTime * punchFollowSmoothness);

            // Y값은 맵의 상단(플레이어 위쪽 일정 높이)에 고정
            punchPoint.position = new Vector3(targetX, punchHeight, 0f);
        }

        private void HandlePunchCycle()
        {
            punchTimer -= Time.deltaTime;
            if (punchTimer <= 0f)
            {
                ExecutePunch();
                SetRandomPunchTimer();
            }
        }

        private void SetRandomPunchTimer() => punchTimer = Random.Range(minWaitTime, maxWaitTime);

        private void ExecutePunch()
        {
            if (punchPrefab != null && punchPoint != null)
            {
                // PunchPoint의 현재 위치에서 펀치 생성
                Instantiate(punchPrefab, punchPoint.position, Quaternion.identity);
            }
        }
        #endregion
    }
}
