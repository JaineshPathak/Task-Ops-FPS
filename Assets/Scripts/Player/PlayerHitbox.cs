using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private BoxCollider box;

    public enum hitBox
    {
        head,
        hand,
        chest,
        leg
    };

    public hitBox hitBoxType;

    private void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            if(hitBoxType == hitBox.head)
                BoltConsole.Write("Bullet Hit HEAD!", Color.white);
            else if(hitBoxType == hitBox.chest)
                BoltConsole.Write("Bullet Hit CHEST!", Color.red);
            else if (hitBoxType == hitBox.hand)
                BoltConsole.Write("Bullet Hit HANDS!", Color.green);
            else if (hitBoxType == hitBox.leg)
                BoltConsole.Write("Bullet Hit LEGS!", Color.cyan);
        }
    }*/
}
