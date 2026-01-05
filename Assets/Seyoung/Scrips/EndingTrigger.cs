using System;
using System.Collections;
using UnityEngine;

//엔딩 트리거 통과 시 이네미,파티클, 카메라 무브 정지
namespace Team3
{
    public class EndingTrigger : MonoBehaviour
    {
        public GameObject enemy;
        public GameObject particle;

        private bool triggered = false;
        public MonoBehaviour cameraMoveScript;
        public GameObject item;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered) return;
            if (!other.CompareTag("Player")) return;

            triggered = true;

            if (enemy != null)
                enemy.SetActive(false);
            if (particle != null)
                particle.SetActive(false);

            if (cameraMoveScript != null)
                cameraMoveScript.enabled = false;

            if (item != null)
                item.SetActive(true);
        }

    }
}