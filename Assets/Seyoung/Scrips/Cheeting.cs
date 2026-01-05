using UnityEngine;

namespace Team3
{

    public class Cheeting : MonoBehaviour
    {

        public bool isCheating = false;

        [Header("Teleport Points")]
        public Transform point1;
        public Transform point2;
        public Transform point3;
        public Transform point4;
        public Transform point5;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!isCheating) return;


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TeleportTo(point1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TeleportTo(point2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TeleportTo(point3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TeleportTo(point4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TeleportTo(point5);
            }
        }

        private void TeleportTo(Transform target)
        {
            if (target == null) return;

            // 이동 중이던 물리 상태 정리
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            transform.position = target.position;
            Debug.Log($"[CHEAT] Player Teleport → {target.name}");
        }
    }
}