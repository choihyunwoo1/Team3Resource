using UnityEngine;

namespace Team3
{
    public class MiniFrontCollider : MonoBehaviour
    {
        public PlayerMove player;
        [SerializeField] bool isWall = false;


        private void OnTriggerStay2D(Collider2D other)
        {
            isWall = true;

            if (other.CompareTag("Wall"))
                player.SetFrontBlocked(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            isWall = false;

            if (other.CompareTag("Wall"))
                player.SetFrontBlocked(false);
        }
    }
}