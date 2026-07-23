using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider, soundVolumeSlider, masterVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle, vsyncToggle, shadersToggle;
    [SerializeField] private GameObject fullscreenLabel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Renderer2DData renderer2D;

    public void Start()
    {
        // Declare defaults
        if (!PlayerPrefs.HasKey("musicVolume"))
            PlayerPrefs.SetFloat("musicVolume", 50);

        if (!PlayerPrefs.HasKey("soundVolume"))
            PlayerPrefs.SetFloat("soundVolume", 50);

        if (!PlayerPrefs.HasKey("masterVolume"))
            PlayerPrefs.SetFloat("masterVolume", 100);

        if (!PlayerPrefs.HasKey("fullscreen"))
            PlayerPrefs.SetInt("fullscreen", 1);

        if (!PlayerPrefs.HasKey("vsync"))
            PlayerPrefs.SetInt("vsync", 1);

        // Reload saved settings
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume();

        soundVolumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
        SetSoundVolume();

        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();

        if (!Utils.IsWebPlayer())
        {
            fullscreenToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("fullscreen") == 1);
            SetFullscreen();
        }
        else
        {
            fullscreenToggle.gameObject.SetActive(false);
            fullscreenLabel.SetActive(false);

            // Update navigation for other things
            Utils.SetNavigation(vsyncToggle, closeButton, Utils.Direction.DOWN);
            Utils.SetNavigation(musicVolumeSlider, musicVolumeSlider, Utils.Direction.LEFT);
            Utils.SetNavigation(musicVolumeSlider, musicVolumeSlider, Utils.Direction.RIGHT);

            closeButton.GetComponent<NavigationAlternator>().rightNav.selectOnUp = vsyncToggle;
        }

        vsyncToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("vsync") == 1);
        SetVSync();

        shadersToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("shaders") == 1);
        SetShaders();
    }

    public void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
        if (Keyboard.current.f11Key.wasPressedThisFrame)
        {
            Screen.SetResolution(Display.main.systemWidth, (int)(9 / 16f * Display.main.systemWidth), !Screen.fullScreen);
        }
#endif
        
        if (fullscreenToggle.isOn != Screen.fullScreen)
        {
            fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
            PlayerPrefs.SetInt("fullscreen", Screen.fullScreen ? 1 : 0);
        }
    }

    public void SetMusicVolume()
    {
        // AkUnitySoundEngine.SetRTPCValue("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
    }

    public void SetSoundVolume()
    {
        // AkUnitySoundEngine.SetRTPCValue("SFXVolume", soundVolumeSlider.value);
        PlayerPrefs.SetFloat("soundVolume", soundVolumeSlider.value);
    }

    public void SetMasterVolume()
    {
        // AkUnitySoundEngine.SetRTPCValue("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
    }

    public void SetFullscreen()
    {
        if (Utils.IsWebPlayer()) return;
        Screen.SetResolution(Display.main.systemWidth, (int)(9 / 16f * Display.main.systemWidth), fullscreenToggle.isOn);
        PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
    }

    public void SetVSync()
    {
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt("vsync", vsyncToggle.isOn ? 1 : 0);
    }

    public void SetShaders()
    {
        PlayerPrefs.SetInt("shaders", shadersToggle.isOn ? 1 : 0);
        Camera.main.TryGetComponent(out UniversalAdditionalCameraData cameraData);
        if (cameraData)
        {
            cameraData.renderPostProcessing = shadersToggle.isOn;
        }
        renderer2D.rendererFeatures[0].SetActive(shadersToggle.isOn);
    }
}