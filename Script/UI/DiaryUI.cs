using UnityEngine;
using TMPro;

namespace Choi
{
    public class DiaryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text diaryTextField;
        [SerializeField] private GameObject[] diaryPanels;

        public void ShowPanelByCause(DeathCause cause)
        {
            foreach (var panel in diaryPanels)
                panel.SetActive(false);

            diaryPanels[(int)cause].SetActive(true);
        }

        public void DisplayEntry(string text)
        {
            diaryTextField.text = text;
        }
    }
}
