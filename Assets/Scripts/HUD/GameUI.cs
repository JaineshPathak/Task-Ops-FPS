using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;
using System.IO;
using SimpleJSON;

public class GameUI : BoltSingletonPrefab<GameUI>
{
    [SerializeField] private RawImage miniMapRawImage;
    [SerializeField] private RenderTexture minimapTexture;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Image Crosshair;
    [SerializeField] private RectTransform youKilledObj;
    [SerializeField] private AudioMixer mainMixer;

    private string configFilePath;
    private JSONObject configJSON;
    private float loadMenuVolume;

    void Start()
    {
        SetupMiniMap();
        PauseMenu.isOn = false;
        Cursor.visible = pauseMenu.activeSelf;

        configFilePath = Application.persistentDataPath + "/Settings.json";

        if (File.Exists(configFilePath))
        {
            configJSON = new JSONObject();
            string jSONString = File.ReadAllText(configFilePath);
            configJSON = JSON.Parse(jSONString) as JSONObject;
        }

        loadMenuVolume = configJSON["Audio_Menu_Volume"];
        mainMixer.SetFloat("MenuVol", loadMenuVolume);
    }

    public void SendKillMessage(string msg)
    {
        if(!youKilledObj.gameObject.activeSelf)
            youKilledObj.gameObject.SetActive(true);

        youKilledObj.sizeDelta = new Vector2(msg.Length * 9f, youKilledObj.sizeDelta.y);
        youKilledObj.GetComponentInChildren<Text>().text = msg.ToUpper();

        StartCoroutine(DeactivateObj(5f));
    }

    IEnumerator DeactivateObj(float time)
    {
        yield return new WaitForSeconds(time);

        youKilledObj.gameObject.SetActive(false);
    }

    public void ToggleCrosshair(bool toggle)
    {
        Crosshair.enabled = toggle;
    }

    void SetupMiniMap()
    {
        if (miniMapRawImage != null && minimapTexture != null)
            miniMapRawImage.texture = minimapTexture;
    }

    public void SetupUIState(BoltEntity entity)
    {
        GetComponent<PlayerHealthUI>().SetupState(entity);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;

        Cursor.visible = pauseMenu.activeSelf;
        Cursor.lockState = pauseMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
