using UnityEngine;
using System.Collections;

namespace Team3
{
    public class StageTrigger : MonoBehaviour
    {
        [Header("Targets")]
        public GameObject player;     // Player 오브젝트
        public GameObject enemyRoot;  // Enemy 부모 오브젝트
        public GameObject npc;

        [Header("Settings")]
        public float freezeTime = 2f;

        private bool triggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered) return;
            if (!other.CompareTag("Player")) return;

            triggered = true;

            StartCoroutine(FreezeSequence());

            if (npc != null)
                npc.SetActive(true);
        }

        private IEnumerator FreezeSequence()
        {
            // 🔒 Player Freeze
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            // 🔒 Enemy Freeze (부모 + 자식 스크립트 전부)
            MonoBehaviour[] enemyScripts = enemyRoot.GetComponentsInChildren<MonoBehaviour>();
            foreach (var script in enemyScripts)
            {
                script.enabled = false;
            }

            yield return new WaitForSeconds(freezeTime);

            // 🔓 Player Unfreeze
            if (playerRb != null)
            {
                playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }

            // 🔓 Enemy Unfreeze
            foreach (var script in enemyScripts)
            {
                script.enabled = true;
            }

        }
    }
}
