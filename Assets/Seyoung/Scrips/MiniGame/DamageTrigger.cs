using UnityEngine;
using System.Collections;

namespace Team3
{
    public class DamageTrigger : MonoBehaviour
    {
        public int damage = 10;
        public float cooldown = 0.5f;
        public MiniGameEnemy enemy;
        [SerializeField]
        private AttackItem attackItem;

        [SerializeField]
        private bool canDamage = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!canDamage) return;

            if (!other.CompareTag("Player"))
                return;

            if (attackItem.eatItem == false)
                return;

            if (attackItem.eatItem == true)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                StartCoroutine(DamageCooldown());
                attackItem.eatItem = false;
            }
        }

        private IEnumerator DamageCooldown()
        {
            canDamage = false;
            yield return new WaitForSeconds(cooldown);
            canDamage = true;
        }
    }
}