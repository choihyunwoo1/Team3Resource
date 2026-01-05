using UnityEngine;

namespace Serin
{
    /// <summary>
    /// MainMenu UI를 관리하는 클래스 
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {        
        #region Variables
        public SceneFader fader;
        [SerializeField]
        private string loadToScene = "PlayScene01";

        //UI
        public GameObject mainmenuUI;
        public GameObject optionUI;
        #endregion


        #region Custom Method
        public void StartButton()
        {
            //fader.FadeTo(loadToScene);
            Debug.Log("PlayScene1로 이동");
        }

        public void OptionsButton()
        {
            //ShowOptionUI();
            Debug.Log("Option 창 활성화");            
        }
        public void CollectionButton()
        {
            //1차 스토리 엔딩을 보고난 후 활성화되는 버튼
            Debug.Log("Option 창 활성화");
        }

        public void QuitButton()
        {
            //Application.Quit();
            Debug.Log("게임 종료");
        }
        #endregion
    }
}
