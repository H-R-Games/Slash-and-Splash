using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class SetAjustes : MonoBehaviour
{
    public AudioMixer audioMixer;
    private PostProcessVolume _postProcessVolume;

    private Bloom _bloom;
    [Header("UI Values")]
    [SerializeField] private Text bloomIntensityValueText;

    public void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings(out _bloom);
    }

    public void SetVolume(float volume)
    {
        if (volume == -70) volume = -80;
        audioMixer.SetFloat("volumen", volume);
    }

    public void BloomOnOff(bool on)
    {
        if(on)
        {
            _bloom.active = true;
        }
        else
        {
            _bloom.active = false;
        }
    }

    public void Intensity(float sliderValue)
    {
        _bloom.intensity.value = sliderValue;
        bloomIntensityValueText.text = sliderValue.ToString("0");
    }



}
