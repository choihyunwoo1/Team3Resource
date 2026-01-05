using UnityEngine;
using UnityEngine.Audio;

namespace Team3
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Range(0, 1)] public float master = 1f;
        [Range(0, 1)] public float bgm = 1f;
        [Range(0, 1)] public float sfx = 1f;

        public AudioMixer mixer;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
        public void ApplyVolume(VolumeType type, float value)
        {
            float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

            if (type == VolumeType.Master)
                mixer.SetFloat("MasterVol", dB);
            else if (type == VolumeType.BGM)
                mixer.SetFloat("BGMVol", dB);
            else
                mixer.SetFloat("SFXVol", dB);
        }

        public float BGMVolume => master * bgm;
        public float SFXVolume => master * sfx;
    }
}