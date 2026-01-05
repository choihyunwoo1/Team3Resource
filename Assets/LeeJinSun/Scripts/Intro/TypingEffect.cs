using UnityEngine;
using System.Collections;
using TMPro;

namespace JS
{
    public class TypingEffect : MonoBehaviour
    {
        #region Variables
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI tx;

        [Header("Settings")]
        [SerializeField] private string _text = "END?";       // 출력할 문장

        [Tooltip("글자 사이의 속도 (높을수록 천천히)")]
        [SerializeField] private float typingSpeed = 0.1f;

        [Header("Mode Settings")]
        [Tooltip("체크하면 END?를 한 줄씩 계속 추가합니다. 체크 안 하면 한 번만 쓰고 끝납니다.")]
        [SerializeField] private bool isRepeatMode = false;
        [SerializeField] private int repeatCount = 3; // 몇 줄까지 추가할 것인지
        [SerializeField] private float lineDelay = 0.1f; // 문장 완성 후 다음 문장까지의 대기 시간

        // 상태를 저장하는 변수
        private bool isTypingFinished = false;

        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            if (tx == null) tx = GetComponent<TextMeshProUGUI>();

            // 1. 이미 타이핑이 끝난 상태라면?
            // 반복 모드가 아니고 이미 끝났다면 유지
            if (!isRepeatMode && isTypingFinished)
            {
                tx.text = _text;
                return;
            }

            // 2. 처음 실행되는 경우라면 타이핑 시작
            StopAllCoroutines();
            StartCoroutine(Typing());
        }
        #endregion

        #region Custom Method
        IEnumerator Typing()
        {
            tx.text = "";

            if (isRepeatMode)
            {
                string accumulatedText = "";
                for (int r = 0; r < repeatCount; r++)
                {
                    string lineToAdd = _text + "\n";
                    for (int i = 0; i <= lineToAdd.Length; i++)
                    {
                        tx.text = accumulatedText + lineToAdd.Substring(0, i);
                        yield return new WaitForSeconds(typingSpeed);
                    }
                    accumulatedText = tx.text;

                    // 문장 사이의 대기 시간 (여기서 속도가 결정됨)
                    yield return new WaitForSeconds(lineDelay);
                }
            }
            else
            {
                // --- 연출 1: 기존 한 번만 타이핑 모드 ---
                for (int i = 0; i <= _text.Length; i++)
                {
                    tx.text = _text.Substring(0, i);
                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            isTypingFinished = true;
        }

        #endregion
    }
}



