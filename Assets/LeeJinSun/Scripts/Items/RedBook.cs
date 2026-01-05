using UnityEngine;

namespace JS
{
    public class RedBook : PickupItem
    {
        #region Variables
        [SerializeField] private float multiplier = 1.2f;
        #endregion

        #region Custom Method
        protected override bool PickUp(Player player)
        {
            GameManager.BuffEnemy(EnemyBuffType.Red, multiplier);
            return true;

        }
        #endregion
    }
}