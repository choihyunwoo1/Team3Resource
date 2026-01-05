using UnityEngine;
using UnityEngine.UI;

namespace JS
{
    /// <summary>
    /// 슬라임 UI 흘러내리면서 사라지는 효과
    /// </summary>
    public class SlimeEffect : MonoBehaviour
    {
        #region Variables
        public Image img;
        private RectTransform rectTransform;

        [Header("Settings")]
        public float fadeSpeed = 0.5f;   // 투명해지는 속도
        public float fallSpeed = 50f;    // 아래로 내려가는 속도 (픽셀 단위)

        // 다시 활성화될 때 위치를 초기화하기 위한 변수
        private Vector2 startAnchoredPosition;
        private bool isInitialized = false; // 초기 위치 저장 여부 체크
        #endregion

        #region Unity Event Method
        void Awake()
        {
            img = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();

            // 처음에 생성된 위치를 저장 (나중에 리셋할 때 사용)
            if (rectTransform != null)
            {
                startAnchoredPosition = rectTransform.anchoredPosition;
                isInitialized = true;
            }
        }

        // 오브젝트가 활성화될 때마다 초기 알파값 세팅 (재사용 대비)
        void OnEnable()
        {
            //1. 이미지 알파값 리셋 (불투명하게)
            if (img != null)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }

            // 2. 위치 리셋 (저장해둔 처음 위치로)
            if (isInitialized && rectTransform != null)
            {
                rectTransform.anchoredPosition = startAnchoredPosition + new Vector2(Random.Range(-50f, 50f), 0);
            }
        }
        void Update()
        {
            // 안전 장치: 컴포넌트가 없으면 동작하지 않음
            if (img == null || rectTransform == null) return;

            if (img.color.a > 0)
            {
                // 1. 알파값 감소 (투명화)
                Color c = img.color;
                c.a -= fadeSpeed * Time.deltaTime;
                img.color = c;

                // 2. 위치 이동 (내려가기)
                rectTransform.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;
            }
            else
            {
                // 완전히 투명해지면 오브젝트 비활성화
                gameObject.SetActive(false);
            }
        }

        #endregion

        #region Custom Method
        

        #endregion
    }
}
