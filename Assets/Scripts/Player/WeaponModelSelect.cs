using UnityEngine;
using UnityEngine.UI;

public class WeaponModelSelect : MonoBehaviour
{
    Dropdown weaponDropdown;

    [SerializeField] byte type;  //0 - pri, 1 - secon

    int priModelID, seconModelID;

    // Use this for initialization
    void Start()
    {
        weaponDropdown = GetComponent<Dropdown>();

        LoadValue(weaponDropdown);
        weaponDropdown.onValueChanged.AddListener(delegate { DropDownChanged(weaponDropdown); });
    }

    void LoadValue(Dropdown dropValue)
    {
        switch(type)
        {
            case 0:
                if (PlayerPrefs.HasKey("WeaponID"))
                    priModelID = PlayerPrefs.GetInt("WeaponID");

                    dropValue.value = priModelID;
                break;
            case 1:
                if (PlayerPrefs.HasKey("SecondWeaponID"))
                    seconModelID = PlayerPrefs.GetInt("SecondWeaponID");

                dropValue.value = seconModelID;
                break;
        }
    }

    void DropDownChanged(Dropdown changed)
    {
        switch(type)
        {
            case 0:
                PlayerPrefs.SetInt("WeaponID", changed.value);
                break;
            case 1:
                PlayerPrefs.SetInt("SecondWeaponID", changed.value);
                break;
        }
        
        //Debug.Log("PlayerModelID changed to: " + changed.value);
    }
}
