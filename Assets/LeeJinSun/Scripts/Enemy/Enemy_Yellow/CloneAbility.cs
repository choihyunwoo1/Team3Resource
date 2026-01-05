using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace JS
{
    /// <summary>
    /// Enemy의 기믹 연출, 플레이어에게 가깝게 다가가기, 분신술, 화면 색 반전 추가
    /// </summary>
    public class CloneAbility : MonoBehaviour, IEnemyAbility
    {
        #region Variables
        private Enemy_Main owner;
        private GameManager gameManager;

        [Header("Visuals")]
        [SerializeField] private GameObject cloneVisual; // 분신 외형 (자식 오브젝트)
        [SerializeField] private GameObject clonePrefab; // 분신으로 소환할 프리팹 (자기 자신과 똑같이 생긴 것)

        [Header("Settings")]        
        [SerializeField] private float spawnRadius = 5f;  // 분신이 배치될 범위
        [SerializeField] private float cloneDuration = 3f; // 분신 유지 시간

        [Header("Random Count Settings")]
        [SerializeField] private int minCloneCount = 2;
        [SerializeField] private int maxCloneCount = 6;

        [Header("Random Scale Settings")]
        [SerializeField] private float minScale = 0.8f;   // 최소 크기 배율
        [SerializeField] private float maxScale = 1.5f;   // 최대 크기 배율

        [Header("Interval")]
        [SerializeField] private float minWaitTime = 4f;
        [SerializeField] private float maxWaitTime = 8f;

        [Header("Jumpscare Renderer Feature")]
        [SerializeField] private string featureName = "FullScreenPassRendererFeature"; // Renderer2D에 설정한 Name과 동일해야 함
        private FullScreenPassRendererFeature invertFeature;

        private List<GameObject> activeClones = new List<GameObject>();
        private Vector3 originalScale; // 본체의 원래 크기 저장용
        #endregion

        #region Custom Method
        public void Setup(Enemy_Main enemy)
        {
            owner = enemy;
            gameManager = Object.FindAnyObjectByType<GameManager>();

            originalScale = owner.transform.localScale;

            // 렌더러 에셋에서 Full Screen Pass Feature 찾아오기
            var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipeline != null)
            {
                // Renderer List 중 현재 사용 중인 0번(Renderer2D)에서 Feature 검색
                // 유니티 6에서는 좀 더 직관적인 접근이 필요할 수 있습니다.
                invertFeature = GetRendererFeature(featureName);
            }

        }

        public void OnEnter()
        {
            StopAllCoroutines();
            if (cloneVisual != null) cloneVisual.SetActive(true);

            // 게임오버 시 분신 제거를 위해 이벤트 구독
            if (gameManager != null) gameManager.OnGameOver += HandleGameOver;

            StartCoroutine(CloneRoutine());
        }

        public void OnExit()
        {
            StopAllCoroutines();
            if (gameManager != null) gameManager.OnGameOver -= HandleGameOver;

            if (cloneVisual != null) cloneVisual.SetActive(false);
            ClearClones();

            // 혹시 본체가 숨겨진 상태에서 종료되면 다시 보이게 설정
            StopAllCoroutines();

            SetFeatureActive(false); // 종료 시 반드시 꺼줌
            //invertFeature.SetActive(false);
        }

        public void OnTick() { }

        public void OnGameOver() => OnExit();

        private IEnumerator CloneRoutine()
        {
            while (true)
            {
                // 1. 랜덤 대기 (플레이어를 추적하는 시간)
                yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

                // 2. 분신 소환 시작
                Debug.Log("이너미 분신술 발동!");
                //색 반전 켜기
                //if (invertFeature != null) invertFeature.SetActive(true);
                SetFeatureActive(true);

                int randomCount = Random.Range(minCloneCount, maxCloneCount + 1);
                yield return new WaitForSeconds(0.5f);
                SpawnClones(randomCount);

                //invertFeature.SetActive(false);
                SetFeatureActive(false);

                // 3. 본체는 잠시 투명하게 하거나 판정을 끔 (선택 사항)
                // owner.GetComponent<SpriteRenderer>().enabled = false; 

                // 4. 유지 시간만큼 대기
                yield return new WaitForSeconds(cloneDuration);

                // 5. 분신 제거 및 본체 복귀
                ClearClones();
                // owner.GetComponent<SpriteRenderer>().enabled = true;

                // 분신술 시간이 끝나면 본체 크기를 원래대로 돌려놓습니다.
                owner.transform.localScale = originalScale;
            }
        }

        private void SetFeatureActive(bool active)
        {
            if (invertFeature != null)
            {
                invertFeature.SetActive(active);
            }
        }

        // 렌더러 피처를 찾는 헬퍼 함수
        private FullScreenPassRendererFeature GetRendererFeature(string name)
        {
            // 1. 현재 사용 중인 URP 에셋 가져오기
            var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipeline == null) return null;

            // 2. 리플렉션을 이용해 내부의 rendererDataList에 접근
            // 유니티 6에서 접근 제한이 걸린 필드에 접근하는 가장 확실한 방법입니다.
            FieldInfo propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
            var rendererDataList = (ScriptableRendererData[])propertyInfo?.GetValue(pipeline);

            if (rendererDataList != null && rendererDataList.Length > 0)
            {
                // 3. 0번 렌더러(Renderer2D)에서 이름으로 Feature 찾기
                foreach (var feature in rendererDataList[0].rendererFeatures)
                {
                    if (feature.name == name && feature is FullScreenPassRendererFeature fullScreenFeature)
                    {
                        return fullScreenFeature;
                    }
                }
            }
            return null;
        }

        private void SpawnClones(int count)
        {
            ClearClones(); // 혹시 남아있을 분신 제거

            Vector3 centerPos = owner.player != null ? owner.player.position : transform.position;

            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = centerPos + (Vector3)Random.insideUnitCircle * spawnRadius;

                // [추가] 랜덤 크기 계산
                float randomScaleMult = Random.Range(minScale, maxScale);
                Vector3 targetScale = originalScale * randomScaleMult;

                if (i == 0)
                {
                    // 본체 이동 및 랜덤 크기 적용
                    owner.transform.position = spawnPos;
                    owner.transform.localScale = targetScale;
                }
                else
                {
                    // 분신 생성 및 랜덤 크기 적용
                    GameObject clone = Instantiate(clonePrefab, spawnPos, Quaternion.identity);
                    clone.transform.localScale = targetScale;
                    activeClones.Add(clone);
                }
            }

            Debug.Log($"분신술! 개수: {count - 1}, 크기 다양화 적용됨");
        }

        private void ClearClones()
        {
            foreach (var clone in activeClones)
            {
                if (clone != null) Destroy(clone);
            }
            activeClones.Clear();
        }

        private void HandleGameOver(DeathCause cause) => OnGameOver();
        #endregion
    }
}
