using UnityEngine;

namespace Choi
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

            // 플레이어 방향 반전
            player.ReverseDirection();

            // 적 이동
            enemy.GoToWaypoint(waypoint);

            // 트리거 비활성화
            gameObject.SetActive(false);
        }
    }
}
