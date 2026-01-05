using UnityEngine;
using System.Collections.Generic;
using Choi;

namespace JS
{
    public enum EnemyBuffType
    {
        None,
        SpeedUp,
        ScaleUp,
        LaserBeam,
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
        // 여기에 새로운 상태 추가
    }

    public enum EnemyMoveState
    {
        Chasing,
        MovingToWaypoint
    }

    public class Enemy_Main : MonoBehaviour
    {
        #region Variables - 공통 설정
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject defaultVisual; // 기본 스프라이트 오브젝트
        public Transform player;

        [Header("Movement Settings")]
        [SerializeField] public float speed = 3f;
        [SerializeField] private float floatAmplitude = 0.3f;
        [SerializeField] private float floatFrequency = 3f;

        private float baseSpeed;
        private Vector3 baseScale;
        private EnemyMoveState moveState = EnemyMoveState.Chasing;
        private Transform waypointTarget;
        #endregion

        #region Ability Management - 인터페이스 관리
        private IEnemyAbility currentAbility;
        private EnemyBuffType currentBuff = EnemyBuffType.None;

        // 타입별 능력을 빠르게 찾기 위한 딕셔너리
        private Dictionary<EnemyBuffType, IEnemyAbility> abilityMap = new Dictionary<EnemyBuffType, IEnemyAbility>();
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            // 초기 상태 저장
            baseSpeed = speed;
            baseScale = transform.localScale;

            // 본체에 붙어있는 모든 IEnemyAbility 구현체들을 찾아 딕셔너리에 등록
            // (PunchAbility, LaserAbility 등이 이 오브젝트에 같이 붙어있어야 함)
            IEnemyAbility[] abilities = GetComponents<IEnemyAbility>();
            foreach (var ability in abilities)
            {
                ability.Setup(this);

                // 클래스 이름을 기준으로 매핑하거나, 각 클래스에 Type 프로퍼티를 두어 매핑 가능
                if (ability is PunchAbility) abilityMap[EnemyBuffType.Red] = ability;
                else if (ability is SlimeAbility) abilityMap[EnemyBuffType.Blue] = ability;
                else if (ability is LaughAbility) abilityMap[EnemyBuffType.Green] = ability;
                else if (ability is CloneAbility) abilityMap[EnemyBuffType.Yellow] = ability;
                else if (ability is EyeShootAbility) abilityMap[EnemyBuffType.Purple] = ability;
                //else if (ability is LaserAbility) abilityMap[EnemyBuffType.LaserBeam] = ability;
                // 새로운 능력이 추가될 때마다 여기에 등록 로직 추가
            }
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            // 테스트용: 처음 시작 시 펀치 버프 적용
            //ApplyBuff(EnemyBuffType.Red, 0);
        }

        private void Update()
        {
            if (gameManager.State != GameState.Playing) return;

            // 1. 기본 이동 시스템 (어떤 버프든 공통으로 작동)
            HandleBaseMovement();

            // 2. 능력 시스템 (현재 장착된 능력의 로직 실행)
            currentAbility?.OnTick();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player playerComponent = other.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.Die(DeathCause.EnemyA);
                gameObject.SetActive(false);
            }
        }
        #endregion

        #region Public Methods - 상태 제어
        public void ApplyBuff(EnemyBuffType type, float value)
        {
            if (currentBuff == type) return;

            // 1. 기존 능력 종료 (Clean Up)
            if (currentAbility != null)
            {
                currentAbility.OnExit();
            }
            else
            {
                // 이전 능력이 없었다면 기본 외형을 꺼줌
                defaultVisual.SetActive(false);
            }

            // 특수 처리: SpeedUp이나 ScaleUp처럼 외형 교체가 아닌 수치 변화인 경우
            HandleStatBuffs(type, value);

            // 2. 새 능력 활성화
            if (abilityMap.TryGetValue(type, out IEnemyAbility newAbility))
            {
                currentAbility = newAbility;
                currentBuff = type;
                currentAbility.OnEnter();
            }
            else
            {
                // 적용할 능력이 None이거나 없는 경우 기본 상태로 복구
                currentAbility = null;
                currentBuff = EnemyBuffType.None;
                defaultVisual.SetActive(true);
            }
        }

        // 속도나 크기 같은 단순 수치 버프 처리
        public void HandleStatBuffs(EnemyBuffType type, float value)
        {
            // 리셋
            speed = baseSpeed;
            transform.localScale = baseScale;

            if (type == EnemyBuffType.SpeedUp) speed *= value;
            if (type == EnemyBuffType.ScaleUp) transform.localScale = baseScale * value;
        }

        public void GoToWaypoint(Transform waypoint)
        {
            if (waypoint == null) return;
            waypointTarget = waypoint;
            moveState = EnemyMoveState.MovingToWaypoint;
        }
        #endregion

        #region Private Methods - 이동 로직
        private void HandleBaseMovement()
        {
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
        }

        private void FollowPlayerGhostStyle()
        {
            if (player == null) return;

            float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            float yTarget = Mathf.Lerp(transform.position.y, player.position.y, 0.15f) + offsetY;
            float xTarget = Mathf.Lerp(transform.position.x, player.position.x, speed * Time.deltaTime);
            //float xTarget = player.position.x;

            Vector3 target = new Vector3(xTarget, yTarget, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);

        }

        private void MoveToWaypoint()
        {
            if (waypointTarget == null)
            {
                moveState = EnemyMoveState.Chasing;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, waypointTarget.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, waypointTarget.position) < 0.1f)
            {
                waypointTarget = null;
                moveState = EnemyMoveState.Chasing;
            }
        }

        private void CatchUpIfTooFar()
        {
            if (player == null) return;
            if (player.position.x - transform.position.x > 10f)
            {
                transform.position = new Vector3(player.position.x - 8f, transform.position.y, transform.position.z);
            }
        }
        #endregion
    }
}