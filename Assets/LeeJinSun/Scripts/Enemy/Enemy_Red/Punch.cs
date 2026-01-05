using UnityEngine;

namespace JS
{
    /// <summary>
    /// 하늘에서 펀치 공격하는 클래스
    /// </summary>
    public class Punch : MonoBehaviour
    {
        #region Variables
        //참조
        private Rigidbody2D rb2D;
        private Animator animator;
        //접촉하는 충돌체
        private PolygonCollider2D punchCollider;

        //하강 속도
        [SerializeField] private float fallSpeed = 15f;
        //
        [SerializeField] private float destroyTime = 4f;

        [SerializeField] private bool isGround = false;
        private bool hasHitGround = false; // 바닥에 한 번이라도 닿았는지 체크

        //접촉면 범위
        [SerializeField] private float groundDistance = 0.05f;

        //접촉 조건
        [SerializeField]
        private ContactFilter2D contactFilter;

        //캐스트 결과
        private RaycastHit2D[] groundHits = new RaycastHit2D[5];
        #endregion

        #region Property
        public bool IsGround
        {
            get { return isGround; }
            set
            {
                isGround = value;
                animator.SetBool("IsGround", value);
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            rb2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            punchCollider = GetComponent<PolygonCollider2D>();

            // 중력 없이 일정한 속도로 떨어지게 설정
            rb2D.gravityScale = 0;
        }

        private void Start()
        {
            // 생성되자마자 아래로 발사
            rb2D.linearVelocity = Vector2.down * fallSpeed;

            // 화면 밖으로 나갈 때를 대비해 자동 삭제
            Destroy(gameObject, destroyTime);
        }

        private void FixedUpdate()
        {
            // 이미 바닥에 닿았다면 아래 로직을 실행하지 않음 (최적화 및 버그 방지)
            if (hasHitGround) return;

            IsGround = (punchCollider.Cast(Vector2.down, contactFilter, groundHits, groundDistance) > 0);

            if (IsGround)
            {
                OnHitGround();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Die(DeathCause.EnemyA);
            }
            Destroy(gameObject, 1f); // 충돌 후 소멸
        }

        #endregion

        #region Custom Method
        private void OnHitGround()
        {
            hasHitGround = true; // 중복 실행 방지

            // 1. 속도를 즉시 0으로 만들어 멈춤
            rb2D.linearVelocity = Vector2.zero;

            // 2. 물리 연산 자체를 멈추기 위해 BodyType을 Kinematic으로 변경 (선택 사항이지만 추천)
            rb2D.bodyType = RigidbodyType2D.Kinematic;

            // 3. 애니메이션 파라미터 전달
            animator.SetBool("IsGround", true);

            // 4. 바닥에 닿은 후 일정 시간 뒤에 삭제 (애니메이션 재생 시간을 고려)
            // 기존 Start의 Destroy를 취소하고 새로 설정하거나, 유지합니다.
            CancelInvoke(); // 기존 파괴 예약 취소 (선택)
            Destroy(gameObject, 0.6f);
        }
        #endregion
    }
}
