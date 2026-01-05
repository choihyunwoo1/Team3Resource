using UnityEngine;

namespace Choi
{
    public class EndingCalculator : MonoBehaviour
    {
        public static EndingType GetEndingType(int collectedCount, bool missingA, bool missingB, bool missingC, bool missingD, bool missingE)
        {
            if (collectedCount == 0)
                return EndingType.ZeroItem;

            if (collectedCount == 5)
                return EndingType.AllFive;

            // 1~3개
            if (collectedCount <= 3)
                return EndingType.OneToThree;

            // 누락된 아이템별 개별 엔딩
            if (missingA) return EndingType.MissingA;
            if (missingB) return EndingType.MissingB;
            if (missingC) return EndingType.MissingC;
            if (missingD) return EndingType.MissingD;
            if (missingE) return EndingType.MissingE;

            return EndingType.OneToThree;
        }
    }
}