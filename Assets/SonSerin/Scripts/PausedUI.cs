using UnityEngine;


namespace Serin
{
    /// <summary>
    /// Paused UI를 관리하는 클래스
    /// </summary>
    public class PausedUI : MonoBehaviour
    {
        #region Variables
        public GameObject pausedUI;

        public SceneFader fader;
        [SerializeField]
        private string loadToScene = "MainMenu";

        private GameObject thePlayer;
        #endregion


        #region Unity Event Method
        private void Awake()
        {
            //플레이어 움직임
            //thePlayer = FindFirstObjectByType<PlayerMove>().gameObject; 
        }
        private void Update()
        {
            //esc 버튼으로 토글
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Toggle();
            }
        }
        #endregion

        #region Custom Method
        private void Toggle()
        {
            pausedUI.SetActive(!pausedUI.activeSelf);

            if (pausedUI.activeSelf)
            {
                Time.timeScale = 0.0f;
                //플레이어 인풋기능 제거
                //thePlayer.GetComponent<CharacterInput>().enabled = false;
            }
            else
            {
                Time.timeScale = 1.0f;
                //플레이어 인풋기능 활성화
                //thePlayer.GetComponent<CharacterInput>().enabled = true;
            }
        }

        public void MainMenu()
        {
            //눌러서 메인메뉴로 돌아가기
            //fader.FadeTo(loadToScene);
            Time.timeScale = 1.0f;
            Debug.Log("Go To Menu");
        }
        public void Option()
        {
            //눌러서 ShoeOptionUI           
            Debug.Log("옵션 창 열기");
        }
        public void Restart()
        {
            //눌러서 처음부터 다시하기
            Debug.Log("재시작");
        }

        public void Continue()
        {
            //Paused UI를 비활성화 하고 이어서 계속하기
            Debug.Log("계속하기");
            //Toggle();
        }

        #endregion
    }
}
