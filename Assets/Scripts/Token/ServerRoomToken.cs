using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdpKit;

public class ServerRoomToken : Bolt.IProtocolToken
{
    public string activeSceneName;

    public void Read(UdpPacket packet)
    {
        activeSceneName = packet.ReadString();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(activeSceneName);
    }
}
