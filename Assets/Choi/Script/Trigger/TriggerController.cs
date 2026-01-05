using UnityEngine;

namespace Choi
{
    public class TriggerController : MonoBehaviour
    {
        public Animator animator;
        public GameObject triggerObject;
        public GameObject setTrue;
        public GameObject setFalse;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            // 애니메이션 실행
            if (animator != null)
            {
                animator.SetTrigger("OnTrigger");
            }

            // 오브젝트 활성화
            if (triggerObject != null)
            {
                triggerObject.SetActive(true);
            }

            if (setTrue != null)
            {
                setTrue.SetActive(true);
                Debug.Log("Light");
            }
            if (setFalse != null)
            {
                setFalse.SetActive(false);
            }
            // 마지막에 한 번만 제거
            Destroy(gameObject);
        }

    }
}