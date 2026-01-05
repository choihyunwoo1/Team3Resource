using UnityEngine;

namespace Choi
{
    public class MaskUIManager : MonoBehaviour
    {
        public static MaskUIManager Instance;

        [Header("IdleMask UI Objects")]
        [SerializeField] private GameObject idleA;
        [SerializeField] private GameObject idleB;
        [SerializeField] private GameObject idleC;
        [SerializeField] private GameObject idleD;
        [SerializeField] private GameObject idleE;

        [Header("ColorMask UI Objects")]
        [SerializeField] private GameObject maskA;
        [SerializeField] private GameObject maskB;
        [SerializeField] private GameObject maskC;
        [SerializeField] private GameObject maskD;
        [SerializeField] private GameObject maskE;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void UpdateUI()
        {
            if (ItemManager.Instance == null)
                return;

            // Color Mask 업데이트
            bool a = ItemManager.Instance.hasA;
            bool b = ItemManager.Instance.hasB;
            bool c = ItemManager.Instance.hasC;
            bool d = ItemManager.Instance.hasD;
            bool e = ItemManager.Instance.hasE;

            maskA.SetActive(a);
            maskB.SetActive(b);
            maskC.SetActive(c);
            maskD.SetActive(d);
            maskE.SetActive(e);

            idleA.SetActive(!a);
            idleB.SetActive(!b);
            idleC.SetActive(!c);
            idleD.SetActive(!d);
            idleE.SetActive(!e);
        }

        public void HideAll()
        {
            maskA.SetActive(false);
            maskB.SetActive(false);
            maskC.SetActive(false);
            maskD.SetActive(false);
            maskE.SetActive(false);

            // Idle은 강제로 끄고 싶으면 끄기
            idleA.SetActive(false);
            idleB.SetActive(false);
            idleC.SetActive(false);
            idleD.SetActive(false);
            idleE.SetActive(false);
        }

    }
}
