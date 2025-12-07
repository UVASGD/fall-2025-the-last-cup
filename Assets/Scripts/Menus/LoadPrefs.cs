using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class LoadPrefs : MonoBehaviour
{
    [Header("General Setting")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private MenuController menuController;

    [Header("Volume Settings")]
    [SerializeField] private Slider musicVolSlider = null;
    [SerializeField] private Slider sfxVolSlider = null;
    public AudioMixer audioMixer;

    [Header("Gameplay Setting")]
    [SerializeField] private Slider mouseSensSlider = null;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Dropdown qualityDropdown = null;
    [SerializeField] private TMP_Dropdown resolutionDropdown = null;
    // [SerializeField] private Toggle fullscreenToggle = null;
    private Resolution[] resolutions;
    [SerializeField] private Volume postProcessingVolume;
    private ColorAdjustments colorAdjustments;

    public float SliderToDb(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    private void Awake()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("musicVolume"))
            {
                float localMusicVol = PlayerPrefs.GetFloat("musicVolume");
                musicVolSlider.value = localMusicVol;
                audioMixer.SetFloat("music_volume", SliderToDb(localMusicVol));
            }
            else
            {
                float localMusicVol = 0.5f;
                musicVolSlider.value = localMusicVol;
                audioMixer.SetFloat("music_volume", SliderToDb(localMusicVol));
            }

            if (PlayerPrefs.HasKey("sfxVolume"))
            {
                float localSfxVol = PlayerPrefs.GetFloat("sfxVolume");
                audioMixer.SetFloat("sfx_volume", SliderToDb(localSfxVol));
                sfxVolSlider.value = localSfxVol;
            }
            else
            {
                float localSfxVol = 0.05f;
                audioMixer.SetFloat("sfx_volume", SliderToDb(localSfxVol));
                sfxVolSlider.value = localSfxVol;
            }


            if (PlayerPrefs.HasKey("sensitivity"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("sensitivity");

                if (MouseSensitivityHandler.Instance != null)
                {
                    MouseSensitivityHandler.Instance.SetSensitivity(localSensitivity);
                }

                if (mouseSensSlider != null)
                {
                    mouseSensSlider.value = localSensitivity;
                }
            }
            else
            {
                float localSensitivity = 3f;

                if (MouseSensitivityHandler.Instance != null)
                {
                    MouseSensitivityHandler.Instance.SetSensitivity(localSensitivity);
                }

                if (mouseSensSlider != null)
                {
                    mouseSensSlider.value = localSensitivity;
                }
            }


            if (PlayerPrefs.HasKey("brightness"))
            {
                float localBrightness = PlayerPrefs.GetFloat("brightness");
                if (postProcessingVolume.profile.TryGet(out colorAdjustments))
                {
                    if (colorAdjustments != null)
                        colorAdjustments.postExposure.value = localBrightness;
                }
                /*
                else
                {
                    Debug.LogError("Color Adjustments not found in volume.");
                }
                */
                brightnessSlider.value = localBrightness;
            }
            else
            {
                float localBrightness = 0.5f;
                if (postProcessingVolume.profile.TryGet(out colorAdjustments))
                {
                    if (colorAdjustments != null)
                        colorAdjustments.postExposure.value = localBrightness;
                }
                /*
                else
                {
                    Debug.LogError("Color Adjustments not found in volume.");
                }
                */
                brightnessSlider.value = localBrightness;
            }

            if (PlayerPrefs.HasKey("quality"))
            {
                int localQuality = PlayerPrefs.GetInt("quality");
                QualitySettings.SetQualityLevel(localQuality);
                qualityDropdown.value = localQuality;
            }
            else
            {
                int localQuality = 0;
                QualitySettings.SetQualityLevel(localQuality);
                qualityDropdown.value = localQuality;
            }

            if (PlayerPrefs.HasKey("resolution") && PlayerPrefs.HasKey("isFullscreen"))
            {
                /*
                int localIsFullScreen = PlayerPrefs.GetInt("isFullscreen");

                if (localIsFullScreen == 1)
                {
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                }
                else
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
                */

                Screen.fullScreenMode = FullScreenMode.Windowed;

                resolutions = Screen.resolutions
                    .Where(r => r.refreshRateRatio.value.Equals(Screen.currentResolution.refreshRateRatio.value))
                    .ToArray();

                resolutionDropdown.ClearOptions();

                List<string> options = new List<string>();

                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = resolutions[i].width + " x " + resolutions[i].height;
                    options.Add(option);
                }

                int localResolution = PlayerPrefs.GetInt("resolution");
                Resolution resolution = resolutions[localResolution];
                resolutionDropdown.value = localResolution;
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
            }
            /*
            else
            {
                
                int localIsFullScreen = 0;

                if (localIsFullScreen == 1)
                {
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                }
                else
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
                

                resolutions = Screen.resolutions
                    .Where(r => r.refreshRateRatio.value.Equals(Screen.currentResolution.refreshRateRatio.value))
                    .ToArray();

                resolutionDropdown.ClearOptions();

                List<string> options = new List<string>();

                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = resolutions[i].width + " x " + resolutions[i].height;
                    options.Add(option);
                }

                int localResolution = 0;
                Resolution resolution = resolutions[localResolution];
                resolutionDropdown.value = localResolution;
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
            }
            */

            /*
            if (PlayerPrefs.HasKey("isFullscreen"))
            {
                int localIsFullScreen = PlayerPrefs.GetInt("isFullscreen");

                if (localIsFullScreen == 1)
                {
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                }
                else
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
            }
            else
            {
                int localIsFullScreen = 0;

                if (localIsFullScreen == 1)
                {
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                }
                else
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
            }
            */
        }
    }
}