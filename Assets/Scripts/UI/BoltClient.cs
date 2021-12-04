using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltClient : Bolt.GlobalEventListener
{
	public void StartClient()
    {
        if(!BoltNetwork.IsRunning)
            BoltLauncher.StartClient();
    }
}
