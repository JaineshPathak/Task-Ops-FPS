using UnityEngine;

public class KillFeed : MonoBehaviour
{
    [SerializeField] private Sprite suicideIcon;
    [SerializeField] private GameObject killFeedPrefab;

    void Start()
    {
        GameManagerController.instance.onPlayerKilledCallback += BroadcastKillFeed;
    }

    void BroadcastKillFeed(BoltEntity Killer, BoltEntity Victim)
    {
        GameObject go = Instantiate(killFeedPrefab, transform);
        go.transform.SetAsFirstSibling();

        if(Killer == Victim)
            go.GetComponent<KillFeedItem>().Setup("", suicideIcon, Killer.transform.name, true);
        else
            go.GetComponent<KillFeedItem>().Setup(Killer.transform.name, Killer.GetComponent<PlayerController>().activeWeapon.weaponIcon, Victim.transform.name, false);

        Destroy(go, 3f);
    }
}
