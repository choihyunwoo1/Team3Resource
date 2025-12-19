using System;
using UnityEngine;

namespace Choi
{
    public class Player : MonoBehaviour
    {
        #region Variables
        [Header("점프 설정")]
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private int maxJumpCount = 2;
        private int jumpCount;

        [Header("이동 설정")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Wall Check")]
        [SerializeField] private GameObject wallCheckLeft;
        [SerializeField] private GameObject wallCheckRight;

        private Rigidbody2D rb2D;
        private AudioSource audioSource;

        [SerializeField] private bool isFrontBlocked;
        [SerializeField] private bool isGrounded;
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

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                jumpPressed = true;
        }

        private void FixedUpdate()
        {
            if (gameManager.State != GameState.Playing)
                return;

            MoveForward();

            if (jumpPressed)
            {
                TryJump();
                jumpPressed = false;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Obstacle"))
                return;

            Die(DeathCause.Trap);
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

        private void MoveForward()
        {
            if (!isGrounded && isFrontBlocked)
            {
                rb2D.linearVelocity = new Vector2(0f, rb2D.linearVelocity.y);
                return;
            }

            rb2D.linearVelocity = new Vector2(moveSpeed * moveDirection, rb2D.linearVelocity.y);
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
            var hit = Physics2D.OverlapCircle(activeCheck.transform.position, 0.1f, LayerMask.GetMask("Wall"));

            SetFrontBlocked(hit != null);
        }

        public void Die(DeathCause cause)
        {
            if (gameManager.CurrentState != GameState.Playing)
                return;

            rb2D.linearVelocity = Vector2.zero;
            rb2D.bodyType = RigidbodyType2D.Kinematic;

            // ------------- 핵심 추가 --------------
            OnPlayerDied?.Invoke(cause);
            // -------------------------------------

            gameManager.RequestGameOver(cause);
            enabled = false;
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
