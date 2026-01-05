using Choi;
using System;
using UnityEngine;

namespace Team3
{
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        [Header("점프 설정")]
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private int maxJumpCount = 2;
        private int jumpCount;

        [Header("이동 설정")]
        [SerializeField] private float moveSpeed = 5f;
        private float moveInput;

        [Header("Wall Check")]
        [SerializeField] private GameObject wallCheckLeft;
        [SerializeField] private GameObject wallCheckRight;

        private Rigidbody2D rb2D;
        private AudioSource audioSource;

        [SerializeField] private bool isFrontBlocked;
        [SerializeField] private bool isGrounded;
        private bool isDead = false;
        private bool jumpPressed;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private CutsceneManager cutsceneManager;
        [SerializeField] private int moveDirection = 1;

        public event Action<DeathCause> OnPlayerDied;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();

            rb2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }

        private void Update()
        {
            if (gameManager.State != GameState.Playing)
                return;

            // 좌우 이동 입력 (A / D)
            moveInput = Input.GetAxisRaw("Horizontal");

            // 점프 입력 (W / Space / 마우스 클릭)
            if (Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetMouseButtonDown(0))
            {
                jumpPressed = true;
            }
        }

        private void FixedUpdate()
        {
            if (gameManager.State != GameState.Playing)
                return;

            MoveHorizontal();

            if (jumpPressed)
            {
                TryJump();
                jumpPressed = false;
            }
        }
        #endregion

        #region Custom Method
        private void TryJump()
        {
            if (jumpCount >= maxJumpCount)
                return;

            jumpCount++;
            isGrounded = false;

            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0f);
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void MoveHorizontal()
        {
            rb2D.linearVelocity = new Vector2(moveInput * moveSpeed, rb2D.linearVelocity.y);

            // 이동 방향 갱신
            if (moveInput > 0)
                moveDirection = 1;
            else if (moveInput < 0)
                moveDirection = -1;
        }

        public void ReverseDirection()
        {
            moveDirection *= -1;
            UpdateWallCheck();
            RecheckFrontBlocked();
        }

        private void UpdateWallCheck()
        {
            if (moveDirection > 0)
            {
                wallCheckRight.SetActive(true);
                wallCheckLeft.SetActive(false);
            }
            else
            {
                wallCheckRight.SetActive(false);
                wallCheckLeft.SetActive(true);
            }
        }

        private void RecheckFrontBlocked()
        {
            var activeCheck = moveDirection > 0 ? wallCheckRight : wallCheckLeft;
            var hit = Physics2D.OverlapCircle(
                activeCheck.transform.position,
                0.1f,
                LayerMask.GetMask("Wall")
            );

            SetFrontBlocked(hit != null);
        }

        public void Die(DeathCause cause)
        {
            if (isDead)
                return;

            isDead = true;
            CutsceneManager.Instance.PlayDeathCutscene(cause);
        }

        public void SetGrounded(bool grounded)
        {
            if (grounded && rb2D.linearVelocity.y <= 0.01f)
            {
                isGrounded = true;
                jumpCount = 0;
            }
            else if (!grounded)
            {
                isGrounded = false;
            }
        }

        public void SetFrontBlocked(bool blocked)
        {
            isFrontBlocked = blocked;
        }
        #endregion
    }
}
