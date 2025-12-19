using UnityEngine;

namespace Choi
{
    public class LaserBeam : MonoBehaviour
    {
        [SerializeField] private float damageInterval = 0.1f;
        private float damageTimer;

        private void OnEnable()
        {
            damageTimer = 0f;
        }

        private void Update()
        {
            damageTimer += Time.deltaTime;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (damageTimer < damageInterval)
                return;

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Die(DeathCause.EnemyA);
                damageTimer = 0f;
            }
        }
    }
}
