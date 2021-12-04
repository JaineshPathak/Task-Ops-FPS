using Bolt;
using UnityEngine;
using UnityEngine.UI;
using Bolt.photon;
using Bolt.Utils;

public class UICreateRoom : Bolt.GlobalEventListener
{
    [System.Serializable]
    public class SceneLevels
    {
        public string sceneName;
        public Sprite sceneScreenshotImage;
    }

    public InputField roomNameField;
    public Slider slotSlider;
    public Dropdown sceneLoadDropdown;
    public Image levelScreenshot;

    [SerializeField] private SceneLevels[] sceneNames;
    private string levelName;

    void Awake()
    {
        BoltLauncher.SetUdpPlatform(new PhotonPlatform());
    }

    void Start()
    {
        sceneLoadDropdown.onValueChanged.AddListener(delegate { LoadScreenshot(sceneLoadDropdown.value); });
        LoadScreenshot(sceneLoadDropdown.value);
    }

    void LoadScreenshot(int val)
    {
        Debug.Log(sceneLoadDropdown.captionText.text);
        levelScreenshot.sprite = sceneNames[val].sceneScreenshotImage;

        levelName = sceneLoadDropdown.captionText.text;
    }

    public void CreateBoltRoom()
    {
        if (BoltNetwork.IsRunning)
            BoltNetwork.Shutdown();

        BoltConfig config = BoltRuntimeSettings.instance.GetConfigCopy();
        config.serverConnectionLimit = (int)slotSlider.value;

        BoltLauncher.StartServer(config);
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            ServerRoomToken roomToken = new ServerRoomToken
            {
                activeSceneName = levelName
            };

            string roomName = null;

            if (roomNameField.text.Length == 0)
                roomName = "Default Room";
            else
                roomName = roomNameField.text;

            BoltNetwork.SetServerInfo(roomName, roomToken);
            BoltNetwork.LoadScene(sceneNames[sceneLoadDropdown.value].sceneName);
        }
    }
}
