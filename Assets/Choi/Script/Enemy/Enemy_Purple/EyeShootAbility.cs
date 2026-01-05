using UnityEngine;

namespace Choi
{
    public class EyeShootAbility : MonoBehaviour, IEnemyAbility
    {
        private Enemy_Main owner;
        private GameManager gameManager;

        [SerializeField] private GameObject eyeVisual;

        [Header("Speeds")]
        [SerializeField] private float chaseSpeed = 6f;
        [SerializeField] private float dashSpeed = 18f;
        [SerializeField] private float returnSpeed = 8f;
        private const float Y_OFFSET = 2.7f;

        [Header("Attack Interval")]
        [SerializeField] private float patternInterval = 7f;
        private float patternTimer = 0f;

        [Header("Dash Settings")]
        [SerializeField] private float dashDistance = 12f;

        private bool initialized = false;
        private bool active = false;     // Enemy_Main이 ability를 켠 상태인지

        private enum State
        {
            ChasePlayer,
            DashForward,
            ReturnToPlayer
        }

        private State currentState = State.ChasePlayer;

        private Vector3 dashTarget;
        private int facingDir = 1;

        public void Setup(Enemy_Main enemy)
        {
            owner = enemy;
            gameManager = FindAnyObjectByType<GameManager>();
            initialized = true;
        }

        public void OnEnter()
        {
            if (!initialized) return;

            active = true;         // 활성화됨
            owner.overrideMovement = true;

            currentState = State.ChasePlayer;
            patternTimer = 0f;

            eyeVisual?.SetActive(true);
        }

        public void OnExit()
        {
            active = false;        // 비활성화됨
            owner.overrideMovement = false;

            eyeVisual?.SetActive(false);
        }

        // Enemy_Main이 불러주는 Tick → 사용은 하되 강제 틱도 별도로 돌림
        public void OnTick()
        {
            UpdateAbilityLogic();
        }

        private void UpdateAbilityLogic()
        {
            if (!initialized) return;
            if (!active) return;
            if (owner.player == null) return;

            UpdateFacingDirection();

            switch (currentState)
            {
                case State.ChasePlayer:
                    Tick_Chase();
                    break;

                case State.DashForward:
                    Tick_Dash();
                    break;

                case State.ReturnToPlayer:
                    Tick_Return();
                    break;
            }
        }


        private void Update()
        {
            if (!initialized) return;
            if (!active) return;                     // Enemy_Main에서 ability 켜진 경우에만 동작
            if (owner.player == null) return;

            UpdateFacingDirection();

            switch (currentState)
            {
                case State.ChasePlayer:
                    Tick_Chase();
                    break;

                case State.DashForward:
                    Tick_Dash();
                    break;

                case State.ReturnToPlayer:
                    Tick_Return();
                    break;
            }
        }

        private void Tick_Chase()
        {
            patternTimer += Time.deltaTime;

            // 플레이어 위 3 지점으로 추적
            Vector3 target = owner.player.position + new Vector3(0, Y_OFFSET, 0);
            MoveTowards(target, chaseSpeed);

            if (patternTimer >= patternInterval)
            {
                patternTimer = 0f;

                dashTarget = transform.position + new Vector3(dashDistance * facingDir, 0, 0);
                currentState = State.DashForward;
            }
        }

        private void Tick_Dash()
        {
            MoveTowards(dashTarget, dashSpeed);

            if (Vector2.Distance(transform.position, dashTarget) < 0.3f)
            {
                currentState = State.ReturnToPlayer;
            }
        }

        private void Tick_Return()
        {
            // 플레이어 위 3 지점으로 복귀
            Vector3 target = owner.player.position + new Vector3(0, Y_OFFSET, 0);
            MoveTowards(target, returnSpeed);

            if (Vector2.Distance(transform.position, target) < 1f)
            {
                currentState = State.ChasePlayer;
            }
        }

        private void MoveTowards(Vector3 target, float speed)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
        }

        private void UpdateFacingDirection()
        {
            facingDir = (owner.player.position.x > transform.position.x) ? 1 : -1;

            if (eyeVisual != null)
                eyeVisual.transform.localScale = new Vector3(facingDir, 1, 1);
        }
    }
}
