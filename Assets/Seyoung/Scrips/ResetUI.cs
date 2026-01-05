using UnityEngine;

namespace Team3
{
    public class ResetUI : MonoBehaviour
    {
        public GameObject on;
        public GameObject off;

        public void Set(bool isOn)
        {
            on.SetActive(isOn);
            off.SetActive(!isOn);
        }
    }
}