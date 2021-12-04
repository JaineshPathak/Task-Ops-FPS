using UnityEngine;
using System;
using UdpKit;

public class BoltStartQuickMatch : Bolt.GlobalEventListener
{
    //[SerializeField] InputField roomNameInput;

	/*public void StartBoltServer()
    {
        BoltLauncher.StartServer();
    }*/

    public void StartBoltClient()
    {
        BoltLauncher.StartClient();
    }

    /*public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            string matchName = null;

            if (roomNameInput != null)
            {
                if (roomNameInput.text != "")
                    matchName = roomNameInput.text;
                else
                    matchName = Guid.NewGuid().ToString();
            }
            matchName = Guid.NewGuid().ToString();
            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene("DesertLevel");
        }
    }*/

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session List Updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                var roomName = photonSession.HostName;
                Debug.Log("Room Name is: " + roomName);

                BoltNetwork.Connect(photonSession);
            }
        }
    }
}
