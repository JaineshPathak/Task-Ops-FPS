using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.IO;
using SimpleJSON;

public class VideoSettingsMenu : MonoBehaviour
{
    private JSONObject configJSON;
    private string configFilePath;

    public Dropdown resolutionDropdown;
    public Dropdown windowModeDropdown;
    public Dropdown graphicsQualityDropdown;
    public Toggle vSyncToggle;
    public Slider resScaleSlider;
    public Toggle motionBlurToggle;
    public Toggle ambientOccToggle;
    public Toggle bloomToggle;
    public Toggle lensFlareToggle;
    public InputField playerNameField;

    private Resolution[] resolutions;
    private FullScreenMode fsMode;
    private string[] quality;
    private int loadResolution;
    private int loadWindowMode;
    private bool loadVSync;
    private string loadPlayerName;
    private float loadResolutionScale;
    private bool loadAO;
    private bool loadBloom;
    private bool loadMotionBlur;
    private bool loadLensFlare;

    private int loadGraphicsQuality;
    private float loadResScale;

    // Use this for initialization
    private void Start ()
    {
        configFilePath = Application.persistentDataPath + "/Settings.json";

        if (File.Exists(configFilePath))
        {
            configJSON = new JSONObject();

            string jSONString = File.ReadAllText(configFilePath);
            configJSON = JSON.Parse(jSONString) as JSONObject;
        }
        else
        {
            File.Create(configFilePath);
        }

        //===========================================================================
        // Graphics Quality Dropdown

        quality = QualitySettings.names;
        graphicsQualityDropdown.ClearOptions();

        List<string> qualityOptions = new List<string>();

        int currentQuality = 0;
        for(int i = 0; i < quality.Length; i++)
        {
            string qualOption = quality[i];
            qualityOptions.Add(qualOption);

            if(i == QualitySettings.GetQualityLevel())
            {
                currentQuality = i;
            }
        }

        graphicsQualityDropdown.AddOptions(qualityOptions);

        LoadGraphicsQuality();

        #region graphics quality old settings code
        /*if (PlayerPrefs.HasKey("CurrentGraphicsQuality"))
        {
            loadGraphicsQuality = PlayerPrefs.GetInt("CurrentGraphicsQuality");
            LoadGraphicsQuality(loadGraphicsQuality);
        }
        else
        {
            graphicsQualityDropdown.value = currentQuality;
        }*/
        #endregion

        //===========================================================================

        LoadWindowMode();

        //Vsync
        LoadToggleVSync();
        #region vync old settings
        /*if (PlayerPrefs.HasKey("CurrentVSync"))
        {
            string loadVSyncStatus = PlayerPrefs.GetString("CurrentVSync").ToLower();
            loadVSync = Convert.ToBoolean(loadVSyncStatus);
            LoadToggleVSync(loadVSync);
        }*/
        #endregion

        //Resolution Scale
        LoadResolutionScale();

        //Bloom
        LoadToggleBloom();

        //AO
        LoadToggleAO();

        //Motion blur
        LoadToggleMotionBlur();

        //Lens Flare
        LoadToggleLensFlare();

        //Player Name
        LoadPlayerName();

        //===========================================================================
        //Resolution Dropdown

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resOptions = new List<string>();

        int currentRes = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            resOptions.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
               resolutions[i].height == Screen.currentResolution.height)
            {
                currentRes = i;
            }
        }

        resolutionDropdown.AddOptions(resOptions);

        loadResolution = configJSON["Window_Resolution"];

        if (PlayerPrefs.HasKey("CurrentResolution"))
            resolutionDropdown.value = loadResolution;
        else
            resolutionDropdown.value = currentRes;

        resolutionDropdown.RefreshShownValue();

        //===========================================================================
    }

    public void SetResolution(int resVal)
    {
        //PlayerPrefs.SetInt("CurrentResolution", resVal);

        configJSON.Add("Window_Resolution", resVal);
        File.WriteAllText(configFilePath, configJSON.ToString());

        Resolution res = resolutions[resVal];
        Screen.SetResolution(res.width, res.height, fsMode);
    }

    //===========================================================================

    private void LoadWindowMode()
    {
        loadWindowMode = configJSON["Window_Mode"];

        windowModeDropdown.value = loadWindowMode;

        switch (loadWindowMode)
        {
            case 0:
                fsMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                fsMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                fsMode = FullScreenMode.MaximizedWindow;
                break;
            case 3:
                fsMode = FullScreenMode.Windowed;
                break;
        }
    }

    public void SetWindowMode(int windowVal)
    {
        //PlayerPrefs.SetInt("CurrentWindowMode", windowVal);
        configJSON.Add("Window_Mode", windowVal);
        File.WriteAllText(configFilePath, configJSON.ToString());

        switch (windowVal)
        {
            case 0:
                fsMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                fsMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                fsMode = FullScreenMode.MaximizedWindow;
                break;
            case 3:
                fsMode = FullScreenMode.Windowed;
                break;
        }
    }

    //===========================================================================

    private void LoadToggleVSync()
    {
        loadVSync = configJSON["vSync"];
        vSyncToggle.isOn = loadVSync;
    }

    public void ToggleVSync(bool vSyncBool)
    {
        //PlayerPrefs.SetString("CurrentVSync", vSyncBool.ToString());

        configJSON.Add("vSync", vSyncBool);
        File.WriteAllText(configFilePath, configJSON.ToString());

        QualitySettings.vSyncCount = vSyncBool ? 1 : 0;
    }

    //===========================================================================

    private void LoadGraphicsQuality()
    {
        loadGraphicsQuality = configJSON["Graphics_Quality"];

        graphicsQualityDropdown.value = loadGraphicsQuality;
        QualitySettings.SetQualityLevel(graphicsQualityDropdown.value);
    }

    public void SetGraphicsQuality(int graphicsVal)
    {
        configJSON.Add("Graphics_Quality", graphicsVal);
        File.WriteAllText(configFilePath, configJSON.ToString());

        //PlayerPrefs.SetInt("CurrentGraphicsQuality", graphicsVal);
    }

    public void ApplyGraphicsQuality()
    {
        int gq = configJSON["Graphics_Quality"];

        QualitySettings.SetQualityLevel(gq);

        Process startProcess = new Process();
        startProcess.StartInfo.FileName = Application.dataPath.Replace("_Data", ".exe");
        startProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startProcess.Start();
        Application.Quit();
    }

    //===========================================================================

    private void LoadResolutionScale()
    {
        loadResolutionScale = configJSON["Resolution_Scale"];

        resScaleSlider.value = loadResolutionScale;
        QualitySettings.resolutionScalingFixedDPIFactor = loadResolutionScale;
    }

    public void SetResolutionScale(float val)
    {
        configJSON.Add("Resolution_Scale", val);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    //===========================================================================

    //Bloom Setting
    private void LoadToggleBloom()
    {
        loadBloom = configJSON["Effect_Bloom"];
        bloomToggle.isOn = loadBloom;
    }

    public void ToggleBloom(bool status)
    {
        configJSON.Add("Effect_Bloom", status);
        File.WriteAllText(configFilePath, configJSON.ToString());

        //PlayerPrefs.SetString("CurrentBloom", status.ToString());
    }

    //===========================================================================
    
    //Ambient Occlusion setting
    private void LoadToggleAO()
    {
        loadAO = configJSON["Effect_Ambient_Occlusion"];
        ambientOccToggle.isOn = loadAO;
    }

    public void ToggleAO(bool status)
    {
        configJSON.Add("Effect_Ambient_Occlusion", status);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    //===========================================================================

    //Motion Blur
    private void LoadToggleMotionBlur()
    {
        loadMotionBlur = configJSON["Effect_Motion_Blur"];
        motionBlurToggle.isOn = loadMotionBlur;
    }

    public void ToggleMotionBlur(bool status)
    {
        configJSON.Add("Effect_Motion_Blur", status);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    //===========================================================================

    //Lens Flares
    private void LoadToggleLensFlare()
    {
        loadLensFlare = configJSON["Effect_Lens_Flare"];
        lensFlareToggle.isOn = loadLensFlare;
    }

    public void ToggleLensFlare(bool status)
    {
        configJSON.Add("Effect_Lens_Flare", status);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    //===========================================================================

    public void LoadPlayerName()
    {
        loadPlayerName = configJSON["Player_Name"];
        playerNameField.text = loadPlayerName;
    }

    public void SetPlayerName(string val)
    {
        configJSON.Add("Player_Name", val);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }
}
