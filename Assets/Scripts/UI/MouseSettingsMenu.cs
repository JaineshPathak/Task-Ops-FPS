using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using SimpleJSON;

public class MouseSettingsMenu : MonoBehaviour
{
    public Slider genSensitivitySlider;
    public InputField genSensitivityInputText;

    public Slider aimingSensitivitySlider;
    public InputField aimingSensitivityInputText;

    public Toggle invertMouseToggle;

    private float loadGenSens;
    private float loadAimSens;
    private bool loadInvertMouse;

    private string configFilePath;
    private JSONObject configJSON;

    private void Start()
    {
        configFilePath = Application.persistentDataPath + "/Settings.json";

        if (File.Exists(configFilePath))
        {
            configJSON = new JSONObject();

            string jSONString = File.ReadAllText(configFilePath);
            configJSON = JSON.Parse(jSONString) as JSONObject;
        }

        LoadGenSensitivity();
        LoadAimSensitivity();
        LoadInvertStatus();

        genSensitivityInputText.onValueChanged.AddListener( delegate {SetGeneralSensitivityInput(); });
        aimingSensitivityInputText.onValueChanged.AddListener(delegate { SetAimingSensitivityInput(); });
    }

    //=======================================================================================

    private void LoadGenSensitivity()
    {
        loadGenSens = configJSON["Mouse_General_Sensitivity"];

        genSensitivitySlider.value = loadGenSens;
        genSensitivityInputText.text = loadGenSens.ToString();
    }

    private void SetGeneralSensitivityInput()
    {
        float genSensVal = Convert.ToSingle(genSensitivityInputText.text);

        if (genSensVal < 0)
            genSensVal = 0.1f;
        else if (genSensVal > 5f)
            genSensVal = 5f;

        genSensitivityInputText.text = genSensVal.ToString();

        configJSON.Add("Mouse_General_Sensitivity", genSensVal);
        File.WriteAllText(configFilePath, configJSON.ToString());

        genSensitivitySlider.value = genSensVal;
    }

    public void SetGeneralSensitivity(float val)
    {
        configJSON.Add("Mouse_General_Sensitivity", val);
        File.WriteAllText(configFilePath, configJSON.ToString());

        genSensitivityInputText.text = val.ToString();
    }

    //=======================================================================================

    private void LoadAimSensitivity()
    {
        loadAimSens = configJSON["Mouse_Aim_Sensitivity"];

        aimingSensitivitySlider.value = loadAimSens;
        aimingSensitivityInputText.text = loadAimSens.ToString();
    }

    private void SetAimingSensitivityInput()
    {
        float aimSensVal = Convert.ToSingle(aimingSensitivityInputText.text);

        if (aimSensVal < 0)
            aimSensVal = 0.1f;
        else if (aimSensVal > 5f)
            aimSensVal = 5f;

        aimingSensitivityInputText.text = aimSensVal.ToString();

        configJSON.Add("Mouse_Aim_Sensitivity", aimSensVal);
        File.WriteAllText(configFilePath, configJSON.ToString());

        aimingSensitivitySlider.value = aimSensVal;
    }

    public void SetAimingSensitivity(float val)
    {
        configJSON.Add("Mouse_Aim_Sensitivity", val);
        File.WriteAllText(configFilePath, configJSON.ToString());

        aimingSensitivityInputText.text = val.ToString();
    }

    //=======================================================================================

    private void LoadInvertStatus()
    {
        loadInvertMouse = configJSON["Mouse_Invert"];

        invertMouseToggle.isOn = loadInvertMouse;
    }

    public void SetInvertMouseToggle(bool status)
    {
        configJSON.Add("Mouse_Invert", status);
        File.WriteAllText(configFilePath, configJSON.ToString());
    }

    //=======================================================================================
}
