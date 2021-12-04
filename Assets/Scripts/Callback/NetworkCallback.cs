using Bolt;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[BoltGlobalBehaviour]
public class NetworkCallback : GlobalEventListener
{
    GameObject spawnPoint;
    BoltEntity character;

    int playerModelType;

    public static bool IsHeadlessMode()
    {
        return Environment.CommandLine.Contains("-batchmode") && Environment.CommandLine.Contains("-nographics");
    }

    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<ServerRoomToken>();
    }

    public override void SceneLoadLocalDone(string map)
    {
        if (IsHeadlessMode())
            return;

        if (PlayerPrefs.HasKey("PlayerModelID"))
            playerModelType = PlayerPrefs.GetInt("PlayerModelID");

        if (GameManagerController.instance != null)
            spawnPoint = GameManagerController.instance.FindRandomSpawnPoint();

        //character = BoltNetwork.Instantiate(BoltPrefabs.SniperPlayer, spawnPoint.transform.position, spawnPoint.transform.rotation);
        //character.TakeControl();
        
        switch(playerModelType)
        {
            case 0:
                character = BoltNetwork.Instantiate(BoltPrefabs.SniperPlayer_Delta2, spawnPoint.transform.position, spawnPoint.transform.rotation);
                //character.TakeControl();
                break;
            case 1:
                character = BoltNetwork.Instantiate(BoltPrefabs.SniperPlayer_Spetsnaz2, spawnPoint.transform.position, spawnPoint.transform.rotation);
                //character.TakeControl();
                break;
        }

        CameraController.instance.Configure(character);
        MinimapCamera.instance.Configure(character);
        GameUI.instance.SetupUIState(character);
    }

    public override void OnEvent(DamageEvent evnt)
    {
        if ((evnt.VictimID != null) && evnt.VictimID.isOwner && !evnt.VictimID.GetState<ISniperPlayerState>().IsDead)
        {
            evnt.VictimID.GetComponent<PlayerHealth>().TakeDamage(evnt.Damage, evnt.KillerID, evnt.Hitlocation);
        }
    }

    public override void OnEvent(KilledEvent evnt)
    {
        GameManagerController.instance.onPlayerKilledCallback.Invoke(evnt.Killer, evnt.Victim);
    }

    /*void Awake()
    {
        SniperPlayerRegistry.CreateServerPlayer();
    }

    public override void Connected(BoltConnection connection)
    {
        SniperPlayerRegistry.CreateClientPlayer(connection);
    }

    public override void SceneLoadLocalDone(string map)
    {
        GameManagerController.Instantiate();
        GameManagerController.instance.SetupSpawnPoints();

        SniperPlayerRegistry.ServerPlayer.Spawn();
        //GameObject spawnPoint = GetRandomSpawnPoint();

        //BoltNetwork.Instantiate(BoltPrefabs.SniperPlayer, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        SniperPlayerRegistry.GetPlayer(connection).Spawn();
    }*/
}
