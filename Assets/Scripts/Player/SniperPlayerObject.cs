using UnityEngine;

public class SniperPlayerObject
{
    public BoltEntity character;
    public BoltConnection connection;

    GameObject spawnPoint;

    public bool IsServer
    {
        get { return connection == null; }
    }

    public bool IsClient
    {
        get { return connection != null; }
    }

    public void Spawn()
    {
        if (!character)
        {
            if (GameManagerController.instance != null)
                spawnPoint = GameManagerController.instance.FindRandomSpawnPoint();

            character = BoltNetwork.Instantiate(BoltPrefabs.SniperPlayer_Spetsnaz, spawnPoint.transform.position, spawnPoint.transform.rotation);

            if (IsServer)
            {
                character.TakeControl();
            }
            else
            {
                character.AssignControl(connection);
            }
        }
    }
}
