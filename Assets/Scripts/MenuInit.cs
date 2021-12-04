﻿using System.Collections;
using UnityEngine;
using System;
using UdpKit;

public class MenuInit : Bolt.GlobalEventListener {

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

        if(GUILayout.Button("Start Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            //Start the server
            BoltLauncher.StartServer();
        }

        if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            //Start the server
            BoltLauncher.StartClient();
        }

        GUILayout.EndArea();
    }

    public override void BoltStartDone()
    {
        if(BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();

            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene("SceneForVideo");
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session List Updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if(photonSession.Source == UdpSessionSource.Photon)
            {
                BoltNetwork.Connect(photonSession);
            }
        }
    }
}