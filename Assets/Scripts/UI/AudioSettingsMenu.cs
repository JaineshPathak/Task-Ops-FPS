using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using UnityEngine.Audio;

public class AudioSettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider masterVolSlider;
    [SerializeField] private Slider musicVolSlider;
    [SerializeField] private Slider gameVolSlider;
    [SerializeField] private Slider menuVolSlider;

    private float loadMasterVolume;
    private float loadMusicVolume;
    private float loadGameVolume;
    private float loadMenuVolume;

    private string configFilePath;
    private JSONObject configJSON;

    // Start is called before the first frame update
    void Start()
    {
        configFilePath = Application.persistentDataPath + "/Settings.json";

        if (File.Exists(configFilePath))
        {
            configJSON = new JSONObject();

            string jSONString = File.ReadAllText(configFilePath);
            configJSON = JSON.Parse(jSONString) as JSONObject;
        }

        LoadMasterVolume();
        LoadMusicVolume();
        LoadGameVolume();
        LoadMenuVolume();
    }

    // -----------------------------------------------------------------------------------------------------------

    private void LoadMasterVolume()
    {
        loadMasterVolume = configJSON["Audio_Master_Volume"];
        masterVolSlider.value = loadMasterVolume;
        mainMixer.SetFloat("MasterVol", loadMasterVolume);
    }

    public void SetMasterVolume(float val)
    {
        mainMixer.SetFloat("MasterVol", val);

        configJSON.Add("Audio_Master_Volume", val);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    // -----------------------------------------------------------------------------------------------------------

    private void LoadMusicVolume()
    {
        loadMusicVolume = configJSON["Audio_Music_Volume"];
        musicVolSlider.value = loadMusicVolume;
        mainMixer.SetFloat("MusicVol", loadMusicVolume);
    }

    public void SetMusicVolume(float val)
    {
        mainMixer.SetFloat("MusicVol", val);

        configJSON.Add("Audio_Music_Volume", val);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    // -----------------------------------------------------------------------------------------------------------

    private void LoadGameVolume()
    {
        loadGameVolume = configJSON["Audio_Game_Volume"];
        gameVolSlider.value = loadGameVolume;
        mainMixer.SetFloat("GameVol", loadGameVolume);
    }

    public void SetGameVolume(float val)
    {
        mainMixer.SetFloat("GameVol", val);

        configJSON.Add("Audio_Game_Volume", val);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    // -----------------------------------------------------------------------------------------------------------

    private void LoadMenuVolume()
    {
        loadMenuVolume = configJSON["Audio_Menu_Volume"];
        menuVolSlider.value = loadMenuVolume;
        mainMixer.SetFloat("MenuVol", loadMenuVolume);
    }

    public void SetMenuVolume(float val)
    {
        mainMixer.SetFloat("MenuVol", val);

        configJSON.Add("Audio_Menu_Volume", val);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    // -----------------------------------------------------------------------------------------------------------
}
