using UnityEngine;
using System.Collections.Generic;

public class GameManagerController : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnPoints;
    public float respawnTime = 3f;

    public static GameManagerController instance;
    private static Dictionary<string, BoltEntity> players = new Dictionary<string, BoltEntity>();

    public delegate void OnPlayerKilledCallback(BoltEntity Killer, BoltEntity Victim);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Multiple clones of Game Manager Controller detected.");
        else
            instance = this;
    }

    private void Start()
    {
        instance.onPlayerKilledCallback += Killed;
    }

    void Killed(BoltEntity Killer, BoltEntity Victim)
    {
        if(Killer != null && Victim != null)
        {
            if (Killer.isOwner)
                GameUI.instance.SendKillMessage("You Killed " + Victim.transform.name);

            if (Victim.isOwner)
                GameUI.instance.SendKillMessage("Killed By " + Killer.transform.name);

            if (Killer == Victim && Victim.isOwner)
                GameUI.instance.SendKillMessage("Suicide!");
        }
    }

    /*public void SetupSpawnPoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }*/

    public GameObject FindRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    public static void RegisterPlayer(BoltEntity entity)
    {
        players.Add(entity.networkId.ToString(), entity);
    }

    public static void UnregisterPlayer(BoltEntity entity)
    {
        players.Remove(entity.networkId.ToString());
    }

    public static BoltEntity GetPlayer(BoltEntity entity)
    {
        return players[entity.networkId.ToString()];
    }

    void OnGUI ()
    {
        GUILayout.BeginArea(new Rect(10, 100, 500, 500));
        GUILayout.BeginVertical();

        foreach (var item in players.Keys)
        {
            GUILayout.Label(item + " " + players[item].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
