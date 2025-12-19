using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Choi
{
    public class SceneFader : MonoBehaviour
    {
        public static SceneFader Instance;

        public Image img;
        public AnimationCurve curve;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            FadeIn();
        }

        // ---------- Static API ----------
        public static void FadeIn(float delay = 0f)
        {
            Instance.StartCoroutine(Instance.FadeInCoroutine(delay));
        }

        public static void FadeTo(string sceneName)
        {
            Instance.StartCoroutine(Instance.FadeOutCoroutine(sceneName));
        }

        public static void FadeTo(int buildIndex)
        {
            Instance.StartCoroutine(Instance.FadeOutCoroutine(buildIndex));
        }

        // ---------- Instance Logic ----------
        private IEnumerator FadeInCoroutine(float delayTime)
        {
            img.color = new Color(0f, 0f, 0f, 1);
            if (delayTime > 0f)
                yield return new WaitForSeconds(delayTime);

            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                float a = curve.Evaluate(t);
                img.color = new Color(0, 0, 0, a);
                yield return null;
            }
        }

        private IEnumerator FadeOutCoroutine(string sceneName)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                float a = curve.Evaluate(t);
                img.color = new Color(0, 0, 0, a);
                yield return null;
            }

            if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName);
        }

        private IEnumerator FadeOutCoroutine(int buildIndex)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                float a = curve.Evaluate(t);
                img.color = new Color(0, 0, 0, a);
                yield return null;
            }

            if (buildIndex >= 0)
                SceneManager.LoadScene(buildIndex);
        }
    }
}