using Team3;
using UnityEngine;
using UnityEngine.UI;

namespace Team3
{
    public enum VolumeType { Master, BGM, SFX }

    public class VolumeSliderUI : MonoBehaviour
    {
        public VolumeType type;

        public Slider slider;
        public GameObject muteIcon;
        public GameObject unMuteIcon;

        float lastNonZeroValue = 1f;

        void Awake()
        {
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(OnSliderChanged);
        }
        void Start()
        {
            slider.SetValueWithoutNotify(GetValue());
            if (slider.value > 0) lastNonZeroValue = slider.value;

            SetValue(slider.value);
            UpdateIcon();
        }

        // 슬라이더 드래그
        public void OnSliderChanged(float value)
        {
            // 0이 아닌 값은 항상 복원값으로 저장
            if (value > 0.0001f)
                lastNonZeroValue = value;

            SetValue(value);
            UpdateIcon();
        }

        // 뮤트 버튼 클릭
        public void OnMuteButton()
        {
            if (slider.value > 0.0001f)
            {
                // 뮤트
                slider.value = 0f;
            }
            else
            {
                // 언뮤트 (복원)
                slider.value = lastNonZeroValue > 0.0001f ? lastNonZeroValue : 1f;
            }

        }

        void UpdateIcon()
        {
            bool isMute = slider.value <= 0.0001f;
            muteIcon.SetActive(isMute);
            unMuteIcon.SetActive(!isMute);
        }

        float GetValue()
        {
            var am = AudioManager.Instance;
            return type == VolumeType.Master ? am.master :
                   type == VolumeType.BGM ? am.bgm :
                                                am.sfx;
        }

        void SetValue(float v)
        {
            var am = AudioManager.Instance;
            if (type == VolumeType.Master) am.master = v;
            else if (type == VolumeType.BGM) am.bgm = v;
            else am.sfx = v;

            am.ApplyVolume(type, v);

        }
    }
}