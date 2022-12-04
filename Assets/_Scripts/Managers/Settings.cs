using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using static Joystick;

public class Settings : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioMixer _audioMixer;
    
    [SerializeField] private PostProcessVolume _post;
    [SerializeField] private Bloom _bloom;

    [SerializeField] private Joystick _joystick;

    [Header("Values")]
    [SerializeField] private float _bloomIntensity;

    [Header("UI Values")]
    [SerializeField] private Text _bloomIntensityValueText;
    [SerializeField] private GameObject _mainMenu;

    [SerializeField] private GameObject _normalText;
    [SerializeField] private GameObject _anormalText;

    void Start()
    {
        _post = GameObject.FindObjectOfType<PostProcessVolume>();
        _post.profile.TryGetSettings(out _bloom);

        _joystick = GameObject.FindObjectOfType<Joystick>();

        _mainMenu = GameObject.Find("P.Menu");
    }

    private void Update()
    {
        if (_mainMenu.activeSelf && _bloom.active)
        {
            // In main menu
            _bloom.intensity.value = _bloomIntensity;
        }
        else if (_bloom.active)
        {
            // Not in main menu
            _bloom.intensity.value = Helpers.FromRangeToRange(_bloomIntensity, 0, 100, 0, 1);
        }
    }


    public void SetVolume(float volume)
    {
        _audioMixer.SetFloat("volumen", Mathf.Clamp(volume, -70, 0));
    }

    public void BloomOnOff(bool on)
    {
        _bloom.active = on;
    }

    public void Intensity(float sliderValue)
    {
        if (sliderValue == 0) BloomOnOff(false);
        else BloomOnOff(true);
        
        _bloom.intensity.value = sliderValue;
        _bloomIntensityValueText.text = sliderValue.ToString("0");

        _bloomIntensity = sliderValue;
    }

    public void JoystickType(bool on)
    {
        if (!on) _joystick.TypeAim = AimType.Pull;
        else _joystick.TypeAim = AimType.Stretch;

        _normalText.SetActive(on);
        _anormalText.SetActive(!on);
    }
}
