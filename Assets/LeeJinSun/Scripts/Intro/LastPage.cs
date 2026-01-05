using UnityEngine;
using System.Collections;

namespace JS
{
    /// <summary>
    /// 마지막 장 글씨 효과 스크립트
    /// </summary>
    public class LastPage : MonoBehaviour
    {
        #region Variables
        //글씨 오브젝트
        public GameObject yellowText;
        public GameObject redText;
        public GameObject purpleText;
        public GameObject greenText;
        public GameObject mainText;
        #endregion

        #region Unity Event Method
        void Start ()
        {
            StartCoroutine(ShowTexts());
        }
        #endregion

        #region Custom Method
        IEnumerator ShowTexts()
        {
            yield return new WaitForSeconds(0.3f);
            yellowText.SetActive(true);

            yield return new WaitForSeconds(0.3f);
            redText.SetActive(true);

            yield return new WaitForSeconds(0.3f);
            purpleText.SetActive(true);

            yield return new WaitForSeconds(0.3f);
            greenText.SetActive(true);

            yield return new WaitForSeconds(1f);
            mainText.SetActive(true);
        }
        #endregion
    }
}
