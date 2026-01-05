using UnityEngine;

namespace JS
{
    public class GroundCollider : MonoBehaviour
    {
        public Player player;
        [SerializeField] bool isGround = false;

        private void OnTriggerStay2D(Collider2D other)
        {
            isGround = true;    

            if (other.CompareTag("Ground"))
                player.SetGrounded(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            isGround=false;

            if (other.CompareTag("Ground"))
                player.SetGrounded(false);
        }
    }
}