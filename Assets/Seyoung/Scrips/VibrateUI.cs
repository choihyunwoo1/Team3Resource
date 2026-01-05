using UnityEngine;

namespace Team3
{
    public class VibrateUI : MonoBehaviour
    {
        public GameObject onVibrate;
        public GameObject offVibrate;

        public void Set(bool isOn)
        {
            onVibrate.SetActive(isOn);
            offVibrate.SetActive(!isOn);
        }
    }
}