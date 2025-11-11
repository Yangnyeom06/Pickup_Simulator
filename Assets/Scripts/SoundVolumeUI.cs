using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeUI : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider seSlider;

    private void Start()
    {
        // 초기 슬라이더 값 설정
        if (SoundManager.instance != null)
        {
            if (SoundManager.instance.audioSourceBgm != null)
                bgmSlider.value = SoundManager.instance.audioSourceBgm.volume;

            if (SoundManager.instance.audioSourceEffects.Length > 0)
                seSlider.value = SoundManager.instance.audioSourceEffects[0].volume;
        }

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);
    }


    public void SetBGMVolume(float volume)
    {
        if (SoundManager.instance.audioSourceBgm != null)
        {
            SoundManager.instance.audioSourceBgm.volume = volume;
        }
    }

    public void SetSEVolume(float volume)
    {
        foreach (var source in SoundManager.instance.audioSourceEffects)
        {
            if (source != null)
                source.volume = volume;
        }
    }
}