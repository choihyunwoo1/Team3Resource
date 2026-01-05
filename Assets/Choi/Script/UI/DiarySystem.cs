using System.Collections.Generic;
using UnityEngine;

namespace Choi
{
    public class DiarySystem : MonoBehaviour
    {
        public static DiarySystem Instance;

        private HashSet<string> unlocked = new HashSet<string>();
        private const string SaveKey = "UnlockedCutscenes";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            DontDestroyOnLoad(gameObject); 

            Load();
        }

        public void UnlockCutscene(string key)
        {
            if (unlocked.Add(key))
                Save();
        }

        public bool IsUnlocked(string key)
        {
            return unlocked.Contains(key);
        }

        public List<string> GetUnlockedList()
        {
            return new List<string>(unlocked);
        }

        private void Save()
        {
            string data = string.Join(",", unlocked);
            PlayerPrefs.SetString(SaveKey, data);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
                return;

            var data = PlayerPrefs.GetString(SaveKey);
            if (string.IsNullOrEmpty(data))
                return;

            unlocked = new HashSet<string>(data.Split(','));
        }
        public void ResetDiary()
        {
            unlocked.Clear();
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

    }
}