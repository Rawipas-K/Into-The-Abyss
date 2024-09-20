using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeSetting : MonoBehaviour
{
    public static VolumeSetting instance;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] bool isMainMenu, isInGame;
    public const string MIXER_MUSIC = "MusicVolume";
    public const string MIXER_SFX = "SFXVolume";

    private const string VolumeSettingFileName = "volumeSetting.json";

    private VolumeData volumeData;

    void Awake()
    {
        instance = this;
        LoadVolumeSettings();
    }

    private void Start()
    {
        LoadVolumeSettings();
        if (isMainMenu)
        {
            MainMenu.instance.musicSlider.onValueChanged.AddListener(SetMusicVolume);
            MainMenu.instance.sfxSlider.onValueChanged.AddListener(SetSfxVolume);

            MainMenu.instance.musicSlider.value = volumeData.musicVolume;
            MainMenu.instance.sfxSlider.value = volumeData.sfxVolume;
        }
        else if (isInGame)
        {
            UIController.instance.musicSlider.onValueChanged.AddListener(SetMusicVolume);
            UIController.instance.sfxSlider.onValueChanged.AddListener(SetSfxVolume);

            UIController.instance.musicSlider.value = volumeData.musicVolume;
            UIController.instance.sfxSlider.value = volumeData.sfxVolume;
        }
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20f);
        volumeData.musicVolume = value;
        SaveVolumeSettings();
    }

    public void SetSfxVolume(float value)
    {
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20f);
        volumeData.sfxVolume = value;
        SaveVolumeSettings();
    }

    private void LoadVolumeSettings()
    {
        string volumeSettingsPath = Path.Combine(Application.persistentDataPath, VolumeSettingFileName);

        // Check if the file exists
        if (!File.Exists(volumeSettingsPath))
        {
            // If the file doesn't exist, create it with default volume settings
            volumeData = new VolumeData
            {
                musicVolume = 0.5f, // Default music volume
                sfxVolume = 0.5f    // Default SFX volume
            };

            SaveVolumeSettings(); // Save the default volume settings to the file
        }
        else
        {
            // If the file exists, load the volume settings from it
            try
            {
                volumeData = JsonUtility.FromJson<VolumeData>(File.ReadAllText(volumeSettingsPath));
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading volume settings: {e.Message}");
            }
        }

        // Apply loaded volumes to the mixer for immediate effect
        mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(volumeData.musicVolume) * 20f);
        mixer.SetFloat(MIXER_SFX, Mathf.Log10(volumeData.sfxVolume) * 20f);
    }


    private void SaveVolumeSettings()
    {
        string volumeSettingsPath = Path.Combine(Application.persistentDataPath, VolumeSettingFileName);
        string jsonData = JsonUtility.ToJson(volumeData);
        File.WriteAllText(volumeSettingsPath, jsonData);
    }
}

[Serializable]
public class VolumeData
{
    public float musicVolume;
    public float sfxVolume;
}
