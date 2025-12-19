using System;
using System.Collections.Generic;
using UnityEngine;

namespace Choi
{
    public class DiarySystem : MonoBehaviour
    {
        public static DiarySystem Instance { get; private set; }

        [SerializeField]
        private DiaryUI diaryUI; // UI 관리자를 Inspector에서 연결
                                 // 또는 FindObjectOfType<DiaryUI>() 가능

        private Dictionary<int, string> diaryData = new Dictionary<int, string>();

        private int entryCount = 0;

        public event Action<string> OnDiaryUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            LoadDiary(); // 기존 세이브 로드
        }

        void Start()
        {
            Player player = FindObjectOfType<Player>();
        }

        private void OnEnable()
        {
            if (CutsceneManager.Instance != null)
                CutsceneManager.Instance.OnCutsceneFinished.AddListener(GenerateDiaryEntry);
        }


        private void OnDisable()
        {
            if (CutsceneManager.Instance != null)
                CutsceneManager.Instance.OnCutsceneFinished.RemoveListener(GenerateDiaryEntry);
        }



        private void GenerateDiaryEntry(DeathCause cause)
        {
            entryCount++;

            string entry = cause switch
            {
                DeathCause.EnemyA => "누군가가 나를 보고 있었다...",
                DeathCause.Fall => "깊은 어둠 속으로 떨어졌다...",
                DeathCause.Trap => "날카로운 금속이 내 몸을 치었다...",
            };

            diaryData.Add(entryCount, entry);
            OnDiaryUpdated?.Invoke(entry);

            UnlockPanel(cause);               // 패널 해금
            diaryUI.ShowPanelByCause(cause);  // UI 패널 출력

            SaveDiary();
        }


        private void LoadDiary()
        {
            // PlayerPrefs / Json / Local File 등 원하는 방식으로 구현 가능
            // 참고용 더미 로딩 구조

            entryCount = PlayerPrefs.GetInt("DiaryCount", 0);

            for (int i = 1; i <= entryCount; i++)
            {
                string saved = PlayerPrefs.GetString($"Diary{i}", "");
                diaryData.Add(i, saved);
            }
        }


        private void SaveDiary()
        {
            PlayerPrefs.SetInt("DiaryCount", entryCount);

            foreach (var item in diaryData)
                PlayerPrefs.SetString($"Diary{item.Key}", item.Value);
        }


        public string GetEntry(int id)
        {
            if (diaryData.ContainsKey(id))
                return diaryData[id];

            return null;
        }

        public void UnlockPanel(DeathCause cause)
        {
            PlayerPrefs.SetInt($"{cause}_Unlocked", 1);
        }

        public int GetTotalEntryCount() => entryCount;
    }
}