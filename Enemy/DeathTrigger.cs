using UnityEngine;

namespace Choi
{
    /// <summary>
    /// 이 영역에 닿으면 지정된 사망 원인으로 플레이어가 사망합니다.
    /// </summary>
    public class DeathTrigger : MonoBehaviour
    {
        #region Variables
        [SerializeField] private DeathCause deathCause = DeathCause.Trap;
        #endregion

        #region Unity Event Method
        private void OnTriggerEnter2D(Collider2D other)
        {
            Player player = other.GetComponent<Player>();
            if (player == null)
                return;

            player.Die(deathCause);
        }
        #endregion
    }
}
