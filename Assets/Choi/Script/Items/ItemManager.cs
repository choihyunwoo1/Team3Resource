using UnityEngine;

namespace Choi
{
        public enum ItemType
        {
            A,
            B,
            C,
            D,
            E
        }

    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance;

        public bool hasA;
        public bool hasB;
        public bool hasC;
        public bool hasD;
        public bool hasE;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void RegisterItem(ItemType type)
        {
            switch (type)
            {
                case ItemType.A:
                    hasA = true;
                    break;
                case ItemType.B:
                    hasB = true;
                    break;
                case ItemType.C:
                    hasC = true;
                    break;
                case ItemType.D:
                    hasD = true;
                    break;
                case ItemType.E:
                    hasE = true;
                    break;
            }

            // UI 갱신
            MaskUIManager.Instance.UpdateUI();
        }


        public EndingType GetEndingType()
        {
            int total = (hasA ? 1 : 0) +
                        (hasB ? 1 : 0) +
                        (hasC ? 1 : 0) +
                        (hasD ? 1 : 0) +
                        (hasE ? 1 : 0);

            if (total == 0)
                return EndingType.ZeroItem;

            if (total >= 1 && total <= 3)
                return EndingType.OneToThree;

            if (total == 5)
                return EndingType.AllFive;

            if (!hasA) return EndingType.MissingA;
            if (!hasB) return EndingType.MissingB;
            if (!hasC) return EndingType.MissingC;
            if (!hasD) return EndingType.MissingD;
            if (!hasE) return EndingType.MissingE;

            return EndingType.ZeroItem;
        }
    }
}
