using UnityEngine;

namespace Choi
{
    public class GreenBook : PickupItem
    {
        #region Variables

        #endregion

        #region Custom Method
        protected override bool PickUp(Player player)
        {
            GameManager.BuffEnemy(EnemyBuffType.LaserBeam, 0f);
            return true;
        }
        #endregion
    }
}