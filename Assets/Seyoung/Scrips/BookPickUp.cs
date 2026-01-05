using UnityEngine;

namespace Team3
{
    public class BookPickUp : MonoBehaviour
    {
        [SerializeField] private Color lightColor;
        [SerializeField] private float lightIntensity = -1f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            GlobalLightController.Instance.SetLight(lightColor, lightIntensity);
            Destroy(gameObject);
        }
    }
}