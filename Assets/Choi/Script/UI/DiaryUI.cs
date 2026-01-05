using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Choi
{
    public class DiaryUI : MonoBehaviour
    {
        [System.Serializable]
        public class DiaryButton
        {
            public string cutsceneKey;     // key만 넣으면 자동 매칭됨
            public GameObject lockedObj;   // 잠김 상태용
            public GameObject unlockedObj; // 해금 버튼 오브젝트
            public Button button;          // unlocked 쪽 버튼

            [HideInInspector]
            public CutsceneData data;      // 자동 매칭된 ScriptableObject
        }

        [Header("Database")]
        [SerializeField] private List<CutsceneData> cutsceneDatabase; // ScriptableObject 목록

        [Header("UI Output")]
        [SerializeField] private Image previewImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Buttons")]
        [SerializeField] private List<DiaryButton> diaryButtons;

        private Dictionary<string, CutsceneData> dataMap;

        private void Awake()
        {
            BuildDataMap();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void BuildDataMap()
        {
            dataMap = new Dictionary<string, CutsceneData>();
            foreach (var data in cutsceneDatabase)
                dataMap[data.cutsceneKey] = data;
        }

        public void Refresh()
        {
            foreach (var btn in diaryButtons)
            {
                // 1. key 기반으로 ScriptableObject 자동 매칭
                if (dataMap.TryGetValue(btn.cutsceneKey, out CutsceneData data))
                {
                    btn.data = data;
                }
                else
                {
                    Debug.LogWarning($"CutsceneData 없음: {btn.cutsceneKey}");
                    continue;
                }

                // 2. 해금 여부 확인
                bool unlocked = DiarySystem.Instance.IsUnlocked(btn.cutsceneKey);

                btn.lockedObj.SetActive(!unlocked);
                btn.unlockedObj.SetActive(unlocked);

                // 3. 클릭 이벤트 등록
                if (unlocked)
                {
                    btn.button.onClick.RemoveAllListeners();
                    btn.button.onClick.AddListener(() => ShowCutscene(btn.data));
                }
            }
        }

        private void ShowCutscene(CutsceneData data)
        {
            Debug.Log("avvv");
            if (data == null) return;
            Debug.Log("avvv");

            // 좌측 박스 내용 갱신
            previewImage.sprite = data.previewImage;

            titleText.text = data.title;
            descriptionText.text = data.description;
        }
    }
}
