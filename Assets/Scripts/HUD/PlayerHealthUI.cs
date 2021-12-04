using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : Bolt.GlobalEventListener
{
    BoltEntity me;
    ISniperPlayerState meState;

    [SerializeField] Text healthText;

    public void SetupState(BoltEntity ent)
    {
        me = ent;
        meState = me.GetState<ISniperPlayerState>();
        meState.AddCallback("Health", HealthChanged);
    }

    void HealthChanged()
    {
        healthText.text = meState.Health.ToString();
    }
}
