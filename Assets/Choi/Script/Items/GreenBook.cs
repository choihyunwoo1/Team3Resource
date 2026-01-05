using UnityEngine;

namespace Choi
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

            // 아이템 획득 등록
            ItemManager.Instance.RegisterItem(ItemType.E);

            return true;
        }
        #endregion
    }
}