using Team3;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        var am = AudioManager.Instance;
        if (am == null) return;

        // master는 여기서만 곱함
        source.volume = am.sfx * am.master;
    }
}
