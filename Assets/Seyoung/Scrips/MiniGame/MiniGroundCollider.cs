using UnityEngine;

namespace Team3
{
    public class MiniGroundCollider : MonoBehaviour
    {
        public PlayerMove player;
        [SerializeField] bool isGround = false;

        private void OnTriggerStay2D(Collider2D other)
        {
            isGround = true;

            if (other.CompareTag("Ground"))
                player.SetGrounded(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            isGround = false;

            if (other.CompareTag("Ground"))
                player.SetGrounded(false);
        }
    }
}