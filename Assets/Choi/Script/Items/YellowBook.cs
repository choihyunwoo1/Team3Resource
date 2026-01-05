using UnityEngine;

namespace Choi
{
    public class YellowBook : PickupItem
    {
        #region Variables
        [SerializeField] private float multiplier = 1.2f;
        #endregion

        #region Custom Method
        protected override bool PickUp(Player player)
        {
            GameManager.BuffEnemy(EnemyBuffType.Yellow, multiplier);

            // 아이템 획득 등록
            ItemManager.Instance.RegisterItem(ItemType.D);

            return true;
        }
        #endregion
    }
}