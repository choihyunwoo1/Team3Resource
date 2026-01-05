using UnityEngine;

namespace JS
{
    public class DTrigger : MonoBehaviour
    {
        [SerializeField] private Enemy_Main enemy;
        [SerializeField] private Transform waypoint;

        private bool triggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered)
                return;

            Player player = other.GetComponent<Player>();
            if (player == null)
                return;

            triggered = true;

            enemy.GoToWaypoint(waypoint);

            gameObject.SetActive(false);
        }
    }
}
