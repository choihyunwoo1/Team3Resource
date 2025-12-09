using UnityEngine;
using UnityEngine.InputSystem;

namespace My2DGame
{
    /// <summary>
    /// 플레이어를 제어하는 클래스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        //참조
        private Rigidbody2D rb2D;
        private Animator animator;
        private TouchingDirection touchingDirections;
        private Damageable damageable;
        //잔상 효과
        private TrailEffect trailEffect;

        //이동 속도 - 걷는 속도
        [SerializeField]private float walkSpeed = 3f;
        //이동속도 - 뛰는 속도
        [SerializeField]private float runSpeed = 6f;
        //점프
        [SerializeField] private float jumpForce = 5f;
        //점프했을 때 스피드
        [SerializeField] private float airSpeed = 2f;

        //이동 입력값
        private Vector2 inputMove = Vector2.zero;

        //걷기 애니메이션
        private bool isMove = false;
        //뛰기 애니메이션
        private bool isRun = false;
        //반전
        private bool isFacingRight = true;

        //이단 점프
        private bool canDoubleJump = false;

        #endregion

        #region Property
        
        public bool IsFacingRight
        {
            get { return isFacingRight; }
            private set
            {
                //반전 구현
                if(IsFacingRight != value)
                {
                    this.transform.localScale *= new Vector2(-1, 1);
                }

                isFacingRight = value;
            }
        }

        public bool IsMove
        {
            get { return isMove; }
            private set 
            {  
                isMove = value;
                animator.SetBool(AnimationString.IsMove, value);
            }
        }

        public bool IsRun
        {
            get { return isRun; }
            private set
            {
                isRun = value;
                animator.SetBool(AnimationString.IsRun, value);
            }
        }

        //현재 이동 속도 - 읽기 전용
        public float CurrentMoveSpeed
        {
            get
            {
                if(CannotMove)          //애니메이터 파라미터 값 읽어오기
                {
                    return 0f;
                }

                if(IsMove && touchingDirections.IsWall == false)    //이동 가능
                {
                    if(touchingDirections.IsGround)            //땅에 있을 때
                    {
                        if(IsRun)
                    {
                            return runSpeed;
                        }
                    else
                        {
                            return walkSpeed;
                        }
                    }
                    else
                    {
                        return airSpeed;
                    }
                }
                else    //이동 불가
                {
                    return 0f;
                }
            }
        }

        //애니메이터의 파라미터값(CannotMove) 읽어오기
        public bool CannotMove
        {
            get
            {
                return animator.GetBool(AnimationString.CannotMove);
            }
        }

        //애니메이터의 파라미터값(LockVelocity) 읽어오기
        public bool LockVelocity
        {
            get
            {
                return animator.GetBool(AnimationString.LockVelocity);
            }
        }

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            rb2D = this.GetComponent<Rigidbody2D>();            
            animator = this.GetComponent<Animator>();
            touchingDirections = this.GetComponent<TouchingDirection>();
            damageable = this.GetComponent<Damageable>();
            trailEffect = this.GetComponent<TrailEffect>();

            //이벤트 함수 등록
            damageable.hitAction += OnHit;

        }

        private void FixedUpdate()
        {

            //좌우이동
            if(LockVelocity == false)
            {
                rb2D.linearVelocity = new Vector2(inputMove.x * CurrentMoveSpeed, rb2D.linearVelocity.y);
                //rb2D.linearVelocity = new Vector2(inputMove.x * walkSpeed, inputMove.y * walkSpeed);          //좌우위아래 이동
            }

            //점프 애니메이션
            animator.SetFloat(AnimationString.YVelocity, rb2D.linearVelocityY);


        }

        #endregion

        #region Custom method
        //반향 전환
        void SetFacingDirection(Vector2 moveInput)
        {
            if(moveInput.x > 0f && IsFacingRight == false)            //오른쪽으로 이동
            {
                IsFacingRight = true;
            }
            else if (moveInput.x < 0f && IsFacingRight == true)      //왼쪽으로 이동
            {
                IsFacingRight = false;
            }
        }

        //이동 입력 처리
        public void PlayerMove(InputAction.CallbackContext context)
        {
            inputMove = context.ReadValue<Vector2>();
            //Debug.Log(inputMove);

            if(damageable.IsDeath == false)
            {
                IsMove = (inputMove != Vector2.zero);

                //방향 전환
                SetFacingDirection(inputMove);
            }
            else
            {
                IsMove = false;
            }
        }

        //런 입력 처리
        public void PlayerRun(InputAction.CallbackContext context)
        {
            if(context.started)     //버튼을 눌렀을 때 (누르기 시작했을 때)
            {
                IsRun = true;

                //잔상효과 시작
                if (trailEffect != null)
                {
                    trailEffect.StartTrailEffect();
                }

            }
            else if(context.canceled)   //버튼을 뗄 때
            {
                IsRun = false;                
            }
        }

        //점프 입력 처리
        public void PlayerJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (touchingDirections.IsGround && canDoubleJump == false)
                {
                    animator.SetTrigger(AnimationString.JumpTrigger);
                    rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);

                    //잔상효과 시작
                    if(trailEffect != null)
                    {
                        trailEffect.StartTrailEffect();
                    }

                    canDoubleJump = true;

                }
                else if (canDoubleJump)
                {
                    animator.SetTrigger(AnimationString.JumpTrigger);
                    rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);

                    //잔상효과 시작
                    if (trailEffect != null)
                    {
                        trailEffect.StartTrailEffect();
                    }

                    canDoubleJump = false;
                }
            }
        }

        //공격1 입력 처리
        public void PlayerAttack(InputAction.CallbackContext context)
        {
            if(context.started && touchingDirections.IsGround)
            {
                animator.SetTrigger(AnimationString.AttackTrigger);

            }
        }

        //화살 공격 입력 처리
        public void PlayerBowAttack(InputAction.CallbackContext context)
        {
            if (context.started && touchingDirections.IsGround)
            {
                animator.SetTrigger(AnimationString.BowAttackTrigger);

            }
        }

        //데미지 이벤트에 등록되는 함수
        public void OnHit(float damage, Vector2 knokback)
        {
            rb2D.linearVelocity = new Vector2(knokback.x, rb2D.linearVelocityY + knokback.y);
        }


        #endregion
    }
}
