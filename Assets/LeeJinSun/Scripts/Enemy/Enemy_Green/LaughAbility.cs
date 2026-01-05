using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

namespace JS
{
    /// <summary>
    /// 마스크 웃음 공격(카메라 쉐이크)
    /// </summary>
    public class LaughAbility : MonoBehaviour, IEnemyAbility
    {
        #region Variables
        private Enemy_Main owner; // Enemy 본체 참조
        private GameManager gameManager;
        [SerializeField] private Animator animator;      //애니메이터

        [Header("Visual")]
        [SerializeField] private GameObject maskVisual;        // 자식으로 넣은 '마스크 모양' 오브젝트

        [Header("Cinemachine Settings")]
        [SerializeField] private CinemachineCamera vCam; // 하이어라키의 CinemachineCamera 연결
        private CinemachineBasicMultiChannelPerlin noiseModule;

        [Header("Random Interval")]
        [SerializeField] private float minWaitTime = 3f;
        [SerializeField] private float maxWaitTime = 7f;

        [Header("Random Shake")]
        [SerializeField] private float minShakeDuration = 0.5f;
        [SerializeField] private float maxShakeDuration = 1.2f;
        [SerializeField] private float shakeAmplitude = 1.5f; // 흔들림 강도 (Amplitude)
        [SerializeField] private float shakeFrequency = 2.0f;  // 흔들림 속도 (Frequency)

        //원래 크기
        private Vector3 originalScale;

        #endregion

        #region Custom Method
        // 1. 초기 설정: Enemy 본체가 자신을 등록할 때 호출
        public void Setup(Enemy_Main enemy)
        {
            owner = enemy;
            gameManager = Object.FindAnyObjectByType<GameManager>();
            animator = maskVisual.GetComponent<Animator>();
            originalScale = maskVisual.transform.localScale;

            // 시네머신 카메라에서 노이즈 모듈(Perlin)을 가져옵니다.
            if (vCam != null)
            {
                noiseModule = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        // 2. 능력 시작: 외형을 바꾸고 타이머 초기화
        public void OnEnter()
        {
            // 1. 기존 동작 정지 및 초기화
            StopAllCoroutines();
            ResetShake();

            // 2. 외형 변경 (자식 오브젝트 활성화 시 콜라이더도 자동 활성화)
            if (maskVisual != null) maskVisual.SetActive(true);

            // 3. 게임오버 이벤트 구독 (GameManager 스크립트 기반)
            if (gameManager != null)
            {
                gameManager.OnGameOver += HandleGameOver;
            }

            // 4. 웃음 루틴 시작
            StartCoroutine(LaughRoutine());
        }

        // 3. 실행: Enemy의 Update에서 매 프레임 호출됨
        public void OnTick()
        {

        }

        // 4. 능력 종료: 외형을 끄고 상태 정리
        public void OnExit()
        {
            // 1. 이벤트 구독 해제 (메모리 누수 방지)
            if (gameManager != null)
            {
                gameManager.OnGameOver -= HandleGameOver;
            }

            if (maskVisual != null) maskVisual.SetActive(false);

            //크기 리셋
            maskVisual.transform.localScale = originalScale;

            // 2. 모든 동작 정지 및 리셋
            StopAllCoroutines();
            ResetShake();
        }

        public void OnGameOver()
        {
            // 인터페이스에 추가된 게임오버 대응 함수
            OnExit();
        }


        private IEnumerator LaughRoutine()
        {
            while (true)
            {
                // [랜덤 대기] 다음 웃음까지 기다림
                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(waitTime);

                // [랜덤 쉐이크] 흔들림 지속 시간 결정
                float shakeDuration = Random.Range(minShakeDuration, maxShakeDuration);

                // --- [흔들림 강도 계산 로직] ---
                // 현재 크기가 커진 비율을 계산합니다 (기본 크기 1 대비 현재 크기)
                // 예: 크기가 2배면 scaleFactor는 2가 됩니다.
                float scaleFactor = maskVisual.transform.localScale.x;

                // 크기에 비례하여 흔들림 수치를 보정합니다.
                // 강도(Amplitude)와 빈도(Frequency)에 배율을 곱해줍니다.
                float dynamicAmplitude = shakeAmplitude * (scaleFactor * 1.5f);
                float dynamicFrequency = shakeFrequency * (1f + (scaleFactor * 1.2f));

                // 흔들림 시작
                if (animator != null)
                {
                    animator.SetBool("IsLaugh", true);

                    // 애니메이션이 입을 벌리는 지점까지 재생되도록 잠시 대기
                    yield return new WaitForSeconds(0.75f);

                    //Enemy_Main 스크립트 끄기
                    owner.enabled = false;

                    // === 부드럽게 커지는 효과 (Lerp) ===
                    Vector3 currentScale = maskVisual.transform.localScale;

                    // 최대 4배까지 커지도록 제한
                    if (currentScale.x < 4f)
                    {
                        Vector3 targetScale = currentScale * 1.2f;

                        //뒤로 물러날 목표 위치 계산 (현재 x포지션에서 왼쪽으로 1만큼 이동)
                        Vector3 startPos = owner.transform.position;
                        Vector3 targetPos = new Vector3(startPos.x - 5f, startPos.y, startPos.z);

                        float lerpTime = 0.5f;
                        float elapsed = 0f;

                        while (elapsed < lerpTime)
                        {
                            elapsed += Time.deltaTime;
                            float t = elapsed / lerpTime;

                            maskVisual.transform.localScale = Vector3.Lerp(currentScale, targetScale, t);

                            //크기가 어느 정도 커지면 뒤로 서서히 이동 (커지면서 플레이어와 거리를 둠)
                            if(maskVisual.transform.localScale.x > 1.5f)
                            {
                                owner.transform.position = Vector3.Lerp(startPos, targetPos, t);
                                Debug.Log("뒤로 간다잉");
                            }


                            yield return null;
                        }
                        maskVisual.transform.localScale = targetScale;

                        SetShake(dynamicAmplitude, dynamicFrequency);
                    }

                    //Enemy_Main 스크립트 켜기
                    owner.enabled = true;
                    // ==================================================================

                    // 재생 속도를 0으로 만들어 입 벌린 상태 고정
                    animator.SetFloat("LaughSpeed", 0f);
                }
                //SetShake(dynamicAmplitude, dynamicFrequency);
                Debug.Log($"이너미 웃음 발동! 지속 시간: {shakeDuration:F1}초");

                // 흔들림 유지
                yield return new WaitForSeconds(shakeDuration);

                // 흔들림 정지
                if (animator != null)
                {
                    // 속도를 다시 1로 돌려 입을 다무는 나머지 애니메이션 재생
                    animator.SetFloat("LaughSpeed", 1f);
                    animator.SetBool("IsLaugh", false);
                }

                ResetShake();
            }
        }

        //시네머신 노이즈 값을 설정해 화면 흔들기
        private void SetShake(float amplitude, float frequency)
        {
            if (noiseModule != null)
            {
                noiseModule.AmplitudeGain = amplitude;
                noiseModule.FrequencyGain = frequency;
            }
        }

        //흔들림 즉시 멈춤
        private void ResetShake()
        {
            if (noiseModule != null)
            {
                noiseModule.AmplitudeGain = 0f;
                noiseModule.FrequencyGain = 0f;
            }
        }

        //GameManager의 OnGameOver 이벤트 발생 시 호출
        private void HandleGameOver(DeathCause cause)
        {
            OnGameOver();
        }
        #endregion


    }
}
