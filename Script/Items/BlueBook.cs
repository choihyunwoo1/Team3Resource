using UnityEngine;

namespace Choi
{
    public class BlueBook : PickupItem
    {
        #region Variables
        [SerializeField] private float scaleMultiplier = 1.2f;
        #endregion

        #region Custom Method
        protected override bool PickUp(Player player)
        {
            GameManager.BuffEnemy(EnemyBuffType.ScaleUp, scaleMultiplier);
            return true;
        }
        #endregion
    }
}