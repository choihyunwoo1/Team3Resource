using UnityEngine;

namespace JS
{
    public class GreenBook : PickupItem
    {
        #region Variables
        [SerializeField] private float multiplier = 1.2f;
        #endregion

        #region Custom Method
        protected override bool PickUp(Player player)
        {
            GameManager.BuffEnemy(EnemyBuffType.Green, multiplier);
            return true;
        }
        #endregion
    }
}