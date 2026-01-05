using UnityEngine;
using UnityEngine.SceneManagement;

namespace JS
{
    /// <summary>
    /// 메인메뉴 씬을 관리하는 클래스
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        #region Variables
        public SceneFader fader;
        public string loadToScene = "PlayScene";

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            fader = GetComponent<SceneFader>();
        }
        #endregion

        #region Custom Method
        public void OnPlayButton()
        {
            fader.FadeTo(loadToScene);
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }
        #endregion
    }
}