using UnityEngine;

namespace JS
{
    public class PurpleBook : PickupItem
    {
        #region Variables
        [SerializeField] private float multiplier = 1.2f;
        #endregion

        #region Custom Method
        protected override bool PickUp(Player player)
        {
            GameManager.BuffEnemy(EnemyBuffType.Purple, multiplier);
            return true;
        }
        #endregion
    }
}
