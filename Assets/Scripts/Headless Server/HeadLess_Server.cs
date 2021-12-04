using System;
using Bolt.photon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadLess_Server : Bolt.GlobalEventListener
{
    public string Map = "";         //Name of the map to load
    public string RoomID = "";      //Room Name
    public string maxPlayers = "";   //Max Slots

    private string mapName;

    public override void BoltStartBegin()
    {
        // Register any Protocol Token that are you using
        BoltNetwork.RegisterTokenClass<ServerRoomToken>();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            switch(Map)
            {
                case "DesertLevel":
                    mapName = "Desert";
                    break;
                case "Prototype_Level1":
                    mapName = "Prototype Level";
                    break;
            }

            // Create some room custom properties
            ServerRoomToken roomToken = new ServerRoomToken
            {
                activeSceneName = mapName
            };

            // If RoomID was not set, create a random one
            if (RoomID.Length == 0)
            {
                RoomID = "Default Room";
            }

            // Create the Photon Room
            BoltNetwork.SetServerInfo(RoomID, roomToken);

            // Load the requested Level
            BoltNetwork.LoadScene(Map);
        }
    }

    // Use this for initialization
    void Start()
    {
        // Get custom arguments from command line
        Map = GetArg("-m", "-map") ?? Map;
        RoomID = GetArg("-r", "-room") ?? RoomID;
        maxPlayers = GetArg("-mc", "-maxconn") ?? maxPlayers;

        // Validate the requested Level
        var validMap = false;

        foreach (string value in BoltScenes.AllScenes)
        {
            if (SceneManager.GetActiveScene().name != value)
            {
                if (Map == value)
                {
                    validMap = true;
                    break;
                }
            }
        }

        if (!validMap)
        {
            Debug.LogError("Invalid configuration: please verify level name");
            Application.Quit();
        }

        BoltConfig config = BoltRuntimeSettings.instance.GetConfigCopy();
        config.serverConnectionLimit = 12;

        // Start the Server
        BoltLauncher.StartServer(config);
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Utility function to detect if the game instance was started in headless mode.
    /// </summary>
    /// <returns><c>true</c>, if headless mode was ised, <c>false</c> otherwise.</returns>
    public static bool IsHeadlessMode()
    {
        return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
    }

    static string GetArg(params string[] names)
    {
        var args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            foreach (var name in names)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
        }

        return null;
    }
}