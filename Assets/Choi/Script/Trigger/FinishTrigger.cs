using UnityEngine;

namespace Choi
{
    public class FinishTrigger : MonoBehaviour
    {
        private bool triggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered)
                return;

            Player player = other.GetComponent<Player>();
            if (player == null)
                return;

            triggered = true;

            GameManager gm = FindObjectOfType<GameManager>();

            if (gm == null)
            {
                Debug.LogError("[FinishTrigger] GameManager not found in scene.");
                return;
            }

            // 1. 상태 전환
            gm.RequestStageClear();

            // 2. 엔딩 타입 결정
            EndingType result = ItemManager.Instance.GetEndingType();

            // 3. 엔딩 컷씬 재생
            CutsceneManager.Instance.PlayEndingCutscene(result);

            // 4. 비활성화
            gameObject.SetActive(false);
        }
    }
}
