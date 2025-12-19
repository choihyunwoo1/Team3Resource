using UnityEngine;

namespace Choi
{
    /// <summary>
    /// 메인메뉴 씬을 관리하는 클래스
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        #region Variables
        public SceneFader fader;
        public string loadToScene = "PlayScene";

        public GameObject diaryUI;
        #endregion

        #region Unity Event Method

        #endregion

        #region Custom Method
        public void OnPlayButton()
        {
            SceneFader.FadeTo(loadToScene);
        }

        public void OpenDiary()
        {
            diaryUI.SetActive(true);
        }
        public void CloseDiary()
        {
            diaryUI.SetActive(false);
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }
        #endregion
    }
}