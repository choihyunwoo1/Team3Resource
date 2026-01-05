using UnityEngine;
using System.Collections;

namespace JS
{
    /// <summary>
    /// 인트로씬의 전체적인 기능을 관리하는 클래스
    /// 페이드 인, 스킵, 책 넘기기 버튼, 마지막 장 오브젝트 관리, 페이드 아웃 관리
    /// </summary>
    public class IntroManager : MonoBehaviour
    {
        #region Variables
        //참조
        public Book book;
        //public SceneFader fader;
        public GameObject skipButton;
        public GameObject bookButton;

        //페이드 인, 아웃
        [SerializeField] private string loadToScene = "";

        //마지막 장
        public GameObject lastPage;

        //깜빡임 효과
        public GameObject blinkEffect;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //페이드 인
            
            // 0.5초 뒤에 활성화 함수 호출
            StartCoroutine(ShowSkipButton());
        }

        private void Update()
        {
            //책 마지막 페이지일 때
            if(book != null && book.currentPage >= 6)
            {
                bookButton.SetActive(false);
                lastPage.SetActive(true);
            }

            //마지막 페이지가 켜져있을 대
            if(lastPage.activeSelf == true)
            {
                StartCoroutine(ExitIntro());
            }
        }
        #endregion

        #region Custom Method
        //스킵버튼 보이게
        IEnumerator ShowSkipButton()
        {
            yield return new WaitForSeconds(1f);
            skipButton.SetActive(true);

        }

        //스킵버튼 클릭
        public void SkipButton()
        {
            //페이드 아웃
            //fader.FadeTo(loadToScene);
        }

        IEnumerator ExitIntro()
        {
            yield return new WaitForSeconds(2f);

            //눈 깜빡임
            blinkEffect.SetActive(true);

            yield return new WaitForSeconds(1f);

            //페이드 아웃
            //fader.FadeTo(loadToScene);
        }


        #endregion
    }
}
