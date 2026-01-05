using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Choi
{
    public enum EnemyBuffType
    {
        None,
        SpeedUp,
        ScaleUp,
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
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
        [SerializeField] private GameObject defaultVisual;
        public Transform player;

        [Header("Movement Settings")]
        [SerializeField] public float speed = 3f;
        [SerializeField] private float floatAmplitude = 0.3f;
        [SerializeField] private float floatFrequency = 3f;

        private float baseSpeed;
        private Vector3 baseScale;
        private EnemyMoveState moveState = EnemyMoveState.Chasing;
        private Transform waypointTarget;

        // 추가: 정지 상태 플래그
        private bool isFrozen = false;
        public bool overrideMovement = false;
        #endregion

        #region Ability Management
        private IEnemyAbility currentAbility;
        private EnemyBuffType currentBuff = EnemyBuffType.None;
        private Dictionary<EnemyBuffType, IEnemyAbility> abilityMap = new Dictionary<EnemyBuffType, IEnemyAbility>();
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            baseSpeed = speed;
            baseScale = transform.localScale;

            IEnemyAbility[] abilities = GetComponents<IEnemyAbility>();
            foreach (var ability in abilities)
            {
                ability.Setup(this);

                if (ability is PunchAbility) abilityMap[EnemyBuffType.Red] = ability;
                else if (ability is SlimeAbility) abilityMap[EnemyBuffType.Blue] = ability;
                else if (ability is LaughAbility) abilityMap[EnemyBuffType.Green] = ability;
                else if (ability is CloneAbility) abilityMap[EnemyBuffType.Yellow] = ability;
                else if (ability is EyeShootAbility) abilityMap[EnemyBuffType.Purple] = ability;
            }
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            // 핵심: 정지 상태면 아무 것도 하지 않음
            if (isFrozen) return;
            if (gameManager.State != GameState.Playing) return;

            if (!overrideMovement)
            {
                HandleBaseMovement();
            }

            HandleBaseMovement();
            currentAbility?.OnTick();

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player playerComponent = other.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.Die(DeathCause.Enemy);
                gameObject.SetActive(false);
            }
        }
        #endregion

        #region Public Methods - Freeze 제어
        public void Freeze(float duration)
        {
            StartCoroutine(FreezeRoutine(duration));
        }

        private IEnumerator FreezeRoutine(float duration)
        {
            isFrozen = true;
            yield return new WaitForSeconds(duration);
            isFrozen = false;
        }
        #endregion

        #region Public Methods - 상태 제어
        public void ApplyBuff(EnemyBuffType type, float value)
        {
            if (currentBuff == type) return;

            if (currentAbility != null)
            {
                currentAbility.OnExit();
            }
            else
            {
                defaultVisual.SetActive(false);
            }

            HandleStatBuffs(type, value);

            if (abilityMap.TryGetValue(type, out IEnemyAbility newAbility))
            {
                currentAbility = newAbility;
                currentBuff = type;
                currentAbility.OnEnter();
            }
            else
            {
                currentAbility = null;
                currentBuff = EnemyBuffType.None;
                defaultVisual.SetActive(true);
            }
        }

        public void HandleStatBuffs(EnemyBuffType type, float value)
        {
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

            transform.position = Vector3.MoveTowards(
                transform.position,
                waypointTarget.position,
                speed * Time.deltaTime
            );

            // 도착 체크
            if (Vector3.Distance(transform.position, waypointTarget.position) < 0.1f)
            {
                // 도착 순간 뒤집기
                Flip();

                // 다음 동작을 위해 초기화
                waypointTarget = null;
                moveState = EnemyMoveState.Chasing;
            }
        }

        private void CatchUpIfTooFar()
        {
            if (player == null) return;

            if (player.position.x - transform.position.x > 10f)
            {
                transform.position = new Vector3(
                    player.position.x - 8f,
                    transform.position.y,
                    transform.position.z
                );
            }
        }
        public void Flip()
        {
            float newY = transform.eulerAngles.y == 0 ? 180 : 0;
            transform.eulerAngles = new Vector3(0, newY, 0);
        }
        #endregion
    }
}
