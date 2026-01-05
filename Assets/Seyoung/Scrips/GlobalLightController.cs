using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Team3
{
    public class GlobalLightController : MonoBehaviour
    {
        public static GlobalLightController Instance;

        [SerializeField] private Light2D globalLight;

        private Color currentColor;
        private float currentIntensity;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // 시작 시 현재 상태 저장
            currentColor = globalLight.color;
            currentIntensity = globalLight.intensity;
        }

        public void SetLight(Color color, float intensity = -1f)
        {
            currentColor = color;

            if (intensity >= 0f)
                currentIntensity = intensity;

            globalLight.color = currentColor;
            globalLight.intensity = currentIntensity;
        }

        public Color GetCurrentColor() => currentColor;
    }
}
