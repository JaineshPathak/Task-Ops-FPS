using System;

public class BoltLoadScene : Bolt.GlobalEventListener
{
    private string LevelName;

    public void StartBoltServer(String _levelName)
    {
        LevelName = _levelName;
        BoltLauncher.StartServer();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            string matchName = null;

            matchName = Guid.NewGuid().ToString();
            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene(LevelName);
        }
    }
}
