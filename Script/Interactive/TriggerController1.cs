using UnityEngine;

namespace Choi
{
    public class TriggerController : MonoBehaviour
    {
        public Animator animator;
        public GameObject triggerObject;

        private bool triggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered)
                return;

            if (!other.CompareTag("Player"))
                return;

            triggered = true;

            if (animator != null)
                animator.SetTrigger("OnTrigger");

            if (triggerObject != null)
                triggerObject.SetActive(true);
        }

        // 🔹 애니메이션 이벤트에서 호출됨
        public void OnAnimationEnd()
        {
            Destroy(gameObject);
        }
    }
}
