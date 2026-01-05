using UnityEngine;

namespace Team3
{
    public class MiniGameEnemy : MonoBehaviour
    {
        public int scorePerHit = 10; // 데미지 1회당 점수

        public void TakeDamage(int damage)
        {

            // ⭐ 점수 누적
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scorePerHit);
            }
        }
    }
}
