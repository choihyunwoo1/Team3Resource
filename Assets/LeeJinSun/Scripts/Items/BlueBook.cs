using UnityEngine;

namespace JS
{
    public class BlueBook : PickupItem
    {
        #region Variables
        [SerializeField] private float scaleMultiplier = 1.2f;
        #endregion

        #region Custom Method
        protected override bool PickUp(Player player)
        {
            GameManager.BuffEnemy(EnemyBuffType.Blue, scaleMultiplier);
            return true;
        }
        #endregion
    }
}