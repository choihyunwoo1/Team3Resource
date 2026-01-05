using System.Collections;
using UnityEngine;

namespace Team3
{
    public class AttackItem : MonoBehaviour
    {
        //아이템 트리거에 닿으면 아이템을 먹고, 이 아이템을 가지고 데미지 트리거에 들어가야 데미지를 입음
        private SpriteRenderer itemspriteRenderer;

        public bool eatItem = false;

        private void Awake()
        {
            itemspriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            StartCoroutine(TrueItem());
        }

        private IEnumerator TrueItem()
        {
            itemspriteRenderer.enabled= false;
            eatItem = true;
            Debug.Log("아이템 먹음");
            yield return new WaitForSeconds(5f);

            itemspriteRenderer.enabled = true;
            Debug.Log("아이템 활성화");

        }
    }
    
}