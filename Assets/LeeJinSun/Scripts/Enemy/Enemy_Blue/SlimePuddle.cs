using UnityEngine;
using System.Collections;

namespace JS
{
    public class SlimePuddle : MonoBehaviour
    {
        #region Variables
        [Header("Visual")]
        [SerializeField] private float flowSpeed = 0.2f;      // 아래로 흐르는 속도
        [SerializeField] private float fadeSpeed = 0.5f;      // 투명해지는 속도
        [SerializeField] private float activeDuration = 3f;   // 감속이 유지되는 시간
        [Header("PlayerSpeed")]
        [SerializeField] private float slowMultiplier = 0.3f; // 원래 속도의 30%로 감소
        [SerializeField] private float sinkingVelocity = -1.5f; // 아래로 가라앉는 속도
        private float originalSpeed; // 플레이어의 원래 속도를 저장할 변수

        private SpriteRenderer spriteRenderer;
        private Vector3 initialLocalPos;
        private Color initialColor;
        private bool isEffectActive = false; // 현재 감속 효과가 유효한지 체크

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            spriteRenderer = GetComponent<SpriteRenderer>();
            initialLocalPos = transform.localPosition;
            initialColor = spriteRenderer.color;
        }

        // 이너미가 이 오브젝트를 켤 때마다 호출됨
        private void OnEnable()
        {
            // 위치와 색상 초기화
            transform.localPosition = initialLocalPos;
            spriteRenderer.color = initialColor;
            isEffectActive = true;

            StopAllCoroutines();
            StartCoroutine(FlowAndFadeRoutine());
        }

        // 슬라임 위에 있는 동안 매 프레임 실행
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (isEffectActive && collision.CompareTag("Player"))
            {
                // 플레이어의 이동 스크립트를 가져옵니다. 
                // (스크립트 이름을 본인의 플레이어 이동 스크립트 이름으로 바꿔주세요)
                var playerMove = collision.GetComponent<Player>();
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

                if (playerMove != null && rb != null)
                {
                    // 플레이어의 속도 변수가 'speed'라고 가정했을 때
                    // 처음 들어왔을 때만 원래 속도를 저장해둡니다.
                    if (Mathf.Approximately(playerMove.moveSpeed, originalSpeed) == false && originalSpeed == 0)
                    {
                        originalSpeed = playerMove.moveSpeed;
                    }

                    // 속도를 줄입니다.
                    playerMove.moveSpeed = originalSpeed * slowMultiplier;

                    if (rb.linearVelocity.y < 0) // 내려가고 있을 때 더 빨리 가라앉게
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, sinkingVelocity);
                    }
                }
            }
        }

        // 슬라임을 벗어나는 순간 실행
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                var playerMove = collision.GetComponent<Player>();

                if (playerMove != null && originalSpeed != 0)
                {
                    // 원래 속도로 복구시킵니다.
                    playerMove.moveSpeed = originalSpeed;
                    originalSpeed = 0; // 초기화
                }
            }
        }
        private void OnDisable()
        {
            // 오브젝트가 갑자기 꺼질 경우를 대비해 효과 초기화
            isEffectActive = false;
        }


        #endregion

        #region Custom Method
        private IEnumerator FlowAndFadeRoutine()
        {
            // 1. 유지 단계 (플레이어를 방해함)
            float timer = 0f;
            while (timer < activeDuration)
            {
                transform.localPosition += Vector3.down * flowSpeed * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }

            // 2. 소멸 단계 (서서히 사라짐)
            Color currentColor = spriteRenderer.color;
            while (currentColor.a > 0)
            {
                transform.localPosition += Vector3.down * flowSpeed * Time.deltaTime;
                currentColor.a -= fadeSpeed * Time.deltaTime;
                spriteRenderer.color = currentColor;

                // 알파값이 많이 낮아지면 감속 효과를 미리 끔 (시각적 일치)
                if (currentColor.a < 0.2f) isEffectActive = false;

                yield return null;
            }

            // 부모 매니저에게 내가 꺼짐을 알림
            SlimePuddleParent manager = transform.parent.GetComponent<SlimePuddleParent>();
            if (manager != null)
            {
                manager.CheckAndDisableParent();
            }

            gameObject.SetActive(false);
        }
        #endregion
    }
}
