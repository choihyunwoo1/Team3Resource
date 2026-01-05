using UnityEngine;
using System.Collections;

namespace Team3
{
    public class Teleport : MonoBehaviour
    {
        public Transform end;          // 도착 위치
        public float delay = 1f;       // 텔레포트 전 대기 시간

        public string teleportAnim = "Teleport"; // 애니메이션 트리거 이름

        private bool isTeleporting = false;

        //흔들림 - 위아래
        [SerializeField] private float bobingAmount = 1f;    //흔들림 량
        [SerializeField] private float verticalBobFrequency = 1f;    //흔들림 속도

        //회전
        [SerializeField] private float rotateSpeed = 360f;

        private Vector3 localStartPos;

        protected void Start()
        {
            localStartPos = transform.localPosition;
        }

        protected void Update()
        {
            float bob = Mathf.Sin(Time.time * verticalBobFrequency) * bobingAmount;
            transform.localPosition = localStartPos + Vector3.up * bob;

            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isTeleporting) return;
            if (!other.CompareTag("Player")) return;

            StartCoroutine(TeleportSequence(other));
        }

        private IEnumerator TeleportSequence(Collider2D playerCol)
        {
            isTeleporting = true;

            Rigidbody2D rb = playerCol.GetComponent<Rigidbody2D>();
            Animator anim = playerCol.GetComponent<Animator>();

            // 이동 잠금
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            // 애니메이션
            if (anim != null)
                anim.SetTrigger(teleportAnim);

            yield return new WaitForSeconds(delay);

            // 텔레포트
            rb.position = end.position;

            // 이동 복구
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;


            isTeleporting = false;
        }
    }
}
