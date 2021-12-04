using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour
{
    [SerializeField] private Text killerText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private Image suicideIcon;
    [SerializeField] private Text victimText;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(string Killer, Sprite _Icon, string Victim, bool IsSuicide)
    {
        //rectTransform.sizeDelta = new Vector2(Killer.Length + _Icon.textureRect.width + Victim.Length, 25f);

        BoltConsole.Write("Killer Length: " + Killer.Length, Color.white);
        BoltConsole.Write("Icon Width: " + weaponIcon.rectTransform.sizeDelta.x, Color.white);
        BoltConsole.Write("Victim Length: " + Victim.Length, Color.white);

        if (IsSuicide)
        {
            killerText.text = Killer;
            victimText.text = Victim;

            weaponIcon.gameObject.SetActive(false);
            suicideIcon.gameObject.SetActive(true);
            suicideIcon.sprite = _Icon;

            killerText.rectTransform.sizeDelta = new Vector2(Killer.Length * 12f, killerText.rectTransform.sizeDelta.y);
            victimText.rectTransform.sizeDelta = new Vector2(Victim.Length * 12f, victimText.rectTransform.sizeDelta.y);

            rectTransform.sizeDelta = new Vector2(suicideIcon.rectTransform.sizeDelta.x + victimText.rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        }
        else
        {
            killerText.text = Killer;
            victimText.text = Victim;

            suicideIcon.gameObject.SetActive(false);
            weaponIcon.gameObject.SetActive(true);
            weaponIcon.sprite = _Icon;

            killerText.rectTransform.sizeDelta = new Vector2(Killer.Length * 12f, killerText.rectTransform.sizeDelta.y);
            victimText.rectTransform.sizeDelta = new Vector2(Victim.Length * 12f, victimText.rectTransform.sizeDelta.y);

            rectTransform.sizeDelta = new Vector2(killerText.rectTransform.sizeDelta.x + weaponIcon.rectTransform.sizeDelta.x + victimText.rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        }
	}
}
