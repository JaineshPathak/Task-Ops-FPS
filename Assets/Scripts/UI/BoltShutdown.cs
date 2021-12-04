using UnityEngine;

public class BoltShutdown : Bolt.GlobalEventListener
{
	public void ShutdownBolt()
    {
        if(BoltNetwork.IsRunning)
            BoltLauncher.Shutdown();
    }
}
