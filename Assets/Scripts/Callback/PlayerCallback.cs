using System.Collections;
using System.Collections.Generic;
using Bolt;
using UdpKit;

[BoltGlobalBehaviour("DesertLevel", "PrototypeLevel_1", "WeaponRange_Scene, SceneForVideo")]
public class PlayerCallback : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string map)
    {
        CameraController.Instantiate();
        MinimapCamera.Instantiate();
        GameUI.Instantiate();
    }

    /*public override void ControlOfEntityGained(BoltEntity entity)
    {
        CameraController.instance.Configure(entity);
        MinimapCamera.instance.Configure(entity);
        GameUI.instance.SetupUIState(entity);
        CameraController.instance.transform.SetParent(entity.transform);
    }*/
}
