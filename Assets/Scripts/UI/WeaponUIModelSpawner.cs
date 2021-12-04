using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUIModelSpawner : MonoBehaviour
{
    public enum weaponSelectionType
    {
        SEL_Primary,
        SEL_Secondary
    };

    public weaponSelectionType weaponSelection;

    public List<GameObject> uiWeaponModelPrefabs = new List<GameObject>();
    private List<GameObject> uiWeaponModelPrefabsInstance;
    public Transform parentObject;

    private int primaryWeaponID;
    private int secondaryWeaponID;

    // Use this for initialization
    void Start ()
    {
        if (uiWeaponModelPrefabs.Count != 0)
        {
            uiWeaponModelPrefabsInstance = new List<GameObject>(new GameObject[uiWeaponModelPrefabs.Count]);
            SpawnModels();
        }
    }

    void SpawnModels()
    {
        for (int i = 0; i < uiWeaponModelPrefabs.Count; i++)
            uiWeaponModelPrefabsInstance[i] = Instantiate(uiWeaponModelPrefabs[i], transform.position, transform.rotation, parentObject);

        LoadCurrentModel();
    }

    void LoadCurrentModel()
    {
        for (int i = 0; i < uiWeaponModelPrefabs.Count; i++)
            uiWeaponModelPrefabsInstance[i].SetActive(false);

        if (weaponSelection == weaponSelectionType.SEL_Primary)
        {
            if (PlayerPrefs.HasKey("WeaponID"))
                primaryWeaponID = PlayerPrefs.GetInt("WeaponID");

            uiWeaponModelPrefabsInstance[primaryWeaponID].SetActive(true);
        }
        else if (weaponSelection == weaponSelectionType.SEL_Secondary)
        {
            if (PlayerPrefs.HasKey("SecondWeaponID"))
                secondaryWeaponID = PlayerPrefs.GetInt("SecondWeaponID");

            uiWeaponModelPrefabsInstance[secondaryWeaponID].SetActive(true);
        }
    }

    public void SetNewWeapon(int val)
    {
        for (int i = 0; i < uiWeaponModelPrefabs.Count; i++)
            uiWeaponModelPrefabsInstance[i].SetActive(false);

        if (weaponSelection == weaponSelectionType.SEL_Primary)
        {
            PlayerPrefs.SetInt("WeaponID", val);
            uiWeaponModelPrefabsInstance[val].SetActive(true);
        }

        if (weaponSelection == weaponSelectionType.SEL_Secondary)
        {
            PlayerPrefs.SetInt("SecondWeaponID", val);
            uiWeaponModelPrefabsInstance[val].SetActive(true);
        }        
    }
}
