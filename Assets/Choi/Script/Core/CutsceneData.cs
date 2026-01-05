using UnityEngine;

namespace Choi
{
    [CreateAssetMenu(menuName = "Choi/CutsceneData")]
    public class CutsceneData : ScriptableObject
    {
        public string cutsceneKey;

        public string title;             // 제목 UI
        public Sprite previewImage;      // 왼쪽 이미지
        [TextArea] public string description; // 설명 텍스트
    }
}
