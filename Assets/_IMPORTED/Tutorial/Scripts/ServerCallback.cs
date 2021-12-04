/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class ServerCallback : Bolt.GlobalEventListener
{
    public override void Connected(BoltConnection connection)
    {
        var log = new LogEvent();
        log.Message = string.Format("[0] connected", connection.RemoteEndPoint);
        log.Send();
    }

    public override void Disconnected(BoltConnection connection)
    {
        var log = new LogEvent();
        log.Message = string.Format("[0] disconnected", connection.RemoteEndPoint);
        log.Send();
    }

}
*/