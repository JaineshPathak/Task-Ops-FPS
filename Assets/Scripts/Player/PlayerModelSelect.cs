using UnityEngine;
using UnityEngine.UI;

public class PlayerModelSelect : MonoBehaviour
{
    Dropdown playerDropDown;
    int modelID;

	// Use this for initialization
	void Start ()
    {
        playerDropDown = GetComponent<Dropdown>();

        LoadValue(playerDropDown);
        playerDropDown.onValueChanged.AddListener(delegate { DropDownChanged(playerDropDown); });
	}

    void LoadValue(Dropdown dropValue)
    {
        if (PlayerPrefs.HasKey("PlayerModelID"))
            modelID = PlayerPrefs.GetInt("PlayerModelID");

        dropValue.value = modelID;
    }

    void DropDownChanged(Dropdown changed)
    {
        PlayerPrefs.SetInt("PlayerModelID", changed.value);
        //Debug.Log("PlayerModelID changed to: " + changed.value);
    }
}
