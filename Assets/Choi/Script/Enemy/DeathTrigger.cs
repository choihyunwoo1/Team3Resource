using UnityEngine;

namespace Choi
{
    /// <summary>
    /// 이 영역에 닿으면 지정된 사망 원인으로 플레이어가 사망합니다.
    /// </summary>
    public class DeathTrigger : MonoBehaviour
    {
        [SerializeField] private DeathCause deathCause;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player p = other.GetComponent<Player>();
            if (p == null) return;

            p.Die(deathCause);
        }
    }
}
