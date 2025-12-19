using UnityEngine;

namespace Choi
{
    public class RedBook : PickupItem
    {
        #region Variables
        [SerializeField] private float multiplier = 1.2f;
        #endregion

        #region Custom Method
        protected override bool PickUp(Player player)
        {
            GameManager.BuffEnemy(EnemyBuffType.SpeedUp, multiplier);
            return true;

        }
        #endregion
    }
}