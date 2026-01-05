using UnityEngine;

public class SlimePuddleParent : MonoBehaviour
{
    // 부모가 SetActive(true) 될 때마다 호출됩니다.
    private void OnEnable()
    {
        // 모든 자식 오브젝트를 순회하며 다시 활성화시킵니다.
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    // 자식이 꺼질 때마다 호출되는 함수
    public void CheckAndDisableParent()
    {
        // 다음 프레임에 체크하거나, 현재 꺼지려는 자식을 제외하고 활성화된 게 있는지 확인
        StartCoroutine(CheckRoutine());
    }

    private System.Collections.IEnumerator CheckRoutine()
    {
        // 자식이 SetActive(false) 되는 찰나의 시간을 기다림
        yield return null;

        bool anyChildActive = false;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                anyChildActive = true;
                break;
            }
        }

        // 활성화된 자식이 하나도 없다면 부모도 꺼짐
        if (!anyChildActive)
        {
            gameObject.SetActive(false);
            Debug.Log("모든 슬라임이 사라져서 부모 오브젝트를 비활성화합니다.");
        }
    }

}
