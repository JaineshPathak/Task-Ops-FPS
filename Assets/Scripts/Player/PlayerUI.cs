using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    PlayerHealth phealth;

    [SerializeField] RawImage miniMapRawImage;
    [SerializeField] RenderTexture minimapTexture;
    [SerializeField] Text healthText;

    public void SetHealthController(PlayerHealth _pHealth)
    {
        phealth = _pHealth;
    }

    public void SetupMiniMap()
    {
        if (miniMapRawImage != null && minimapTexture != null)
            miniMapRawImage.texture = minimapTexture;
    }

    public void HealthChanged()
    {
        healthText.text = phealth.state.Health.ToString();
    }
}
